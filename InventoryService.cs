using System.Data;
using Oracle.ManagedDataAccess.Client;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms;

internal sealed class InventoryService
{
    public DataTable LoadStock(string keyword = "")
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT v.VARIANT_ID,v.SKU,s.STYLE_NAME||' - '||s.BRAND||' / '||v.SIZE_CODE||' / '||v.COLOR AS ITEM,v.STOCK_QTY,v.REORDER_LEVEL,v.COST_PRICE,v.SELL_PRICE,CASE WHEN v.STOCK_QTY<=v.REORDER_LEVEL THEN 'LOW' ELSE 'OK' END AS STOCK_ALERT FROM SHOE_VARIANT v JOIN SHOE_STYLE s ON s.STYLE_ID=v.STYLE_ID WHERE v.IS_ACTIVE=1 AND s.IS_ACTIVE=1 AND (:q IS NULL OR UPPER(v.SKU) LIKE '%'||UPPER(:q)||'%' OR UPPER(s.STYLE_NAME) LIKE '%'||UPPER(:q)||'%') ORDER BY CASE WHEN v.STOCK_QTY<=v.REORDER_LEVEL THEN 0 ELSE 1 END,v.STOCK_QTY,s.STYLE_NAME";
        cmd.Parameters.Add("q", OracleDbType.NVarchar2, 150).Value = string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim();
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public string Receive(UserSession actor, int? employeeId, int variantId, int quantity, decimal unitCost, string supplier)
    {
        actor.Require("WAREHOUSE");
        var employee = actor.IsManager ? employeeId ?? actor.EmployeeId : actor.EmployeeId;
        if (employee is null) throw new InvalidOperationException("Tài khoản quản lý cần chọn nhân viên lập phiếu nhập.");
        if (quantity <= 0 || unitCost < 0) throw new ArgumentException("Số lượng phải lớn hơn 0; giá nhập không được âm.");
        using var c = OracleConnectionFactory.Create(); c.Open(); using var tx = c.BeginTransaction();
        try
        {
            var p = SalesService.LockVariant(c, tx, variantId); var id = SalesService.NextId(c, tx, "SEQ_PURCHASE_RECEIPT");
            var code = $"PN{DateTime.Now:yyyyMMddHHmmss}{id:D6}"; var total = quantity * unitCost; var after = checked(p.Stock + quantity);
            if (total > 999_999_999_999.99m) throw new ArgumentException("Tổng phiếu nhập vượt giới hạn lưu trữ Oracle.");
            using (var header = c.CreateCommand())
            {
                header.Transaction = tx; header.BindByName = true; header.CommandText = "INSERT INTO PURCHASE_RECEIPT(RECEIPT_ID,RECEIPT_CODE,EMPLOYEE_ID,CREATED_BY,SUPPLIER_NAME,TOTAL_AMOUNT) VALUES(:id,:code,:employee,:actor,:supplier,:total)";
                SalesService.Add(header,"id",OracleDbType.Int32,id); SalesService.Add(header,"code",OracleDbType.Varchar2,code); SalesService.Add(header,"employee",OracleDbType.Int32,employee.Value); SalesService.Add(header,"actor",OracleDbType.Int32,actor.UserId);
                SalesService.Add(header,"supplier",OracleDbType.NVarchar2,string.IsNullOrWhiteSpace(supplier)?DBNull.Value:supplier.Trim()); SalesService.Add(header,"total",OracleDbType.Decimal,total); header.ExecuteNonQuery();
            }
            using (var detail = c.CreateCommand())
            {
                detail.Transaction = tx; detail.BindByName = true; detail.CommandText = "INSERT INTO PURCHASE_LINE(RECEIPT_ID,LINE_NO,VARIANT_ID,SKU,ITEM_DESCRIPTION,QUANTITY,UNIT_COST,LINE_TOTAL) VALUES(:id,1,:variant,:sku,:description,:qty,:cost,:total)";
                SalesService.Add(detail,"id",OracleDbType.Int32,id); SalesService.Add(detail,"variant",OracleDbType.Int32,variantId); SalesService.Add(detail,"sku",OracleDbType.Varchar2,p.Sku); SalesService.Add(detail,"description",OracleDbType.NVarchar2,p.Description);
                SalesService.Add(detail,"qty",OracleDbType.Int32,quantity); SalesService.Add(detail,"cost",OracleDbType.Decimal,unitCost); SalesService.Add(detail,"total",OracleDbType.Decimal,total); detail.ExecuteNonQuery();
            }
            using (var update = c.CreateCommand())
            {
                update.Transaction = tx; update.BindByName = true; update.CommandText = "UPDATE SHOE_VARIANT SET STOCK_QTY=:stock,COST_PRICE=:cost WHERE VARIANT_ID=:id";
                SalesService.Add(update,"stock",OracleDbType.Int32,after); SalesService.Add(update,"cost",OracleDbType.Decimal,unitCost); SalesService.Add(update,"id",OracleDbType.Int32,variantId); update.ExecuteNonQuery();
            }
            SalesService.InsertMovement(c,tx,variantId,"RECEIVE",quantity,after,code,actor.UserId); tx.Commit(); return code;
        }
        catch { tx.Rollback(); throw; }
    }

    public DataTable Movements(int take = 300)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT * FROM (SELECT m.MOVED_AT,m.MOVEMENT_TYPE,v.SKU,m.QUANTITY_DELTA,m.STOCK_AFTER,m.REFERENCE_CODE,u.USERNAME FROM STOCK_MOVEMENT m JOIN SHOE_VARIANT v ON v.VARIANT_ID=m.VARIANT_ID JOIN APP_USER u ON u.USER_ID=m.ACTOR_USER_ID ORDER BY m.MOVEMENT_ID DESC) WHERE ROWNUM<=:take";
        cmd.Parameters.Add("take", OracleDbType.Int32).Value = take; var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }
}
