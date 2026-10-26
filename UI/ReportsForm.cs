using System.Data;
using System.Runtime.Versioning;
using System.Text;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class ReportsForm:Form
{
    private readonly ReportRepository _reports=new();
    public ReportsForm(UserSession actor){actor.Require("MANAGER");InitializeComponent();fromDate.Value=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);toDate.Value=DateTime.Today;Load+=(_,_)=>LoadReport();}
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
            Headers(salesGrid,("SALE_DATE","Ngày"),("INVOICE_COUNT","Số hóa đơn"),("REVENUE","Doanh thu"),("COST","Giá vốn"));Headers(sellersGrid,("SKU","SKU"),("ITEM","Sản phẩm"),("SOLD","Số lượng bán"),("REVENUE","Doanh thu"));Headers(lowStockGrid,("SKU","SKU"),("ITEM","Sản phẩm"),("STOCK_QTY","Tồn"),("REORDER_LEVEL","Ngưỡng"));Headers(orderStatusGrid,("ORDER_STATUS","Trạng thái"),("ORDER_COUNT","Số đơn"),("ORDER_TOTAL","Tổng tiền"));
            var revenue=sales.Rows.Cast<DataRow>().Sum(r=>Convert.ToDecimal(r["REVENUE"]));var cost=sales.Rows.Cast<DataRow>().Sum(r=>Convert.ToDecimal(r["COST"]));summaryLabel.Text=$"{fromDate.Value:dd/MM/yyyy} – {toDate.Value:dd/MM/yyyy}   |   Doanh thu {revenue:N0} VNĐ   |   Giá vốn {cost:N0} VNĐ   |   Lãi gộp {(revenue-cost):N0} VNĐ   |   SKU sắp hết: {low.Rows.Count}";
        }
        catch(Exception ex){MessageBox.Show(ex.Message,"Không tạo được báo cáo",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private static void Headers(DataGridView g,params (string Key,string Caption)[] items){foreach(var (key,caption) in items)if(g.Columns.Contains(key))g.Columns[key].HeaderText=caption;}
    private static string Escape(string? s){s??="";if(s.StartsWith('=')||s.StartsWith('+')||s.StartsWith('-')||s.StartsWith('@'))s="'"+s;return "\""+s.Replace("\"","\"\"")+"\"";}
}
