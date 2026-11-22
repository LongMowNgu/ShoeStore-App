using System.Data;
using System.Drawing.Printing;
using System.Runtime.Versioning;

using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class ReceiptForm : Form
{
    private readonly string _code = string.Empty; private readonly InvoiceRepository _repo = new(); private DataTable _lines = new(); private readonly PrintDocument _print = new();
    private int _printLineIndex;
    private string _printTotal = "";
    public ReceiptForm() { InitializeComponent(); UiTheme.Apply(this); }
    internal ReceiptForm(string code)
    {
        _code=code; InitializeComponent(); UiTheme.Apply(this); _print.PrintPage += PrintPage; Load += (_,_)=>LoadInvoice();
    }
    private void closeButton_Click(object? sender, EventArgs e) => Close();

    private void LoadInvoice()
    {
        try
        {
            var h=_repo.FindInvoice(_code); if(h.Rows.Count==0) throw new InvalidOperationException("Không tìm thấy đơn hàng.");
            var r=h.Rows[0]; headerLabel.Text=$"CỬA HÀNG GIÀY\nMã đơn: {r["INVOICE_CODE"]}    Ngày: {Convert.ToDateTime(r["CREATED_AT"]):dd/MM/yyyy HH:mm}\nKhách hàng: {r["FULL_NAME"]}    Tổng tiền: {Convert.ToDecimal(r["TOTAL_AMOUNT"]):N0} VNĐ";
            _printTotal = "TỔNG CỘNG: " + Convert.ToDecimal(r["TOTAL_AMOUNT"]).ToString("N0") + " VNĐ";
            _lines=_repo.GetLines(Convert.ToInt32(r["INVOICE_ID"])); linesGrid.DataSource=_lines;
            var labels=new[]{("SKU","Mã biến thể"),("ITEM_DESCRIPTION","Sản phẩm"),("QUANTITY","SL"),("UNIT_PRICE","Đơn giá"),("LINE_TOTAL","Thành tiền")}; foreach(var (key,label) in labels) if(linesGrid.Columns.Contains(key)) linesGrid.Columns[key].HeaderText=label;
        }
        catch(Exception ex){MessageBox.Show(ex.Message,"Lỗi hóa đơn",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private void printButton_Click(object? sender,EventArgs e)
    { _printLineIndex=0; using var preview=new PrintPreviewDialog{Document=_print,Width=900,Height=700}; preview.ShowDialog(this); }
    private void PrintPage(object? sender,PrintPageEventArgs e)
    {
        var g=e.Graphics!; var bounds=e.MarginBounds; using var title=new Font("Segoe UI",16,FontStyle.Bold); using var body=new Font("Segoe UI",9); using var bold=new Font("Segoe UI",10,FontStyle.Bold);
        var y=(float)bounds.Top; var left=(float)bounds.Left; var right=(float)bounds.Right;
        g.DrawString("CỬA HÀNG GIÀY",title,Brushes.Black,left,y); y+=34;
        g.DrawString("HÓA ĐƠN: "+_code,bold,Brushes.Black,left,y); y+=24;
        g.DrawString("MÃ BIẾN THỂ / SẢN PHẨM",bold,Brushes.Black,left,y); g.DrawString("SL",bold,Brushes.Black,right-300,y); g.DrawString("ĐƠN GIÁ",bold,Brushes.Black,right-230,y); g.DrawString("THÀNH TIỀN",bold,Brushes.Black,right-135,y); y+=24;
        var lineHeight=Math.Max(body.GetHeight(g)+7,20f); var descriptionWidth=Math.Max(100f,bounds.Width-310f);
        using var format=new StringFormat { Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap };
        while(_printLineIndex<_lines.Rows.Count && y+lineHeight+30<bounds.Bottom)
        {
            var row=_lines.Rows[_printLineIndex++]; var description=Convert.ToString(row["SKU"])+"  "+Convert.ToString(row["ITEM_DESCRIPTION"]);
            g.DrawString(description,body,Brushes.Black,new RectangleF(left,y,descriptionWidth,lineHeight),format);
            g.DrawString(Convert.ToString(row["QUANTITY"]),body,Brushes.Black,right-300,y);
            g.DrawString(Convert.ToDecimal(row["UNIT_PRICE"]).ToString("N0"),body,Brushes.Black,right-230,y);
            g.DrawString(Convert.ToDecimal(row["LINE_TOTAL"]).ToString("N0"),body,Brushes.Black,right-135,y); y+=lineHeight;
        }
        if(_printLineIndex<_lines.Rows.Count){e.HasMorePages=true;return;}
        y+=12;g.DrawString(_printTotal,bold,Brushes.Black,Math.Max(left,right-260),y);e.HasMorePages=false;
    }
}

internal sealed class InvoiceRepository
{
    public DataTable FindInvoice(string code)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT o.OrderID AS INVOICE_ID,TO_CHAR(o.OrderID) AS INVOICE_CODE,o.OrderDate AS CREATED_AT,o.TotalAmount AS TOTAL_AMOUNT,c.FullName FROM Orders o JOIN Customers c ON c.CustomerID=o.CustomerID WHERE o.OrderID=:id";
        cmd.Parameters.Add("id", OracleDbType.Int32).Value = int.Parse(code); var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable GetLines(int invoiceId)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT TO_CHAR(v.VariantID) AS SKU,p.ProductName||' / '||sz.SizeValue||' / '||cl.ColorName AS ITEM_DESCRIPTION,d.Quantity,d.UnitPrice AS UNIT_PRICE,d.Quantity*d.UnitPrice AS LINE_TOTAL FROM OrderDetails d JOIN ProductVariants v ON v.VariantID=d.VariantID JOIN Products p ON p.ProductID=v.ProductID JOIN Sizes sz ON sz.SizeID=v.SizeID JOIN Colors cl ON cl.ColorID=v.ColorID WHERE d.OrderID=:id ORDER BY d.OrderDetailID";
        cmd.Parameters.Add("id", OracleDbType.Int32).Value = invoiceId; var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable Search(DateTime from, DateTime to, string text)
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT OrderID AS INVOICE_ID,TO_CHAR(OrderID) AS INVOICE_CODE,OrderDate AS CREATED_AT,TotalAmount AS TOTAL_AMOUNT FROM Orders WHERE OrderDate>=:fromDate AND OrderDate<:toDate AND (:term IS NULL OR TO_CHAR(OrderID) LIKE '%'||:term||'%') ORDER BY OrderDate DESC";
        cmd.Parameters.Add("fromDate", OracleDbType.TimeStamp).Value = from.Date;
        cmd.Parameters.Add("toDate", OracleDbType.TimeStamp).Value = to.Date.AddDays(1);
        cmd.Parameters.Add("term", OracleDbType.Varchar2, 40).Value = string.IsNullOrWhiteSpace(text) ? DBNull.Value : text.Trim();
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }
}


