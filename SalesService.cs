using System.Data;
using Oracle.ManagedDataAccess.Client;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms;

internal sealed record CartLine(int VariantId, int Quantity);

internal sealed class SalesService
{
    private const decimal MaxDocumentTotal = 999_999_999_999.99m;
    public DataTable GetSellableVariants(string keyword = "")
    {
        using var connection = OracleConnectionFactory.Create();
        using var command = connection.CreateCommand(); command.BindByName = true;
        command.CommandText = """
            SELECT v.VARIANT_ID, v.SKU, s.STYLE_NAME || ' - ' || s.BRAND || ' / ' || v.SIZE_CODE || ' / ' || v.COLOR AS ITEM,
                   v.SELL_PRICE, v.STOCK_QTY
            FROM SHOE_VARIANT v JOIN SHOE_STYLE s ON s.STYLE_ID=v.STYLE_ID
            WHERE v.IS_ACTIVE=1 AND s.IS_ACTIVE=1 AND (:q IS NULL OR UPPER(v.SKU) LIKE '%'||UPPER(:q)||'%' OR UPPER(s.STYLE_NAME) LIKE '%'||UPPER(:q)||'%')
            ORDER BY s.STYLE_NAME,v.SIZE_CODE,v.COLOR
            """;
        command.Parameters.Add("q", OracleDbType.NVarchar2, 150).Value = string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim();
        var table = new DataTable(); using var adapter = new OracleDataAdapter(command); adapter.Fill(table); return table;
    }

