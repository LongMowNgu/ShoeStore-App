using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ShoeStore.WinForms;

internal sealed class InvoiceRepository
{
    public DataTable FindInvoice(string code)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT i.INVOICE_ID,i.INVOICE_CODE,i.CREATED_AT,i.TOTAL_AMOUNT,e.FULL_NAME,u.USERNAME FROM SALES_INVOICE i LEFT JOIN EMPLOYEE e ON e.EMPLOYEE_ID=i.EMPLOYEE_ID JOIN APP_USER u ON u.USER_ID=i.CREATED_BY WHERE i.INVOICE_CODE=:code";
        cmd.Parameters.Add("code", OracleDbType.Varchar2, 40).Value = code; var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable GetLines(int invoiceId)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT SKU,ITEM_DESCRIPTION,QUANTITY,UNIT_PRICE,LINE_TOTAL FROM SALES_LINE WHERE INVOICE_ID=:id ORDER BY LINE_NO";
        cmd.Parameters.Add("id", OracleDbType.Int32).Value = invoiceId; var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable Search(DateTime from, DateTime to, string text)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT INVOICE_ID,INVOICE_CODE,CREATED_AT,TOTAL_AMOUNT FROM SALES_INVOICE WHERE CREATED_AT>=:fromDate AND CREATED_AT<:toDate AND (:term IS NULL OR UPPER(INVOICE_CODE) LIKE '%'||UPPER(:term)||'%') ORDER BY CREATED_AT DESC";
        cmd.Parameters.Add("fromDate", OracleDbType.TimeStamp).Value = from.Date;
        cmd.Parameters.Add("toDate", OracleDbType.TimeStamp).Value = to.Date.AddDays(1);
        cmd.Parameters.Add("term", OracleDbType.Varchar2, 40).Value = string.IsNullOrWhiteSpace(text) ? DBNull.Value : text.Trim();
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }
}
