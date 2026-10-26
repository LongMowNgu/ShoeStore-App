using System.Data;
using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class PosForm : Form
{
    private readonly UserSession _actor; private readonly SalesService _sales = new(); private readonly DataTable _cart = new(); private int? _variantId;
    public PosForm(UserSession actor)
    {
        _actor = actor; actor.Require("CASHIER"); InitializeComponent();
        _cart.Columns.Add("VARIANT_ID", typeof(int)); _cart.Columns.Add("SKU", typeof(string)); _cart.Columns.Add("ITEM", typeof(string)); _cart.Columns.Add("QUANTITY", typeof(int)); _cart.Columns.Add("UNIT_PRICE", typeof(decimal)); _cart.Columns.Add("LINE_TOTAL", typeof(decimal));
        _cart.PrimaryKey = [_cart.Columns["VARIANT_ID"]!];
        employeeId.Value = actor.EmployeeId ?? 0; employeeId.Enabled = actor.IsManager; Load += (_, _) => LoadProducts();
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
            var code = _sales.Checkout(_actor, emp, _cart.Rows.Cast<DataRow>().Select(r => new CartLine((int)r["VARIANT_ID"], (int)r["QUANTITY"])).ToArray());
            _cart.Clear(); BindCart(); LoadProducts(); statusLabel.Text = $"Đã lưu hóa đơn {code}; tồn kho đã cập nhật.";
            using var receipt = new ReceiptForm(code); receipt.ShowDialog(this);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void LoadProducts(string keyword = "")
    {
        try { productsGrid.DataSource = _sales.GetSellableVariants(keyword); Hide(productsGrid,"VARIANT_ID"); Header(productsGrid,"SKU","Mã SKU"); Header(productsGrid,"ITEM","Sản phẩm / cỡ / màu"); Header(productsGrid,"SELL_PRICE","Giá bán"); Header(productsGrid,"STOCK_QTY","Tồn"); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void BindCart()
    { cartGrid.DataSource = _cart; Hide(cartGrid,"VARIANT_ID"); Header(cartGrid,"SKU","SKU"); Header(cartGrid,"ITEM","Sản phẩm"); Header(cartGrid,"QUANTITY","SL"); Header(cartGrid,"UNIT_PRICE","Đơn giá"); Header(cartGrid,"LINE_TOTAL","Thành tiền"); totalLabel.Text = "Tổng: " + _cart.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["LINE_TOTAL"])).ToString("N0") + " VNĐ"; }
    private static void Hide(DataGridView g,string column) { if (g.Columns.Contains(column)) g.Columns[column].Visible=false; }
    private static void Header(DataGridView g,string column,string text) { if (g.Columns.Contains(column)) g.Columns[column].HeaderText=text; }
    private static void Warn(string text) => MessageBox.Show(text,"Kiểm tra",MessageBoxButtons.OK,MessageBoxIcon.Warning);
}
