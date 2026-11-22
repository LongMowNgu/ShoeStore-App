#nullable enable
namespace QuanLyBanGiay;

partial class PosForm
{
    private System.ComponentModel.IContainer? components;
    private TextBox searchBox = null!;
    private DataGridView productsGrid = null!;
    private DataGridView cartGrid = null!;
    private NumericUpDown quantity = null!;
    private NumericUpDown employeeId = null!;
    private Label totalLabel = null!;
    private Label statusLabel = null!;
    private Button addButton = null!;
    private Button checkoutButton = null!;
    private Button search = null!;
    private Button reload = null!;
    private Label title = null!;
    private Label qtyLabel = null!;
    private Label empLabel = null!;
    private TableLayoutPanel rootLayout = null!;
    private TableLayoutPanel contentLayout = null!;
    private TableLayoutPanel productLayout = null!;
    private TableLayoutPanel cartLayout = null!;
    private FlowLayoutPanel searchBar = null!;
    private FlowLayoutPanel productActions = null!;
    private TableLayoutPanel cartFooter = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        searchBox = new TextBox(); productsGrid = new DataGridView(); cartGrid = new DataGridView();
        quantity = new NumericUpDown(); employeeId = new NumericUpDown(); totalLabel = new Label(); statusLabel = new Label();
        addButton = new Button(); checkoutButton = new Button(); search = new Button(); reload = new Button();
        title = new Label(); qtyLabel = new Label(); empLabel = new Label();
        rootLayout = new TableLayoutPanel(); contentLayout = new TableLayoutPanel(); productLayout = new TableLayoutPanel(); cartLayout = new TableLayoutPanel();
        searchBar = new FlowLayoutPanel(); productActions = new FlowLayoutPanel(); cartFooter = new TableLayoutPanel();
        ((System.ComponentModel.ISupportInitialize)productsGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cartGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)quantity).BeginInit();
        ((System.ComponentModel.ISupportInitialize)employeeId).BeginInit();
        SuspendLayout();
        
        Text = "Bán hàng tại quầy"; Name = "PosForm"; StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(1240, 760); MinimumSize = new Size(1000, 650);
        Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(244, 247, 251); WindowState = FormWindowState.Maximized;
        rootLayout.Dock = DockStyle.Fill; rootLayout.Padding = new Padding(24, 18, 24, 14); rootLayout.ColumnCount = 1; rootLayout.RowCount = 3;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        title.Dock = DockStyle.Fill; title.Text = "BÁN HÀNG TẠI QUẦY"; title.Font = new Font("Segoe UI", 20F, FontStyle.Bold); title.ForeColor = Color.FromArgb(40, 91, 145); title.TextAlign = ContentAlignment.MiddleLeft;
        contentLayout.Dock = DockStyle.Fill; contentLayout.ColumnCount = 2; contentLayout.RowCount = 1; contentLayout.Margin = new Padding(0);
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F)); contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F)); contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        productLayout.Dock = DockStyle.Fill; productLayout.Margin = new Padding(0, 0, 12, 0); productLayout.ColumnCount = 1; productLayout.RowCount = 3;
        productLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); productLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F)); productLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); productLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
        searchBar.Dock = DockStyle.Fill; searchBar.WrapContents = false; searchBar.Margin = new Padding(0, 0, 0, 8); searchBar.Padding = new Padding(0, 2, 0, 0);
        searchBox.Name = "searchBox"; searchBox.PlaceholderText = "Tìm tên giày hoặc mã biến thể"; searchBox.Size = new Size(300, 30); searchBox.Margin = new Padding(0, 0, 8, 0);
        search.Name = "search"; search.Text = "Tìm"; search.Size = new Size(82, 32); search.Click += search_Click;
        reload.Name = "reload"; reload.Text = "Tải lại"; reload.Size = new Size(90, 32); reload.Click += reload_Click;
        searchBar.Controls.Add(searchBox); searchBar.Controls.Add(search); searchBar.Controls.Add(reload);
        productsGrid.Name = "productsGrid"; productsGrid.Dock = DockStyle.Fill; productsGrid.Margin = new Padding(0); productsGrid.ReadOnly = true; productsGrid.AllowUserToAddRows = false; productsGrid.AllowUserToDeleteRows = false; productsGrid.MultiSelect = false; productsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; productsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; productsGrid.RowHeadersVisible = false; productsGrid.BackgroundColor = Color.White; productsGrid.SelectionChanged += productsGrid_SelectionChanged;
        productActions.Dock = DockStyle.Fill; productActions.WrapContents = true; productActions.AutoScroll = true; productActions.Padding = new Padding(0, 12, 0, 0); productActions.Margin = new Padding(0);
        qtyLabel.Text = "Số lượng"; qtyLabel.AutoSize = true; qtyLabel.Margin = new Padding(0, 8, 6, 0);
        quantity.Name = "quantity"; quantity.Minimum = 1; quantity.Maximum = 100000; quantity.Value = 1; quantity.Width = 90; quantity.Margin = new Padding(0, 4, 12, 0);
        addButton.Name = "addButton"; addButton.Text = "Thêm vào giỏ"; addButton.Size = new Size(145, 36); addButton.Margin = new Padding(0, 0, 14, 0); addButton.Click += addButton_Click;
        empLabel.Text = "Mã nhân viên"; empLabel.AutoSize = true; empLabel.Margin = new Padding(0, 8, 6, 0);
        employeeId.Name = "employeeId"; employeeId.Maximum = 1000000000; employeeId.Width = 145; employeeId.Margin = new Padding(0, 4, 0, 0);
        productActions.Controls.Add(qtyLabel); productActions.Controls.Add(quantity); productActions.Controls.Add(addButton); productActions.Controls.Add(empLabel); productActions.Controls.Add(employeeId);
        productLayout.Controls.Add(searchBar, 0, 0); productLayout.Controls.Add(productsGrid, 0, 1); productLayout.Controls.Add(productActions, 0, 2);
        cartLayout.Dock = DockStyle.Fill; cartLayout.Margin = new Padding(12, 0, 0, 0); cartLayout.ColumnCount = 1; cartLayout.RowCount = 2;
        cartLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); cartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); cartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
        cartGrid.Name = "cartGrid"; cartGrid.Dock = DockStyle.Fill; cartGrid.Margin = new Padding(0); cartGrid.ReadOnly = true; cartGrid.AllowUserToAddRows = false; cartGrid.AllowUserToDeleteRows = false; cartGrid.MultiSelect = false; cartGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; cartGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; cartGrid.RowHeadersVisible = false; cartGrid.BackgroundColor = Color.White;
        cartFooter.Dock = DockStyle.Fill; cartFooter.ColumnCount = 2; cartFooter.RowCount = 1; cartFooter.Margin = new Padding(0, 8, 0, 0);
        cartFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F)); cartFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F)); cartFooter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        totalLabel.Name = "totalLabel"; totalLabel.Text = "Tổng: 0 VNĐ"; totalLabel.AutoSize = true; totalLabel.Font = new Font("Segoe UI", 17F, FontStyle.Bold); totalLabel.ForeColor = Color.FromArgb(40, 91, 145); totalLabel.Anchor = AnchorStyles.Left; totalLabel.TextAlign = ContentAlignment.MiddleLeft;
        checkoutButton.Name = "checkoutButton"; checkoutButton.Text = "Thanh toán / Xuất hóa đơn"; checkoutButton.Size = new Size(220, 40); checkoutButton.Anchor = AnchorStyles.Right; checkoutButton.Click += checkoutButton_Click;
        cartFooter.Controls.Add(totalLabel, 0, 0); cartFooter.Controls.Add(checkoutButton, 1, 0);
        cartLayout.Controls.Add(cartGrid, 0, 0); cartLayout.Controls.Add(cartFooter, 0, 1);
        contentLayout.Controls.Add(productLayout, 0, 0); contentLayout.Controls.Add(cartLayout, 1, 0);
        statusLabel.Dock = DockStyle.Fill; statusLabel.Name = "statusLabel"; statusLabel.AutoSize = true; statusLabel.ForeColor = Color.FromArgb(105, 120, 139); statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        rootLayout.Controls.Add(title, 0, 0); rootLayout.Controls.Add(contentLayout, 0, 1); rootLayout.Controls.Add(statusLabel, 0, 2);
        Controls.Add(rootLayout);
        ((System.ComponentModel.ISupportInitialize)productsGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)cartGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)quantity).EndInit(); ((System.ComponentModel.ISupportInitialize)employeeId).EndInit();
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
