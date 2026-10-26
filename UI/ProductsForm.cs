using System.Data;
using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class ProductsForm : Form
{
    private readonly ProductRepository _products = new();
    private readonly UserSession _session;
    private int? _selectedVariantId;
    private int? _selectedStyleId;

    public ProductsForm(UserSession session)
    {
        _session = session;
        _session.Require("WAREHOUSE");
        InitializeComponent();
        Load += (_, _) => Reload();
    }

    private void searchButton_Click(object? sender, EventArgs e) => Reload(searchBox.Text);
    private void reloadButton_Click(object? sender, EventArgs e) { searchBox.Clear(); Reload(); }

    private void addButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        Run(() => _products.Add(styleName.Text, brand.Text, category.Text, sku.Text, sizeCode.Text, color.Text, price.Value, (int)reorder.Value), "Đã thêm mẫu giày và biến thể. Tồn kho ban đầu bằng 0.");
    }

    private void addVariantButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateVariantInput()) return;
        if (_selectedStyleId is null) { Warn("Chọn một biến thể thuộc mẫu giày cần thêm cỡ/màu."); return; }
        var selectedStyleId = _selectedStyleId.Value;
        Run(() => _products.AddVariant(selectedStyleId, sku.Text, sizeCode.Text, color.Text, price.Value, (int)reorder.Value), "Đã thêm biến thể. Tồn kho ban đầu bằng 0.");
    }

    private void updateButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        if (_selectedVariantId is null || _selectedStyleId is null) { Warn("Chọn một biến thể trong danh sách trước khi cập nhật."); return; }
        Run(() => _products.Update(_selectedVariantId.Value, _selectedStyleId.Value, styleName.Text, brand.Text, category.Text, sku.Text, sizeCode.Text, color.Text, price.Value, (int)reorder.Value), "Đã cập nhật sản phẩm.");
    }

    private void deactivateButton_Click(object? sender, EventArgs e)
    {
        if (_selectedVariantId is null) { Warn("Chọn biến thể cần ngừng bán."); return; }
        if (MessageBox.Show("Ngừng bán biến thể đang chọn? Dữ liệu lịch sử được giữ lại.", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        Run(() => _products.Deactivate(_selectedVariantId.Value), "Đã ngừng bán biến thể.");
    }

    private void grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (grid.CurrentRow?.DataBoundItem is not DataRowView view) return;
        var row = view.Row;
        _selectedVariantId = Convert.ToInt32(row["VARIANT_ID"]);
        _selectedStyleId = Convert.ToInt32(row["STYLE_ID"]);
        styleName.Text = Convert.ToString(row["STYLE_NAME"]); brand.Text = Convert.ToString(row["BRAND"]); category.Text = Convert.ToString(row["CATEGORY"]);
        sku.Text = Convert.ToString(row["SKU"]); sizeCode.Text = Convert.ToString(row["SIZE_CODE"]); color.Text = Convert.ToString(row["COLOR"]);
        price.Value = Math.Clamp(Convert.ToDecimal(row["SELL_PRICE"]), price.Minimum, price.Maximum);
        reorder.Value = Math.Clamp(Convert.ToDecimal(row["REORDER_LEVEL"]), reorder.Minimum, reorder.Maximum);
        statusLabel.Text = $"Tồn hiện tại: {row["STOCK_QTY"]}. Tồn kho chỉ thay đổi qua nhập hàng, bán hàng hoặc kiểm kê.";
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(styleName.Text) || string.IsNullOrWhiteSpace(brand.Text) || string.IsNullOrWhiteSpace(sku.Text) || string.IsNullOrWhiteSpace(sizeCode.Text) || string.IsNullOrWhiteSpace(color.Text))
        { Warn("Nhập đủ tên mẫu, thương hiệu, SKU, kích cỡ và màu sắc."); return false; }
        return true;
    }

    private bool ValidateVariantInput()
    {
        if (string.IsNullOrWhiteSpace(sku.Text) || string.IsNullOrWhiteSpace(sizeCode.Text) || string.IsNullOrWhiteSpace(color.Text))
        { Warn("Nhập SKU, kích cỡ và màu sắc cho biến thể."); return false; }
        return true;
    }

    private void Run(Action action, string success)
    {
        try { action(); Reload(searchBox.Text); ClearFields(); statusLabel.Text = success; }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể lưu dữ liệu Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void Reload(string keyword = "")
    {
        try
        {
            grid.DataSource = _products.Search(keyword);
            foreach (var name in new[] { "VARIANT_ID", "STYLE_ID" }) if (grid.Columns.Contains(name)) grid.Columns[name].Visible = false;
            var captions = new Dictionary<string, string> { ["SKU"] = "Mã SKU", ["STYLE_NAME"] = "Mẫu giày", ["BRAND"] = "Thương hiệu", ["CATEGORY"] = "Loại", ["SIZE_CODE"] = "Cỡ", ["COLOR"] = "Màu", ["SELL_PRICE"] = "Giá bán", ["STOCK_QTY"] = "Tồn", ["REORDER_LEVEL"] = "Ngưỡng" };
            foreach (var item in captions) if (grid.Columns.Contains(item.Key)) grid.Columns[item.Key].HeaderText = item.Value;
            statusLabel.Text = $"Có {grid.Rows.Count} biến thể đang kinh doanh.";
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Không tải được sản phẩm từ Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void ClearFields()
    { _selectedStyleId = null; _selectedVariantId = null; styleName.Clear(); brand.Clear(); category.Clear(); sku.Clear(); sizeCode.Clear(); color.Clear(); price.Value = 0; reorder.Value = 3; grid.ClearSelection(); }

    private void Warn(string message) => MessageBox.Show(message, "Kiểm tra thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
