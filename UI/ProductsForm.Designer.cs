#nullable enable
namespace QuanLyBanGiay;

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
    private Label title = null!;
    private Button searchButton = null!;
    private Button reloadButton = null!;
    private GroupBox panel = null!;
    private TableLayoutPanel rootLayout = null!;
    private TableLayoutPanel contentLayout = null!;
    private FlowLayoutPanel searchPanel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        searchBox = new TextBox(); styleName = new TextBox(); brand = new TextBox(); category = new TextBox(); sku = new TextBox(); sizeCode = new TextBox(); color = new TextBox();
        price = new NumericUpDown(); reorder = new NumericUpDown(); grid = new DataGridView(); statusLabel = new Label();
        addButton = new Button(); addVariantButton = new Button(); updateButton = new Button(); deactivateButton = new Button();
        title = new Label(); searchButton = new Button(); reloadButton = new Button(); panel = new GroupBox();
        rootLayout = new TableLayoutPanel(); contentLayout = new TableLayoutPanel(); searchPanel = new FlowLayoutPanel();
        labelStyle = new Label(); labelBrand = new Label(); labelCategory = new Label(); labelSku = new Label(); labelSize = new Label(); labelColor = new Label(); labelPrice = new Label(); labelReorder = new Label();
        ((System.ComponentModel.ISupportInitialize)price).BeginInit(); ((System.ComponentModel.ISupportInitialize)reorder).BeginInit(); ((System.ComponentModel.ISupportInitialize)grid).BeginInit(); SuspendLayout();
        Text = "Sản phẩm và biến thể"; Name = "ProductsForm"; StartPosition = FormStartPosition.CenterParent; WindowState = FormWindowState.Maximized; AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(1240, 780); MinimumSize = new Size(1050, 680); Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(244, 247, 251);
        rootLayout.Dock = DockStyle.Fill; rootLayout.Padding = new Padding(24, 18, 24, 14); rootLayout.ColumnCount = 1; rootLayout.RowCount = 4;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        title.Dock = DockStyle.Fill; title.Text = "SẢN PHẨM VÀ BIẾN THỂ"; title.Font = new Font("Segoe UI", 20F, FontStyle.Bold); title.ForeColor = Color.FromArgb(40, 91, 145); title.TextAlign = ContentAlignment.MiddleLeft;
        searchPanel.Dock = DockStyle.Fill; searchPanel.WrapContents = false; searchPanel.Margin = new Padding(0); searchPanel.Padding = new Padding(0, 4, 0, 0);
        searchBox.Name = "searchBox"; searchBox.PlaceholderText = "Tên mẫu, thương hiệu hoặc mã biến thể"; searchBox.Size = new Size(340, 30); searchBox.Margin = new Padding(0, 0, 8, 0);
        searchButton.Name = "searchButton"; searchButton.Text = "Tìm kiếm"; searchButton.Size = new Size(105, 32); searchButton.Margin = new Padding(0, 0, 8, 0); searchButton.Click += searchButton_Click;
        reloadButton.Name = "reloadButton"; reloadButton.Text = "Tải lại"; reloadButton.Size = new Size(95, 32); reloadButton.Click += reloadButton_Click;
        searchPanel.Controls.Add(searchBox); searchPanel.Controls.Add(searchButton); searchPanel.Controls.Add(reloadButton);
        contentLayout.Dock = DockStyle.Fill; contentLayout.ColumnCount = 2; contentLayout.RowCount = 1; contentLayout.Margin = new Padding(0); contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F)); contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F)); contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        grid.Name = "grid"; grid.Dock = DockStyle.Fill; grid.Margin = new Padding(0, 0, 12, 0); grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false; grid.MultiSelect = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White; grid.SelectionChanged += grid_SelectionChanged;
        panel.Name = "panel"; panel.Text = "Thông tin mẫu và biến thể"; panel.Dock = DockStyle.Fill; panel.Margin = new Padding(12, 0, 0, 0); panel.Padding = new Padding(14);
        styleName.Name = "styleName"; styleName.MaxLength = 100; styleName.Location = new Point(18, 42); styleName.Size = new Size(320, 28); styleName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        brand.Name = "brand"; brand.MaxLength = 50; brand.Location = new Point(18, 102); brand.Size = new Size(320, 28); brand.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        category.Name = "category"; category.MaxLength = 50; category.Location = new Point(18, 162); category.Size = new Size(320, 28); category.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sku.Name = "sku"; sku.MaxLength = 40; sku.Location = new Point(18, 222); sku.Size = new Size(320, 28); sku.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        sizeCode.Name = "sizeCode"; sizeCode.MaxLength = 10; sizeCode.Location = new Point(18, 282); sizeCode.Size = new Size(150, 28);
        color.Name = "color"; color.MaxLength = 30; color.Location = new Point(198, 282); color.Size = new Size(150, 28); color.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        price.Name = "price"; price.Location = new Point(18, 350); price.Size = new Size(160, 28); price.Maximum = 1000000000; price.DecimalPlaces = 0; price.ThousandsSeparator = true;
        reorder.Name = "reorder"; reorder.Location = new Point(198, 350); reorder.Size = new Size(150, 28); reorder.Maximum = 1000000; reorder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        labelStyle.Text = "Tên mẫu giày *"; labelStyle.Location = new Point(18, 20); labelStyle.AutoSize = true;
        labelBrand.Text = "Thương hiệu *"; labelBrand.Location = new Point(18, 80); labelBrand.AutoSize = true;
        labelCategory.Text = "Loại giày"; labelCategory.Location = new Point(18, 140); labelCategory.AutoSize = true;
        labelSku.Text = "Mã nhà cung cấp *"; labelSku.Location = new Point(18, 200); labelSku.AutoSize = true;
        labelSize.Text = "Kích cỡ *"; labelSize.Location = new Point(18, 260); labelSize.AutoSize = true;
        labelColor.Text = "Màu sắc *"; labelColor.Location = new Point(198, 260); labelColor.AutoSize = true;
        labelPrice.Text = "Giá bán (VNĐ)"; labelPrice.Location = new Point(18, 328); labelPrice.AutoSize = true;
        labelReorder.Text = "Ngưỡng cảnh báo tồn"; labelReorder.Location = new Point(198, 328); labelReorder.AutoSize = true;
        addButton.Text = "Thêm mẫu"; addButton.Location = new Point(18, 400); addButton.Size = new Size(84, 34); addButton.Click += addButton_Click;
        addVariantButton.Text = "Thêm cỡ/màu"; addVariantButton.Location = new Point(108, 400); addVariantButton.Size = new Size(108, 34); addVariantButton.Click += addVariantButton_Click;
        updateButton.Text = "Cập nhật"; updateButton.Location = new Point(222, 400); updateButton.Size = new Size(82, 34); updateButton.Click += updateButton_Click;
        deactivateButton.Text = "Ngừng bán"; deactivateButton.Location = new Point(18, 444); deactivateButton.Size = new Size(105, 34); deactivateButton.Click += deactivateButton_Click;
        panel.Controls.Add(labelStyle); panel.Controls.Add(styleName); panel.Controls.Add(labelBrand); panel.Controls.Add(brand); panel.Controls.Add(labelCategory); panel.Controls.Add(category); panel.Controls.Add(labelSku); panel.Controls.Add(sku); panel.Controls.Add(labelSize); panel.Controls.Add(sizeCode); panel.Controls.Add(labelColor); panel.Controls.Add(color); panel.Controls.Add(labelPrice); panel.Controls.Add(price); panel.Controls.Add(labelReorder); panel.Controls.Add(reorder); panel.Controls.Add(addButton); panel.Controls.Add(addVariantButton); panel.Controls.Add(updateButton); panel.Controls.Add(deactivateButton);
        contentLayout.Controls.Add(grid, 0, 0); contentLayout.Controls.Add(panel, 1, 0);
        statusLabel.Dock = DockStyle.Fill; statusLabel.Name = "statusLabel"; statusLabel.AutoSize = true; statusLabel.ForeColor = Color.FromArgb(105, 120, 139); statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.Controls.Add(title, 0, 0); rootLayout.Controls.Add(searchPanel, 0, 1); rootLayout.Controls.Add(contentLayout, 0, 2); rootLayout.Controls.Add(statusLabel, 0, 3);
        Controls.Add(rootLayout);
        ((System.ComponentModel.ISupportInitialize)price).EndInit(); ((System.ComponentModel.ISupportInitialize)reorder).EndInit(); ((System.ComponentModel.ISupportInitialize)grid).EndInit(); ResumeLayout(false);
    }

    private Label labelStyle = null!;
    private Label labelBrand = null!;
    private Label labelCategory = null!;
    private Label labelSku = null!;
    private Label labelSize = null!;
    private Label labelColor = null!;
    private Label labelPrice = null!;
    private Label labelReorder = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
