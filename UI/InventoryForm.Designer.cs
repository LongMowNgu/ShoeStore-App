#nullable enable
namespace QuanLyBanGiay;

partial class InventoryForm
{
    private System.ComponentModel.IContainer? components;
    private DataGridView stockGrid = null!;
    private DataGridView movementsGrid = null!;
    private TextBox searchBox = null!;
    private CheckBox lowStockOnly = null!;
    private TextBox supplierBox = null!;
    private NumericUpDown quantity = null!;
    private NumericUpDown unitCost = null!;
    private NumericUpDown employeeId = null!;
    private Label statusLabel = null!;
    private Button receiveButton = null!;
    private Label title = null!;
    private Button search = null!;
    private Button refresh = null!;
    private Label qtyLabel = null!;
    private Label costLabel = null!;
    private Label supplierLabel = null!;
    private Label empLabel = null!;
    private TabControl tabs = null!;
    private TabPage stockTab = null!;
    private TabPage movementTab = null!;
    private TableLayoutPanel rootLayout = null!;
    private FlowLayoutPanel searchPanel = null!;
    private GroupBox receiveBox = null!;
    private FlowLayoutPanel receivePanel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container(); stockGrid = new DataGridView(); movementsGrid = new DataGridView(); searchBox = new TextBox(); lowStockOnly = new CheckBox(); supplierBox = new TextBox(); quantity = new NumericUpDown(); unitCost = new NumericUpDown(); employeeId = new NumericUpDown(); statusLabel = new Label(); receiveButton = new Button(); title = new Label(); search = new Button(); refresh = new Button(); qtyLabel = new Label(); costLabel = new Label(); supplierLabel = new Label(); empLabel = new Label(); tabs = new TabControl(); stockTab = new TabPage(); movementTab = new TabPage(); rootLayout = new TableLayoutPanel(); searchPanel = new FlowLayoutPanel(); receiveBox = new GroupBox(); receivePanel = new FlowLayoutPanel();
        ((System.ComponentModel.ISupportInitialize)stockGrid).BeginInit(); ((System.ComponentModel.ISupportInitialize)movementsGrid).BeginInit(); ((System.ComponentModel.ISupportInitialize)quantity).BeginInit(); ((System.ComponentModel.ISupportInitialize)unitCost).BeginInit(); ((System.ComponentModel.ISupportInitialize)employeeId).BeginInit(); SuspendLayout();
        Text = "Kho và nhập hàng"; Name = "InventoryForm"; StartPosition = FormStartPosition.CenterParent; WindowState = FormWindowState.Maximized; AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(1240, 780); MinimumSize = new Size(1040, 680); Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(244, 247, 251);
        rootLayout.Dock = DockStyle.Fill; rootLayout.Padding = new Padding(24, 18, 24, 14); rootLayout.ColumnCount = 1; rootLayout.RowCount = 5; rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 126F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        title.Dock = DockStyle.Fill; title.Text = "KHO HÀNG · NHẬP HÀNG · CẢNH BÁO TỒN"; title.Font = new Font("Segoe UI", 20F, FontStyle.Bold); title.ForeColor = Color.FromArgb(40, 91, 145); title.TextAlign = ContentAlignment.MiddleLeft;
        searchPanel.Dock = DockStyle.Fill; searchPanel.WrapContents = false; searchPanel.Margin = new Padding(0); searchPanel.Padding = new Padding(0, 3, 0, 0);
        searchBox.Name = "searchBox"; searchBox.PlaceholderText = "Tìm SKU hoặc tên mẫu"; searchBox.Size = new Size(320, 30); searchBox.Margin = new Padding(0, 0, 8, 0);
        search.Name = "search"; search.Text = "Tìm"; search.Size = new Size(86, 32); search.Margin = new Padding(0, 0, 8, 0); search.Click += search_Click;
        refresh.Name = "refresh"; refresh.Text = "Tải lại"; refresh.Size = new Size(92, 32); refresh.Click += refresh_Click;
        lowStockOnly.Name = "lowStockOnly"; lowStockOnly.Text = "Chỉ hiện hết hàng"; lowStockOnly.AutoSize = true; lowStockOnly.Margin = new Padding(8, 8, 0, 0); lowStockOnly.CheckedChanged += lowStockOnly_CheckedChanged;
        searchPanel.Controls.Add(searchBox); searchPanel.Controls.Add(search); searchPanel.Controls.Add(refresh); searchPanel.Controls.Add(lowStockOnly);
        tabs.Name = "tabs"; tabs.Dock = DockStyle.Fill; tabs.Margin = new Padding(0); stockTab.Text = "Tồn kho"; movementTab.Text = "Lịch sử nhập / bán / đơn";
        stockGrid.Name = "stockGrid"; stockGrid.Dock = DockStyle.Fill; stockGrid.ReadOnly = true; stockGrid.AllowUserToAddRows = false; stockGrid.AllowUserToDeleteRows = false; stockGrid.MultiSelect = false; stockGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; stockGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; stockGrid.RowHeadersVisible = false; stockGrid.BackgroundColor = Color.White; stockGrid.SelectionChanged += stockGrid_SelectionChanged; stockGrid.CellFormatting += stockGrid_CellFormatting;
        movementsGrid.Name = "movementsGrid"; movementsGrid.Dock = DockStyle.Fill; movementsGrid.ReadOnly = true; movementsGrid.AllowUserToAddRows = false; movementsGrid.AllowUserToDeleteRows = false; movementsGrid.MultiSelect = false; movementsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; movementsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; movementsGrid.RowHeadersVisible = false; movementsGrid.BackgroundColor = Color.White;
        stockTab.Controls.Add(stockGrid); movementTab.Controls.Add(movementsGrid); tabs.TabPages.Add(stockTab); tabs.TabPages.Add(movementTab);
        receiveBox.Dock = DockStyle.Fill; receiveBox.Text = "Tạo phiếu nhập hàng cho biến thể đang chọn"; receiveBox.Padding = new Padding(12); receivePanel.Dock = DockStyle.Fill; receivePanel.WrapContents = true; receivePanel.AutoScroll = true; receivePanel.Padding = new Padding(2, 8, 2, 2);
        qtyLabel.Text = "Số lượng nhập"; qtyLabel.AutoSize = true; qtyLabel.Margin = new Padding(0, 9, 4, 0); quantity.Name = "quantity"; quantity.Minimum = 1; quantity.Maximum = 1000000; quantity.Value = 1; quantity.Width = 105; quantity.Margin = new Padding(0, 4, 12, 0);
        costLabel.Text = "Giá nhập/đôi"; costLabel.AutoSize = true; costLabel.Margin = new Padding(0, 9, 4, 0); unitCost.Name = "unitCost"; unitCost.Maximum = 1000000000; unitCost.DecimalPlaces = 0; unitCost.ThousandsSeparator = true; unitCost.Width = 145; unitCost.Margin = new Padding(0, 4, 12, 0);
        supplierLabel.Text = "Nhà cung cấp"; supplierLabel.AutoSize = true; supplierLabel.Margin = new Padding(0, 9, 4, 0); supplierBox.Name = "supplierBox"; supplierBox.MaxLength = 150; supplierBox.PlaceholderText = "Tên nhà cung cấp"; supplierBox.Width = 190; supplierBox.Margin = new Padding(0, 4, 12, 0);
        empLabel.Text = "Mã nhân viên"; empLabel.AutoSize = true; empLabel.Margin = new Padding(0, 9, 4, 0); employeeId.Name = "employeeId"; employeeId.Maximum = 1000000000; employeeId.Width = 110; employeeId.Margin = new Padding(0, 4, 12, 0);
        receiveButton.Name = "receiveButton"; receiveButton.Text = "Lưu phiếu nhập"; receiveButton.Size = new Size(150, 36); receiveButton.Margin = new Padding(0, 0, 8, 0); receiveButton.Click += receiveButton_Click;
        receivePanel.Controls.Add(qtyLabel); receivePanel.Controls.Add(quantity); receivePanel.Controls.Add(costLabel); receivePanel.Controls.Add(unitCost); receivePanel.Controls.Add(supplierLabel); receivePanel.Controls.Add(supplierBox); receivePanel.Controls.Add(empLabel); receivePanel.Controls.Add(employeeId); receivePanel.Controls.Add(receiveButton); receiveBox.Controls.Add(receivePanel);
        statusLabel.Dock = DockStyle.Fill; statusLabel.Name = "statusLabel"; statusLabel.AutoSize = true; statusLabel.ForeColor = Color.FromArgb(105, 120, 139); statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.Controls.Add(title, 0, 0); rootLayout.Controls.Add(searchPanel, 0, 1); rootLayout.Controls.Add(tabs, 0, 2); rootLayout.Controls.Add(receiveBox, 0, 3); rootLayout.Controls.Add(statusLabel, 0, 4); Controls.Add(rootLayout);
        ((System.ComponentModel.ISupportInitialize)stockGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)movementsGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)quantity).EndInit(); ((System.ComponentModel.ISupportInitialize)unitCost).EndInit(); ((System.ComponentModel.ISupportInitialize)employeeId).EndInit(); ResumeLayout(false);
    }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
