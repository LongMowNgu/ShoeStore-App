#nullable enable
namespace ShoeStore.WinForms.UI;

partial class ProductsForm
{
    private System.ComponentModel.IContainer? components;
    private TextBox searchBox = null!;
    private TextBox styleName = null!;
    private TextBox brand = null!;
    private TextBox category = null!;
    private TextBox sku = null!;
    private TextBox sizeCode = null!;
    private TextBox color = null!;
    private NumericUpDown price = null!;
    private NumericUpDown reorder = null!;
    private DataGridView grid = null!;
    private Label statusLabel = null!;
    private Button addButton = null!;
    private Button addVariantButton = null!;
    private Button updateButton = null!;
    private Button deactivateButton = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        searchBox = new TextBox(); styleName = new TextBox(); brand = new TextBox(); category = new TextBox();
        sku = new TextBox(); sizeCode = new TextBox(); color = new TextBox(); price = new NumericUpDown(); reorder = new NumericUpDown(); grid = new DataGridView(); statusLabel = new Label();
        addButton = new Button(); addVariantButton = new Button(); updateButton = new Button(); deactivateButton = new Button();
        var title = new Label(); var searchButton = new Button(); var reloadButton = new Button();
        ((System.ComponentModel.ISupportInitialize)price).BeginInit(); ((System.ComponentModel.ISupportInitialize)reorder).BeginInit(); ((System.ComponentModel.ISupportInitialize)grid).BeginInit(); SuspendLayout();
        Text = "Sản phẩm và biến thể"; StartPosition = FormStartPosition.CenterParent; WindowState = FormWindowState.Maximized; MinimumSize = new Size(1100, 720); Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(245, 247, 250);
        title.AutoSize = true; title.Font = new Font("Segoe UI", 20F, FontStyle.Bold); title.Location = new Point(24, 20); title.Text = "MẪU GIÀY VÀ BIẾN THỂ";
        searchBox.Location = new Point(28, 75); searchBox.Size = new Size(340, 28); searchBox.PlaceholderText = "Tên mẫu, thương hiệu hoặc SKU";
        searchButton.Location = new Point(380, 73); searchButton.Size = new Size(105, 32); searchButton.Text = "Tìm kiếm"; searchButton.Click += searchButton_Click;
        reloadButton.Location = new Point(492, 73); reloadButton.Size = new Size(95, 32); reloadButton.Text = "Tải lại"; reloadButton.Click += reloadButton_Click;
        grid.Location = new Point(28, 122); grid.Size = new Size(680, 490); grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false; grid.MultiSelect = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.RowHeadersVisible = false; grid.SelectionChanged += grid_SelectionChanged;
        var panel = new GroupBox { Text = "Thông tin mẫu và phiên bản", Location = new Point(730, 120), Size = new Size(360, 492), Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
        AddField(panel, "Tên mẫu giày *", styleName, 28, 40); AddField(panel, "Thương hiệu *", brand, 88, 40); AddField(panel, "Loại giày", category, 148, 40);
        AddField(panel, "SKU / mã biến thể *", sku, 208, 40); AddField(panel, "Kích cỡ *", sizeCode, 268, 40); AddField(panel, "Màu sắc *", color, 328, 40);
        price.Location = new Point(18, 408); price.Size = new Size(150, 28); price.Maximum = 1000000000; price.DecimalPlaces = 0; price.ThousandsSeparator = true;
        reorder.Location = new Point(190, 408); reorder.Size = new Size(145, 28); reorder.Maximum = 1000000;
        panel.Controls.Add(new Label { Text = "Giá bán (VNĐ)", Location = new Point(18, 386), AutoSize = true });
        panel.Controls.Add(new Label { Text = "Ngưỡng cảnh báo tồn", Location = new Point(190, 386), AutoSize = true }); panel.Controls.Add(price); panel.Controls.Add(reorder);
        addButton.Location = new Point(14, 448); addButton.Size = new Size(78, 32); addButton.Text = "Thêm mẫu"; addButton.Click += addButton_Click;
        addVariantButton.Location = new Point(96, 448); addVariantButton.Size = new Size(92, 32); addVariantButton.Text = "Thêm cỡ/màu"; addVariantButton.Click += addVariantButton_Click;
        updateButton.Location = new Point(192, 448); updateButton.Size = new Size(72, 32); updateButton.Text = "Cập nhật"; updateButton.Click += updateButton_Click;
        deactivateButton.Location = new Point(268, 448); deactivateButton.Size = new Size(75, 32); deactivateButton.Text = "Ngừng bán"; deactivateButton.Click += deactivateButton_Click;
        panel.Controls.AddRange(new Control[] { addButton, addVariantButton, updateButton, deactivateButton });
        statusLabel.AutoSize = true; statusLabel.Location = new Point(30, 630); statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; statusLabel.ForeColor = Color.DimGray;
        Controls.AddRange(new Control[] { title, searchBox, searchButton, reloadButton, grid, panel, statusLabel });
        ((System.ComponentModel.ISupportInitialize)price).EndInit(); ((System.ComponentModel.ISupportInitialize)reorder).EndInit(); ((System.ComponentModel.ISupportInitialize)grid).EndInit(); ResumeLayout(false); PerformLayout();
    }

    private static void AddField(Control parent, string caption, TextBox input, int y, int maxLength)
    {
        parent.Controls.Add(new Label { Text = caption, Location = new Point(18, y), AutoSize = true });
        input.Location = new Point(18, y + 21); input.Size = new Size(320, 26); input.MaxLength = maxLength; parent.Controls.Add(input);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
}
