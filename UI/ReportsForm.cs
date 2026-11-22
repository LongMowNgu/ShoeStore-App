using System.Data;
using System.Runtime.Versioning;
using System.Text;

using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class ReportsForm:Form
{
    private readonly ReportRepository _reports=new();
    public ReportsForm() { InitializeComponent(); UiTheme.Apply(this); }
    internal ReportsForm(UserSession actor){actor.Require("MANAGER");InitializeComponent(); UiTheme.Apply(this);fromDate.Value=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);toDate.Value=DateTime.Today;Load+=(_,_)=>LoadReport();}
    private void load_Click(object? s,EventArgs e)=>LoadReport();
    private void export_Click(object? s,EventArgs e)
    {
        using var dlg=new SaveFileDialog{Filter="CSV UTF-8|*.csv",FileName=$"BaoCaoGiay_{DateTime.Today:yyyyMMdd}.csv"};if(dlg.ShowDialog(this)!=DialogResult.OK)return;
        try{var table=salesGrid.DataSource as DataTable??new DataTable();var sb=new StringBuilder();sb.AppendLine(string.Join(";",table.Columns.Cast<DataColumn>().Select(x=>Escape(x.ColumnName))));foreach(DataRow r in table.Rows)sb.AppendLine(string.Join(";",table.Columns.Cast<DataColumn>().Select(c=>Escape(Convert.ToString(r[c])))));File.WriteAllText(dlg.FileName,"\uFEFF"+sb,Encoding.UTF8);MessageBox.Show("Đã xuất báo cáo doanh thu.","Xuất CSV",MessageBoxButtons.OK,MessageBoxIcon.Information);}catch(Exception ex){MessageBox.Show(ex.Message,"Không xuất được",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private void LoadReport()
    {
        try
        {
            var sales=_reports.SalesSummary(fromDate.Value,toDate.Value);var best=_reports.BestSellers(fromDate.Value,toDate.Value);var low=_reports.LowStock();var orders=_reports.OrdersByStatus();
            salesGrid.DataSource=sales;sellersGrid.DataSource=best;lowStockGrid.DataSource=low;orderStatusGrid.DataSource=orders;
            if (salesGrid.Columns.Contains("COST")) salesGrid.Columns["COST"].Visible = false;
            if (lowStockGrid.Columns.Contains("REORDER_LEVEL")) lowStockGrid.Columns["REORDER_LEVEL"].Visible = false;
            Headers(salesGrid,("SALE_DATE","Ngày"),("INVOICE_COUNT","Số hóa đơn"),("REVENUE","Doanh thu"),("COST","Giá vốn"));Headers(sellersGrid,("SKU","Mã biến thể"),("ITEM","Sản phẩm"),("SOLD","Số lượng bán"),("REVENUE","Doanh thu"));Headers(lowStockGrid,("SKU","Mã biến thể"),("ITEM","Sản phẩm"),("STOCK_QTY","Tồn"),("REORDER_LEVEL","Ngưỡng"));Headers(orderStatusGrid,("ORDER_STATUS","Trạng thái"),("ORDER_COUNT","Số đơn"),("ORDER_TOTAL","Tổng tiền"));
            var revenue=sales.Rows.Cast<DataRow>().Sum(r=>Convert.ToDecimal(r["REVENUE"]));summaryLabel.Text=$"{fromDate.Value:dd/MM/yyyy} – {toDate.Value:dd/MM/yyyy}   |   Doanh thu đơn hoàn tất {revenue:N0} VNĐ   |   Biến thể hết hàng: {low.Rows.Count}";
        }
        catch(Exception ex){MessageBox.Show(ex.Message,"Không tạo được báo cáo",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private static void Headers(DataGridView g,params (string Key,string Caption)[] items){foreach(var (key,caption) in items)if(g.Columns.Contains(key))g.Columns[key].HeaderText=caption;}
    private static string Escape(string? s){s??="";if(s.StartsWith('=')||s.StartsWith('+')||s.StartsWith('-')||s.StartsWith('@'))s="'"+s;return "\""+s.Replace("\"","\"\"")+"\"";}
}

internal sealed class ReportRepository
{
    public DataTable SalesSummary(DateTime from, DateTime to)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT TRUNC(o.OrderDate) AS SALE_DATE,COUNT(*) AS INVOICE_COUNT,SUM(o.TotalAmount) AS REVENUE,0 AS COST FROM Orders o WHERE o.Status='Completed' AND o.OrderDate>=:a AND o.OrderDate<:b GROUP BY TRUNC(o.OrderDate) ORDER BY SALE_DATE";
        DateRange(cmd, from, to); var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable BestSellers(DateTime from, DateTime to)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT TO_CHAR(v.VariantID) AS SKU,MAX(p.ProductName||' / '||sz.SizeValue||' / '||cl.ColorName) AS ITEM,SUM(d.Quantity) AS SOLD,SUM(d.Quantity*d.UnitPrice) AS REVENUE FROM OrderDetails d JOIN Orders o ON o.OrderID=d.OrderID JOIN ProductVariants v ON v.VariantID=d.VariantID JOIN Products p ON p.ProductID=v.ProductID JOIN Sizes sz ON sz.SizeID=v.SizeID JOIN Colors cl ON cl.ColorID=v.ColorID WHERE o.Status='Completed' AND o.OrderDate>=:a AND o.OrderDate<:b GROUP BY v.VariantID ORDER BY SOLD DESC,SKU FETCH FIRST 20 ROWS ONLY";
        DateRange(cmd, from, to); var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable LowStock()
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT TO_CHAR(v.VariantID) AS SKU,p.ProductName||' / '||sz.SizeValue||' / '||cl.ColorName AS ITEM,v.StockQuantity AS STOCK_QTY,0 AS REORDER_LEVEL FROM ProductVariants v JOIN Products p ON p.ProductID=v.ProductID JOIN Sizes sz ON sz.SizeID=v.SizeID JOIN Colors cl ON cl.ColorID=v.ColorID WHERE p.Status='Active' AND v.StockQuantity=0 ORDER BY p.ProductName";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable OrdersByStatus()
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT Status AS ORDER_STATUS,COUNT(*) AS ORDER_COUNT,SUM(TotalAmount) AS ORDER_TOTAL FROM Orders GROUP BY Status ORDER BY Status";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    private static void DateRange(OracleCommand cmd, DateTime from, DateTime to)
    {
        if (from.Date > to.Date) throw new ArgumentException("Ngày bắt đầu không được sau ngày kết thúc.");
        cmd.Parameters.Add("a", OracleDbType.TimeStamp).Value = from.Date;
        cmd.Parameters.Add("b", OracleDbType.TimeStamp).Value = to.Date.AddDays(1);
    }
}


