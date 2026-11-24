using System.Data;
using System.Runtime.Versioning;

using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class PosForm : Form
{
    private readonly UserSession _actor = null!; private readonly SalesService _sales = new(); private readonly DataTable _cart = new(); private int? _variantId;
    private readonly ComboBox customerSelect = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 190 };
    public PosForm() { InitializeComponent(); UiTheme.Apply(this); }
    internal PosForm(UserSession actor)
    {
        _actor = actor; actor.Require("MANAGER"); InitializeComponent(); UiTheme.Apply(this);
        quantity.Maximum = 999;
        _cart.Columns.Add("VARIANT_ID", typeof(int)); _cart.Columns.Add("SKU", typeof(string)); _cart.Columns.Add("ITEM", typeof(string)); _cart.Columns.Add("QUANTITY", typeof(int)); _cart.Columns.Add("UNIT_PRICE", typeof(decimal)); _cart.Columns.Add("LINE_TOTAL", typeof(decimal));
        _cart.PrimaryKey = [_cart.Columns["VARIANT_ID"]!];
        employeeId.Value = 0; employeeId.Enabled = actor.IsManager;
        var customerLabel = new Label { Text = "Khách hàng", AutoSize = true, Margin = new Padding(12, 8, 5, 0) };
        customerSelect.Margin = new Padding(0, 4, 10, 0);
        productActions.Controls.Add(customerLabel); productActions.Controls.Add(customerSelect);
        Load += (_, _) => { LoadProducts(); LoadCustomers(); };
    }
    private void search_Click(object? s, EventArgs e) => LoadProducts(searchBox.Text);
    private void reload_Click(object? s, EventArgs e) { searchBox.Clear(); LoadProducts(); }
    private void productsGrid_SelectionChanged(object? s, EventArgs e)
    { if (productsGrid.CurrentRow?.DataBoundItem is DataRowView row) _variantId = Convert.ToInt32(row["VARIANT_ID"]); }
    private void addButton_Click(object? s, EventArgs e)
    {
        if (_variantId is null || productsGrid.CurrentRow?.DataBoundItem is not DataRowView row) { Warn("Chọn sản phẩm trong danh sách."); return; }
        var stock = Convert.ToInt32(row["STOCK_QTY"]); var id = _variantId.Value; var existing = _cart.Rows.Find(id);
        var nextQty = (existing is null ? 0 : Convert.ToInt32(existing["QUANTITY"])) + (int)quantity.Value;
        if (nextQty > stock) { Warn($"Số lượng vượt tồn hiện tại ({stock})."); return; }
        var unitPrice = Convert.ToDecimal(row["SELL_PRICE"]); var amount = unitPrice * nextQty;
        if (existing is null) _cart.Rows.Add(id, row["SKU"], row["ITEM"], nextQty, unitPrice, amount);
        else { existing["QUANTITY"] = nextQty; existing["LINE_TOTAL"] = amount; }
        BindCart();
    }
    private void checkoutButton_Click(object? s, EventArgs e)
    {
        if (_cart.Rows.Count == 0) { Warn("Giỏ hàng đang trống."); return; }
        try
        {
            int? emp = _actor.IsManager && employeeId.Value > 0 ? (int)employeeId.Value : _actor.EmployeeId;
            if (customerSelect.SelectedValue is null) throw new InvalidOperationException("Chọn khách hàng trước khi thanh toán.");
            var code = _sales.Checkout(_actor, emp, Convert.ToInt32(customerSelect.SelectedValue), _cart.Rows.Cast<DataRow>().Select(r => new CartLine((int)r["VARIANT_ID"], (int)r["QUANTITY"])).ToArray());
            _cart.Clear(); BindCart(); LoadProducts(); statusLabel.Text = $"Đã lưu hóa đơn {code}; tồn kho đã cập nhật.";
            using var receipt = new ReceiptForm(code); receipt.ShowDialog(this);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void LoadProducts(string keyword = "")
    {
        try { productsGrid.DataSource = _sales.GetSellableVariants(keyword); Hide(productsGrid,"VARIANT_ID"); Header(productsGrid,"SKU","Mã biến thể"); Header(productsGrid,"ITEM","Sản phẩm / cỡ / màu"); Header(productsGrid,"SELL_PRICE","Giá bán"); Header(productsGrid,"STOCK_QTY","Tồn"); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void LoadCustomers()
    {
        try { customerSelect.DataSource = _sales.GetCustomers(); customerSelect.DisplayMember = "FULL_NAME"; customerSelect.ValueMember = "CUSTOMER_ID"; }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Không tải được khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void BindCart()
    { cartGrid.DataSource = _cart; Hide(cartGrid,"VARIANT_ID"); Header(cartGrid,"SKU","Mã biến thể"); Header(cartGrid,"ITEM","Sản phẩm"); Header(cartGrid,"QUANTITY","SL"); Header(cartGrid,"UNIT_PRICE","Đơn giá"); Header(cartGrid,"LINE_TOTAL","Thành tiền"); totalLabel.Text = "Tổng: " + _cart.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["LINE_TOTAL"])).ToString("N0") + " VNĐ"; }
    private static void Hide(DataGridView g,string column) { if (g.Columns.Contains(column)) g.Columns[column].Visible=false; }
    private static void Header(DataGridView g,string column,string text) { if (g.Columns.Contains(column)) g.Columns[column].HeaderText=text; }
    private static void Warn(string text) => MessageBox.Show(text,"Kiểm tra",MessageBoxButtons.OK,MessageBoxIcon.Warning);
}

internal sealed record CartLine(int VariantId, int Quantity);

internal sealed class SalesService
{
    public DataTable GetSellableVariants(string keyword = "")
    {
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand(); cmd.BindByName = true;
        cmd.CommandText = "SELECT v.VariantID AS VARIANT_ID,TO_CHAR(v.VariantID) AS SKU,p.ProductName||' / '||sz.SizeValue||' / '||cl.ColorName AS ITEM,p.Price AS SELL_PRICE,v.StockQuantity AS STOCK_QTY FROM ProductVariants v JOIN Products p ON p.ProductID=v.ProductID JOIN Sizes sz ON sz.SizeID=v.SizeID JOIN Colors cl ON cl.ColorID=v.ColorID WHERE p.Status='Active' AND (:q IS NULL OR UPPER(p.ProductName) LIKE '%'||UPPER(:q)||'%' OR TO_CHAR(v.VariantID) LIKE '%'||:q||'%') ORDER BY p.ProductName,sz.SizeValue,cl.ColorName";
        cmd.Parameters.Add("q", OracleDbType.NVarchar2, 150).Value = string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim();
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }
    public DataTable GetCustomers()
    { using var c=OracleConnectionFactory.Create(); using var q=c.CreateCommand(); q.CommandText="SELECT CustomerID AS CUSTOMER_ID,FullName AS FULL_NAME FROM Customers ORDER BY FullName"; var d=new DataTable(); using var a=new OracleDataAdapter(q); a.Fill(d); return d; }
    public string Checkout(UserSession actor, int? employeeId, int customerId, IReadOnlyCollection<CartLine> cart)
    {
        actor.Require("MANAGER"); if (cart.Count == 0) throw new ArgumentException("Giỏ hàng đang trống.");
        using var c=OracleConnectionFactory.Create(); c.Open(); using var tx=c.BeginTransaction();
        try {
            if(employeeId.HasValue){using var check=c.CreateCommand();check.Transaction=tx;check.BindByName=true;check.CommandText="SELECT COUNT(*) FROM Employees WHERE EmployeeID=:id";Add(check,"id",OracleDbType.Int32,employeeId.Value);if(Convert.ToInt32(check.ExecuteScalar())==0)throw new InvalidOperationException("Mã nhân viên không có trong Employees; nhập 0 nếu không gắn nhân viên.");}
            Lock(c,tx,"Orders"); Lock(c,tx,"OrderDetails"); Lock(c,tx,"Payments");
            var oid=Next(c,tx,"Orders","OrderID"); var did=Next(c,tx,"OrderDetails","OrderDetailID"); var pid=Next(c,tx,"Payments","PaymentID");
            var lines=new List<(int Id,int Qty,decimal Price)>(); decimal total=0;
            foreach(var x in cart.GroupBy(x=>x.VariantId).Select(g=>(Id:g.Key,Qty:g.Sum(z=>z.Quantity))).OrderBy(x=>x.Id)) {
                if(x.Qty<=0) throw new ArgumentException("Số lượng phải lớn hơn 0.");
                var p=ReadVariant(c,tx,x.Id); if(p.Stock<x.Qty) throw new InvalidOperationException($"Không đủ tồn cho {p.Name}; còn {p.Stock}.");
                total+=p.Price*x.Qty; lines.Add((x.Id,x.Qty,p.Price));
                using var u=c.CreateCommand(); u.Transaction=tx; u.BindByName=true; u.CommandText="UPDATE ProductVariants SET StockQuantity=StockQuantity-:q WHERE VariantID=:id AND StockQuantity>=:q"; Add(u,"q",OracleDbType.Int32,x.Qty); Add(u,"id",OracleDbType.Int32,x.Id); if(u.ExecuteNonQuery()!=1) throw new InvalidOperationException("Tồn kho vừa thay đổi; tải lại danh sách.");
            }
            using(var h=c.CreateCommand()){h.Transaction=tx;h.BindByName=true;h.CommandText="INSERT INTO Orders(OrderID,CustomerID,EmployeeID,OrderDate,Status,TotalAmount) VALUES(:id,:customer,:employee,SYSDATE,'Completed',:total)";Add(h,"id",OracleDbType.Int32,oid);Add(h,"customer",OracleDbType.Int32,customerId);Add(h,"employee",OracleDbType.Int32,(object?)employeeId??DBNull.Value);Add(h,"total",OracleDbType.Decimal,total);h.ExecuteNonQuery();}
            foreach(var l in lines){using var d=c.CreateCommand();d.Transaction=tx;d.BindByName=true;d.CommandText="INSERT INTO OrderDetails(OrderDetailID,OrderID,VariantID,Quantity,UnitPrice) VALUES(:id,:ord,:var,:qty,:price)";Add(d,"id",OracleDbType.Int32,did++);Add(d,"ord",OracleDbType.Int32,oid);Add(d,"var",OracleDbType.Int32,l.Id);Add(d,"qty",OracleDbType.Int32,l.Qty);Add(d,"price",OracleDbType.Decimal,l.Price);d.ExecuteNonQuery();}
            using(var pay=c.CreateCommand()){pay.Transaction=tx;pay.BindByName=true;pay.CommandText="INSERT INTO Payments(PaymentID,OrderID,PaymentMethod,Amount,PaymentDate,Status) VALUES(:id,:ord,'Cash',:amount,SYSDATE,'Paid')";Add(pay,"id",OracleDbType.Int32,pid);Add(pay,"ord",OracleDbType.Int32,oid);Add(pay,"amount",OracleDbType.Decimal,total);pay.ExecuteNonQuery();}
            tx.Commit();return oid.ToString();
        } catch {tx.Rollback();throw;}
    }
    private static (string Name,int Stock,decimal Price) ReadVariant(OracleConnection c,OracleTransaction tx,int id){using var q=c.CreateCommand();q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT p.ProductName||' / '||sz.SizeValue||' / '||cl.ColorName,v.StockQuantity,p.Price FROM ProductVariants v JOIN Products p ON p.ProductID=v.ProductID JOIN Sizes sz ON sz.SizeID=v.SizeID JOIN Colors cl ON cl.ColorID=v.ColorID WHERE v.VariantID=:id AND p.Status='Active' FOR UPDATE OF v.StockQuantity";Add(q,"id",OracleDbType.Int32,id);using var r=q.ExecuteReader();if(!r.Read())throw new InvalidOperationException("Không tìm thấy biến thể đang bán.");return(r.GetString(0),Convert.ToInt32(r.GetValue(1)),Convert.ToDecimal(r.GetValue(2)));}
    internal static void Lock(OracleConnection c,OracleTransaction tx,string t){if(t is not("Orders" or "OrderDetails" or "Payments" or "Products" or "ProductVariants" or "Brands" or "Categories" or "Colors" or "Sizes" or "Customers" or "Employees" or "OrderHistory"))throw new ArgumentOutOfRangeException(nameof(t));using var q=c.CreateCommand();q.Transaction=tx;q.CommandText=$"LOCK TABLE {t} IN EXCLUSIVE MODE";q.ExecuteNonQuery();}
    private static int Next(OracleConnection c,OracleTransaction tx,string t,string col){Lock(c,tx,t);using var q=c.CreateCommand();q.Transaction=tx;q.CommandText=$"SELECT NVL(MAX({col}),0)+1 FROM {t}";return Convert.ToInt32(q.ExecuteScalar());}
    internal static void Add(OracleCommand c,string n,OracleDbType t,object v)=>c.Parameters.Add(n,t).Value=v;
}
