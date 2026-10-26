using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ShoeStore.WinForms;

internal sealed class ReportRepository
{
    public DataTable SalesSummary(DateTime from, DateTime to)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT TRUNC(CREATED_AT) AS SALE_DATE,COUNT(*) AS INVOICE_COUNT,SUM(TOTAL_AMOUNT) AS REVENUE,SUM((SELECT NVL(SUM(sl.QUANTITY*sl.UNIT_COST),0) FROM SALES_LINE sl WHERE sl.INVOICE_ID=si.INVOICE_ID)) AS COST FROM SALES_INVOICE si WHERE CREATED_AT>=:a AND CREATED_AT<:b GROUP BY TRUNC(CREATED_AT) ORDER BY SALE_DATE";
        DateRange(cmd, from, to); var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable BestSellers(DateTime from, DateTime to)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT SKU,MAX(ITEM_DESCRIPTION) AS ITEM,SUM(QUANTITY) AS SOLD,SUM(LINE_TOTAL) AS REVENUE FROM SALES_LINE sl JOIN SALES_INVOICE si ON si.INVOICE_ID=sl.INVOICE_ID WHERE si.CREATED_AT>=:a AND si.CREATED_AT<:b GROUP BY SKU ORDER BY SOLD DESC,SKU FETCH FIRST 20 ROWS ONLY";
        DateRange(cmd, from, to); var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable LowStock()
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT v.SKU,s.STYLE_NAME||' / '||v.SIZE_CODE||' / '||v.COLOR AS ITEM,v.STOCK_QTY,v.REORDER_LEVEL FROM SHOE_VARIANT v JOIN SHOE_STYLE s ON s.STYLE_ID=v.STYLE_ID WHERE v.IS_ACTIVE=1 AND v.STOCK_QTY<=v.REORDER_LEVEL ORDER BY v.STOCK_QTY,s.STYLE_NAME";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable OrdersByStatus()
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT ORDER_STATUS,COUNT(*) AS ORDER_COUNT,SUM(TOTAL_AMOUNT) AS ORDER_TOTAL FROM ONLINE_ORDER GROUP BY ORDER_STATUS ORDER BY ORDER_STATUS";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    private static void DateRange(OracleCommand cmd, DateTime from, DateTime to)
    {
        if (from.Date > to.Date) throw new ArgumentException("Ngày bắt đầu không được sau ngày kết thúc.");
        cmd.Parameters.Add("a", OracleDbType.TimeStamp).Value = from.Date;
        cmd.Parameters.Add("b", OracleDbType.TimeStamp).Value = to.Date.AddDays(1);
    }
}