    public string Checkout(UserSession actor, int? employeeId, IReadOnlyCollection<CartLine> cart)
    {
        actor.Require("CASHIER");
        var actualEmployee = actor.IsManager ? employeeId ?? actor.EmployeeId : actor.EmployeeId;
        if (actualEmployee is null) throw new InvalidOperationException("Tài khoản quản lý cần chọn nhân viên lập hóa đơn.");
        if (cart.Count == 0) throw new ArgumentException("Thêm ít nhất một sản phẩm vào giỏ hàng.");
        var quantities = cart.GroupBy(x => x.VariantId).ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));
        if (quantities.Any(x => x.Value <= 0)) throw new ArgumentException("Số lượng phải lớn hơn 0.");

        using var connection = OracleConnectionFactory.Create(); connection.Open(); using var tx = connection.BeginTransaction();
        try
        {
            var id = NextId(connection, tx, "SEQ_SALES_INVOICE"); var code = $"HD{DateTime.Now:yyyyMMddHHmmss}{id:D6}";
            InsertHeader(connection, tx, id, code, actualEmployee.Value, actor.UserId);
            var total = 0m; var lineNo = 0;
            foreach (var item in quantities.OrderBy(x => x.Key))
            {
                var p = LockVariant(connection, tx, item.Key);
                if (p.Stock < item.Value) throw new InvalidOperationException($"Không đủ tồn kho cho {p.Sku}; hiện còn {p.Stock}.");
                var amount = Math.Round(p.Price * item.Value, 2, MidpointRounding.AwayFromZero);
                if (amount > MaxDocumentTotal || total + amount > MaxDocumentTotal) throw new ArgumentException("Tổng hóa đơn vượt giới hạn lưu trữ Oracle.");
                total += amount; lineNo++;
                using (var update = connection.CreateCommand())
                {
                    update.Transaction = tx; update.BindByName = true;
                    update.CommandText = "UPDATE SHOE_VARIANT SET STOCK_QTY=STOCK_QTY-:q WHERE VARIANT_ID=:id AND STOCK_QTY>=:q";
                    update.Parameters.Add("q", OracleDbType.Int32).Value = item.Value; update.Parameters.Add("id", OracleDbType.Int32).Value = item.Key;
                    if (update.ExecuteNonQuery() != 1) throw new InvalidOperationException("Tồn kho vừa thay đổi; tải lại sản phẩm rồi thử lại.");
                }
                var after = p.Stock - item.Value;
                using (var detail = connection.CreateCommand())
                {
                    detail.Transaction = tx; detail.BindByName = true;
                    detail.CommandText = "INSERT INTO SALES_LINE(INVOICE_ID,LINE_NO,VARIANT_ID,SKU,ITEM_DESCRIPTION,QUANTITY,UNIT_PRICE,UNIT_COST,LINE_TOTAL) VALUES(:invoice,:line,:variant,:sku,:description,:qty,:price,:cost,:total)";
                    Add(detail, "invoice", OracleDbType.Int32, id); Add(detail, "line", OracleDbType.Int32, lineNo); Add(detail, "variant", OracleDbType.Int32, item.Key);
                    Add(detail, "sku", OracleDbType.Varchar2, p.Sku); Add(detail, "description", OracleDbType.NVarchar2, p.Description); Add(detail, "qty", OracleDbType.Int32, item.Value);
                    Add(detail, "price", OracleDbType.Decimal, p.Price); Add(detail, "cost", OracleDbType.Decimal, p.Cost); Add(detail, "total", OracleDbType.Decimal, amount); detail.ExecuteNonQuery();
                }
                InsertMovement(connection, tx, item.Key, "SALE", -item.Value, after, code, actor.UserId);
            }
            using (var updateHeader = connection.CreateCommand())
            {
                updateHeader.Transaction = tx; updateHeader.BindByName = true; updateHeader.CommandText = "UPDATE SALES_INVOICE SET TOTAL_AMOUNT=:total WHERE INVOICE_ID=:id";
                Add(updateHeader, "total", OracleDbType.Decimal, total); Add(updateHeader, "id", OracleDbType.Int32, id); updateHeader.ExecuteNonQuery();
            }
            tx.Commit(); return code;
        }
        catch { tx.Rollback(); throw; }
    }

    internal static int NextId(OracleConnection connection, OracleTransaction tx, string sequence)
    {
        using var command = connection.CreateCommand(); command.Transaction = tx; command.CommandText = $"SELECT {sequence}.NEXTVAL FROM DUAL";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    internal static (string Sku, string Description, int Stock, decimal Price, decimal Cost) LockVariant(OracleConnection c, OracleTransaction tx, int id)
    {
        using var cmd = c.CreateCommand(); cmd.Transaction = tx; cmd.BindByName = true;
        cmd.CommandText = "SELECT v.SKU,s.STYLE_NAME||' - '||s.BRAND||' / '||v.SIZE_CODE||' / '||v.COLOR,v.STOCK_QTY,v.SELL_PRICE,v.COST_PRICE FROM SHOE_VARIANT v JOIN SHOE_STYLE s ON s.STYLE_ID=v.STYLE_ID WHERE v.VARIANT_ID=:id AND v.IS_ACTIVE=1 AND s.IS_ACTIVE=1 FOR UPDATE OF v.STOCK_QTY";
        Add(cmd, "id", OracleDbType.Int32, id); using var reader = cmd.ExecuteReader();
        if (!reader.Read()) throw new InvalidOperationException("Không tìm thấy biến thể giày đang kinh doanh.");
        return (reader.GetString(0), reader.GetString(1), Convert.ToInt32(reader.GetValue(2)), Convert.ToDecimal(reader.GetValue(3)), Convert.ToDecimal(reader.GetValue(4)));
    }

    internal static void InsertMovement(OracleConnection c, OracleTransaction tx, int variantId, string type, int delta, int after, string reference, int actor)
    {
        using var cmd = c.CreateCommand(); cmd.Transaction = tx; cmd.BindByName = true;
        cmd.CommandText = "INSERT INTO STOCK_MOVEMENT(MOVEMENT_ID,VARIANT_ID,MOVEMENT_TYPE,QUANTITY_DELTA,STOCK_AFTER,REFERENCE_CODE,ACTOR_USER_ID) VALUES(SEQ_STOCK_MOVEMENT.NEXTVAL,:variant,:type,:delta,:after,:reference,:actor)";
        Add(cmd, "variant", OracleDbType.Int32, variantId); Add(cmd, "type", OracleDbType.Varchar2, type); Add(cmd, "delta", OracleDbType.Int32, delta);
        Add(cmd, "after", OracleDbType.Int32, after); Add(cmd, "reference", OracleDbType.Varchar2, reference); Add(cmd, "actor", OracleDbType.Int32, actor); cmd.ExecuteNonQuery();
    }

    internal static void Add(OracleCommand cmd, string name, OracleDbType type, object value)
        => cmd.Parameters.Add(name, type).Value = value;

    private static void InsertHeader(OracleConnection c, OracleTransaction tx, int id, string code, int employee, int actor)
    {
        using var cmd = c.CreateCommand(); cmd.Transaction = tx; cmd.BindByName = true;
        cmd.CommandText = "INSERT INTO SALES_INVOICE(INVOICE_ID,INVOICE_CODE,EMPLOYEE_ID,CREATED_BY,TOTAL_AMOUNT) VALUES(:id,:code,:employee,:actor,0)";
        Add(cmd,"id",OracleDbType.Int32,id); Add(cmd,"code",OracleDbType.Varchar2,code); Add(cmd,"employee",OracleDbType.Int32,employee); Add(cmd,"actor",OracleDbType.Int32,actor); cmd.ExecuteNonQuery();
    }
}
