using System.Data;
using Oracle.ManagedDataAccess.Client;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms;

internal sealed class OrderService
{
    private static readonly IReadOnlyDictionary<string, string[]> Transitions = new Dictionary<string, string[]>
    {
        ["PENDING"] = ["CONFIRMED", "CANCELLED"],
        ["CONFIRMED"] = ["PREPARING", "CANCELLED"],
        ["PREPARING"] = ["SHIPPING", "CANCELLED"],
        ["SHIPPING"] = ["COMPLETED", "FAILED"],
        ["COMPLETED"] = [], ["CANCELLED"] = [], ["FAILED"] = []
    };

    public DataTable List(string search = "", string status = "")
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT o.ORDER_ID,o.ORDER_CODE,o.CUSTOMER_NAME,o.CUSTOMER_PHONE,o.SHIPPING_ADDRESS,o.ORDER_STATUS,o.STOCK_RESERVED,o.CREATED_AT,o.TOTAL_AMOUNT,COALESCE(e.FULL_NAME,u.USERNAME,'Chưa phân công') AS ASSIGNEE FROM ONLINE_ORDER o LEFT JOIN APP_USER u ON u.USER_ID=o.ASSIGNED_TO_USER_ID LEFT JOIN EMPLOYEE e ON e.EMPLOYEE_ID=u.EMPLOYEE_ID WHERE (:status IS NULL OR o.ORDER_STATUS=:status) AND (:search IS NULL OR UPPER(o.ORDER_CODE) LIKE :pattern OR UPPER(o.CUSTOMER_NAME) LIKE :pattern OR o.CUSTOMER_PHONE LIKE :pattern) ORDER BY o.CREATED_AT DESC";
        cmd.Parameters.Add("status", OracleDbType.Varchar2, 20).Value = string.IsNullOrWhiteSpace(status) ? DBNull.Value : status;
        var term = search.Trim().ToUpperInvariant(); cmd.Parameters.Add("search", OracleDbType.Varchar2, 80).Value = term.Length == 0 ? DBNull.Value : term; cmd.Parameters.Add("pattern", OracleDbType.Varchar2, 100).Value = "%" + term + "%";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable AssignableStaff()
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT u.USER_ID,NVL(e.FULL_NAME,u.USERNAME)||' ('||u.ROLE_CODE||')' AS DISPLAY_NAME FROM APP_USER u LEFT JOIN EMPLOYEE e ON e.EMPLOYEE_ID=u.EMPLOYEE_ID WHERE u.IS_ACTIVE=1 AND u.ROLE_CODE IN ('CASHIER','WAREHOUSE') ORDER BY DISPLAY_NAME";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable History(int orderId)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT h.CHANGED_AT,h.OLD_STATUS,h.NEW_STATUS,NVL(e.FULL_NAME,u.USERNAME) AS ACTOR,h.NOTE FROM ORDER_STATUS_HISTORY h JOIN APP_USER u ON u.USER_ID=h.CHANGED_BY LEFT JOIN EMPLOYEE e ON e.EMPLOYEE_ID=u.EMPLOYEE_ID WHERE h.ORDER_ID=:id ORDER BY h.CHANGED_AT DESC,h.HISTORY_ID DESC";
        cmd.Parameters.Add("id", OracleDbType.Int32).Value = orderId; var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public void Assign(UserSession actor, int orderId, int userId)
    {
        actor.Require("CASHIER", "WAREHOUSE"); using var c = OracleConnectionFactory.Create(); c.Open(); using var tx = c.BeginTransaction();
        try
        {
            string current;
            using (var read = c.CreateCommand()) { read.Transaction = tx; read.BindByName = true; read.CommandText = "SELECT ORDER_STATUS FROM ONLINE_ORDER WHERE ORDER_ID=:id FOR UPDATE"; read.Parameters.Add("id", OracleDbType.Int32).Value = orderId; current = Convert.ToString(read.ExecuteScalar()) ?? throw new InvalidOperationException("Không tìm thấy đơn hàng."); }
            if (current is "COMPLETED" or "CANCELLED" or "FAILED") throw new InvalidOperationException("Không thể phân công đơn đã kết thúc.");
            using (var validate = c.CreateCommand()) { validate.Transaction = tx; validate.BindByName = true; validate.CommandText = "SELECT COUNT(*) FROM APP_USER WHERE USER_ID=:userId AND IS_ACTIVE=1 AND ROLE_CODE IN ('CASHIER','WAREHOUSE')"; validate.Parameters.Add("userId", OracleDbType.Int32).Value = userId; if (Convert.ToInt32(validate.ExecuteScalar()) != 1) throw new InvalidOperationException("Tài khoản nhận đơn không hoạt động hoặc không phải nhân viên thu ngân/kho."); }
            using (var update = c.CreateCommand()) { update.Transaction = tx; update.BindByName = true; update.CommandText = "UPDATE ONLINE_ORDER SET ASSIGNED_TO_USER_ID=:userId,UPDATED_AT=SYSTIMESTAMP WHERE ORDER_ID=:id"; update.Parameters.Add("userId", OracleDbType.Int32).Value = userId; update.Parameters.Add("id", OracleDbType.Int32).Value = orderId; update.ExecuteNonQuery(); }
            AddHistory(c, tx, orderId, current, current, "Đã phân công cho tài khoản " + userId, actor.UserId); tx.Commit();
        }
        catch { tx.Rollback(); throw; }
    }
    public DataTable OrderableVariants()
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT v.VARIANT_ID,v.SKU,s.STYLE_NAME||' / '||v.SIZE_CODE||' / '||v.COLOR AS ITEM,v.SELL_PRICE,v.STOCK_QTY FROM SHOE_VARIANT v JOIN SHOE_STYLE s ON s.STYLE_ID=v.STYLE_ID WHERE v.IS_ACTIVE=1 AND s.IS_ACTIVE=1 ORDER BY s.STYLE_NAME,v.SIZE_CODE,v.COLOR";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public string Create(UserSession actor, string customerName, string phone, string address, int variantId, int quantity)
    {
        actor.Require("CASHIER", "WAREHOUSE");
        if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Nhập tên, số điện thoại và địa chỉ giao hàng.");
        if (quantity <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0.");
        using var c = OracleConnectionFactory.Create(); c.Open(); using var tx = c.BeginTransaction();
        try
        {
            string sku, description; decimal price;
            using (var p = c.CreateCommand())
            {
                p.Transaction = tx; p.BindByName = true;
                p.CommandText = "SELECT v.SKU,s.STYLE_NAME||' - '||s.BRAND||' / '||v.SIZE_CODE||' / '||v.COLOR,v.SELL_PRICE FROM SHOE_VARIANT v JOIN SHOE_STYLE s ON s.STYLE_ID=v.STYLE_ID WHERE v.VARIANT_ID=:id AND v.IS_ACTIVE=1 AND s.IS_ACTIVE=1";
                p.Parameters.Add("id", OracleDbType.Int32).Value = variantId; using var reader = p.ExecuteReader();
                if (!reader.Read()) throw new InvalidOperationException("Không tìm thấy sản phẩm đang kinh doanh.");
                sku = reader.GetString(0); description = reader.GetString(1); price = Convert.ToDecimal(reader.GetValue(2));
            }
            var id = SalesService.NextId(c, tx, "SEQ_ONLINE_ORDER"); var code = $"ON{DateTime.Now:yyyyMMddHHmmss}{id:D6}"; var total = price * quantity;
            if (total > 999_999_999_999.99m) throw new ArgumentException("Tổng đơn vượt giới hạn lưu trữ Oracle.");
            using (var header = c.CreateCommand())
            {
                header.Transaction = tx; header.BindByName = true;
                header.CommandText = "INSERT INTO ONLINE_ORDER(ORDER_ID,ORDER_CODE,CUSTOMER_NAME,CUSTOMER_PHONE,SHIPPING_ADDRESS,CREATED_BY,TOTAL_AMOUNT) VALUES(:id,:code,:name,:phone,:address,:actor,:total)";
                SalesService.Add(header,"id",OracleDbType.Int32,id); SalesService.Add(header,"code",OracleDbType.Varchar2,code); SalesService.Add(header,"name",OracleDbType.NVarchar2,customerName.Trim()); SalesService.Add(header,"phone",OracleDbType.Varchar2,phone.Trim());
                SalesService.Add(header,"address",OracleDbType.NVarchar2,address.Trim()); SalesService.Add(header,"actor",OracleDbType.Int32,actor.UserId); SalesService.Add(header,"total",OracleDbType.Decimal,total); header.ExecuteNonQuery();
            }
            using (var line = c.CreateCommand())
            {
                line.Transaction = tx; line.BindByName = true;
                line.CommandText = "INSERT INTO ONLINE_ORDER_LINE(ORDER_ID,LINE_NO,VARIANT_ID,SKU,ITEM_DESCRIPTION,QUANTITY,UNIT_PRICE,LINE_TOTAL) VALUES(:id,1,:variant,:sku,:description,:qty,:price,:total)";
                SalesService.Add(line,"id",OracleDbType.Int32,id); SalesService.Add(line,"variant",OracleDbType.Int32,variantId); SalesService.Add(line,"sku",OracleDbType.Varchar2,sku); SalesService.Add(line,"description",OracleDbType.NVarchar2,description);
                SalesService.Add(line,"qty",OracleDbType.Int32,quantity); SalesService.Add(line,"price",OracleDbType.Decimal,price); SalesService.Add(line,"total",OracleDbType.Decimal,total); line.ExecuteNonQuery();
            }
            AddHistory(c, tx, id, null, "PENDING", "Tiếp nhận đơn online", actor.UserId);
            tx.Commit(); return code;
        }
        catch { tx.Rollback(); throw; }
    }

    public void UpdateStatus(UserSession actor, int orderId, string newStatus, string note)
    {
        actor.Require("CASHIER", "WAREHOUSE");
        using var c = OracleConnectionFactory.Create(); c.Open(); using var tx = c.BeginTransaction();
        try
        {
            string oldStatus; bool reserved;
            using (var read = c.CreateCommand())
            {
                read.Transaction = tx; read.BindByName = true; read.CommandText = "SELECT ORDER_STATUS,STOCK_RESERVED FROM ONLINE_ORDER WHERE ORDER_ID=:id FOR UPDATE";
                read.Parameters.Add("id", OracleDbType.Int32).Value = orderId; using var r = read.ExecuteReader();
                if (!r.Read()) throw new InvalidOperationException("Không tìm thấy đơn hàng.");
                oldStatus = r.GetString(0); reserved = Convert.ToInt32(r.GetValue(1)) == 1;
            }
            if (!Transitions.TryGetValue(oldStatus, out var allowed) || !allowed.Contains(newStatus))
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ {oldStatus} sang {newStatus}.");
            if (newStatus == "CONFIRMED") Reserve(c, tx, orderId, actor.UserId);
            if (reserved && (newStatus == "CANCELLED" || newStatus == "FAILED")) Release(c, tx, orderId, actor.UserId);
            using (var update = c.CreateCommand())
            {
                update.Transaction = tx; update.BindByName = true; update.CommandText = "UPDATE ONLINE_ORDER SET ORDER_STATUS=:status,STOCK_RESERVED=:reserved,UPDATED_AT=SYSTIMESTAMP WHERE ORDER_ID=:id";
                SalesService.Add(update,"status",OracleDbType.Varchar2,newStatus); SalesService.Add(update,"reserved",OracleDbType.Int32,newStatus is "CONFIRMED" or "PREPARING" or "SHIPPING" ? 1 : 0); SalesService.Add(update,"id",OracleDbType.Int32,orderId); update.ExecuteNonQuery();
            }
            AddHistory(c, tx, orderId, oldStatus, newStatus, note, actor.UserId); tx.Commit();
        }
        catch { tx.Rollback(); throw; }
    }

    private static void Reserve(OracleConnection c, OracleTransaction tx, int orderId, int actor)
    {
        var lines = ReadLines(c, tx, orderId);
        foreach (var line in lines.OrderBy(x => x.VariantId))
        {
            var p = SalesService.LockVariant(c, tx, line.VariantId);
            if (p.Stock < line.Quantity) throw new InvalidOperationException($"Không đủ tồn để xác nhận {line.Sku}; còn {p.Stock}.");
            using var update = c.CreateCommand(); update.Transaction = tx; update.BindByName = true; update.CommandText = "UPDATE SHOE_VARIANT SET STOCK_QTY=STOCK_QTY-:qty WHERE VARIANT_ID=:id AND STOCK_QTY>=:qty";
            SalesService.Add(update,"qty",OracleDbType.Int32,line.Quantity); SalesService.Add(update,"id",OracleDbType.Int32,line.VariantId); update.ExecuteNonQuery();
            SalesService.InsertMovement(c,tx,line.VariantId,"ORDER_RESERVE",-line.Quantity,p.Stock-line.Quantity,$"ORDER-{orderId}",actor);
        }
    }

    private static void Release(OracleConnection c, OracleTransaction tx, int orderId, int actor)
    {
        foreach (var line in ReadLines(c, tx, orderId).OrderBy(x => x.VariantId))
        {
            var p = SalesService.LockVariant(c, tx, line.VariantId); var after = checked(p.Stock + line.Quantity);
            using var update = c.CreateCommand(); update.Transaction = tx; update.BindByName = true; update.CommandText = "UPDATE SHOE_VARIANT SET STOCK_QTY=:qty WHERE VARIANT_ID=:id";
            SalesService.Add(update,"qty",OracleDbType.Int32,after); SalesService.Add(update,"id",OracleDbType.Int32,line.VariantId); update.ExecuteNonQuery();
            SalesService.InsertMovement(c,tx,line.VariantId,"ORDER_RELEASE",line.Quantity,after,$"ORDER-{orderId}",actor);
        }
    }

    private static List<(int VariantId, int Quantity, string Sku)> ReadLines(OracleConnection c, OracleTransaction tx, int orderId)
    {
        using var cmd = c.CreateCommand(); cmd.Transaction = tx; cmd.BindByName = true; cmd.CommandText = "SELECT VARIANT_ID,QUANTITY,SKU FROM ONLINE_ORDER_LINE WHERE ORDER_ID=:id ORDER BY VARIANT_ID"; cmd.Parameters.Add("id", OracleDbType.Int32).Value = orderId;
        using var r = cmd.ExecuteReader(); var list = new List<(int,int,string)>(); while (r.Read()) list.Add((Convert.ToInt32(r.GetValue(0)),Convert.ToInt32(r.GetValue(1)),r.GetString(2))); return list;
    }

    private static void AddHistory(OracleConnection c, OracleTransaction tx, int order, string? oldStatus, string status, string note, int actor)
    {
        using var cmd = c.CreateCommand(); cmd.Transaction = tx; cmd.BindByName = true;
        cmd.CommandText = "INSERT INTO ORDER_STATUS_HISTORY(HISTORY_ID,ORDER_ID,OLD_STATUS,NEW_STATUS,NOTE,CHANGED_BY) VALUES(SEQ_ORDER_HISTORY.NEXTVAL,:orderId,:old,:new,:note,:actor)";
        SalesService.Add(cmd,"orderId",OracleDbType.Int32,order); SalesService.Add(cmd,"old",OracleDbType.Varchar2,(object?)oldStatus??DBNull.Value); SalesService.Add(cmd,"new",OracleDbType.Varchar2,status);
        SalesService.Add(cmd,"note",OracleDbType.NVarchar2,string.IsNullOrWhiteSpace(note)?DBNull.Value:note.Trim()); SalesService.Add(cmd,"actor",OracleDbType.Int32,actor); cmd.ExecuteNonQuery();
    }
}



