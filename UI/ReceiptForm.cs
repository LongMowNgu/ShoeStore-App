using System.Data;
using System.Drawing.Printing;
using System.Runtime.Versioning;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class ReceiptForm : Form
{
    private readonly string _code; private readonly InvoiceRepository _repo = new(); private DataTable _lines = new(); private readonly PrintDocument _print = new();
    public ReceiptForm(string code)
    {
        _code=code; InitializeComponent(); _print.PrintPage += PrintPage; Load += (_,_)=>LoadInvoice();
    }
    private void LoadInvoice()
    {
        try
        {
            var h=_repo.FindInvoice(_code); if(h.Rows.Count==0) throw new InvalidOperationException("Không tìm thấy hóa đơn.");
            var r=h.Rows[0]; headerLabel.Text=$"CỬA HÀNG GIÀY\nHóa đơn: {r["INVOICE_CODE"]}    Ngày: {Convert.ToDateTime(r["CREATED_AT"]):dd/MM/yyyy HH:mm}\nNhân viên: {r["FULL_NAME"]}    Tổng tiền: {Convert.ToDecimal(r["TOTAL_AMOUNT"]):N0} VNĐ";
            _lines=_repo.GetLines(Convert.ToInt32(r["INVOICE_ID"])); linesGrid.DataSource=_lines;
            var labels=new[]{("SKU","Mã SKU"),("ITEM_DESCRIPTION","Sản phẩm"),("QUANTITY","SL"),("UNIT_PRICE","Đơn giá"),("LINE_TOTAL","Thành tiền")}; foreach(var (key,label) in labels) if(linesGrid.Columns.Contains(key)) linesGrid.Columns[key].HeaderText=label;
        }
        catch(Exception ex){MessageBox.Show(ex.Message,"Lỗi hóa đơn",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private void printButton_Click(object? sender,EventArgs e)
    { using var preview=new PrintPreviewDialog{Document=_print,Width=900,Height=700}; preview.ShowDialog(this); }
    private void PrintPage(object? sender,PrintPageEventArgs e)
    {
        var g=e.Graphics!; using var title=new Font("Segoe UI",16,FontStyle.Bold); using var body=new Font("Segoe UI",9); using var bold=new Font("Segoe UI",10,FontStyle.Bold);
        float y=45; g.DrawString("CỬA HÀNG GIÀY",title,Brushes.Black,50,y); y+=36; g.DrawString("HÓA ĐƠN: "+_code,bold,Brushes.Black,50,y); y+=26;
        g.DrawString("SKU / SẢN PHẨM",bold,Brushes.Black,50,y); g.DrawString("SL",bold,Brushes.Black,430,y); g.DrawString("ĐƠN GIÁ",bold,Brushes.Black,480,y); g.DrawString("THÀNH TIỀN",bold,Brushes.Black,600,y); y+=24;
        foreach(DataRow r in _lines.Rows){g.DrawString(Convert.ToString(r["SKU"])+"  "+Convert.ToString(r["ITEM_DESCRIPTION"]),body,Brushes.Black,50,y,new StringFormat{Trimming=StringTrimming.EllipsisCharacter});g.DrawString(Convert.ToString(r["QUANTITY"]),body,Brushes.Black,430,y);g.DrawString(Convert.ToDecimal(r["UNIT_PRICE"]).ToString("N0"),body,Brushes.Black,480,y);g.DrawString(Convert.ToDecimal(r["LINE_TOTAL"]).ToString("N0"),body,Brushes.Black,600,y);y+=24;}
        var h=_repo.FindInvoice(_code); if(h.Rows.Count>0){y+=12;g.DrawString("TỔNG CỘNG: "+Convert.ToDecimal(h.Rows[0]["TOTAL_AMOUNT"]).ToString("N0")+" VNĐ",bold,Brushes.Black,480,y);} e.HasMorePages=false;
    }
}
