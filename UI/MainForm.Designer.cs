#nullable enable
namespace QuanLyBanGiay;

partial class MainForm
{
    private System.ComponentModel.IContainer? components;
    private Panel sidebar = null!;
    private Panel mainPanel = null!;
    private Panel dashboardPanel = null!;
    private Panel pageHost = null!;
    private Panel brandPanel = null!;
    private Panel userPanel = null!;
    private Panel headerPanel = null!;
    private Panel headerActions = null!;
    private Panel ordersHeaderPanel = null!;
    private Label brandLabel = null!;
    private Label userLabel = null!;
    private Label pageTitle = null!;
    private Label subtitleLabel = null!;
    private Label dateLabel = null!;
    private Label connectionLabel = null!;
    private Button homeButton = null!;
    private Button productButton = null!;
    private Button posButton = null!;
    private Button inventoryButton = null!;
    private Button onlineButton = null!;
    private Button reportButton = null!;
    private Button accountButton = null!;
    private Button refreshButton = null!;
    private Button logoutButton = null!;
    private Label salesValue = null!;
    private Label ordersValue = null!;
    private Label stockValue = null!;
    private Label productsValue = null!;
    private DataGridView recentOrdersGrid = null!;
    private TableLayoutPanel dashboardLayout = null!;
    private TableLayoutPanel cardLayout = null!;
    private Panel cardSales = null!;
    private Panel cardOrders = null!;
    private Panel cardStock = null!;
    private Panel cardProducts = null!;
    private Label salesTitle = null!;
    private Label ordersTitle = null!;
    private Label stockTitle = null!;
    private Label productsTitle = null!;
    private Label ordersHeading = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        sidebar = new Panel();
        accountButton = new Button();
        reportButton = new Button();
        inventoryButton = new Button();
        onlineButton = new Button();
        posButton = new Button();
        productButton = new Button();
        homeButton = new Button();
        userPanel = new Panel();
        userLabel = new Label();
        logoutButton = new Button();
        brandPanel = new Panel();
        brandLabel = new Label();
        mainPanel = new Panel();
        pageHost = new Panel();
        dashboardPanel = new Panel();
        dashboardLayout = new TableLayoutPanel();
        headerPanel = new Panel();
        pageTitle = new Label();
        subtitleLabel = new Label();
        headerActions = new Panel();
        dateLabel = new Label();
        refreshButton = new Button();
        cardLayout = new TableLayoutPanel();
        cardSales = new Panel();
        salesTitle = new Label();
        salesValue = new Label();
        cardOrders = new Panel();
        ordersTitle = new Label();
        ordersValue = new Label();
        cardStock = new Panel();
        stockTitle = new Label();
        stockValue = new Label();
        cardProducts = new Panel();
        productsTitle = new Label();
        productsValue = new Label();
        ordersHeaderPanel = new Panel();
        ordersHeading = new Label();
        connectionLabel = new Label();
        recentOrdersGrid = new DataGridView();
        sidebar.SuspendLayout();
        userPanel.SuspendLayout();
        brandPanel.SuspendLayout();
        mainPanel.SuspendLayout();
        dashboardPanel.SuspendLayout();
        dashboardLayout.SuspendLayout();
        headerPanel.SuspendLayout();
        headerActions.SuspendLayout();
        cardLayout.SuspendLayout();
        cardSales.SuspendLayout();
        cardOrders.SuspendLayout();
        cardStock.SuspendLayout();
        cardProducts.SuspendLayout();
        ordersHeaderPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)recentOrdersGrid).BeginInit();
        SuspendLayout();
        // 
        // sidebar
        // 
        sidebar.BackColor = Color.FromArgb(24, 39, 58);
        sidebar.Controls.Add(accountButton);
        sidebar.Controls.Add(reportButton);
        sidebar.Controls.Add(inventoryButton);
        sidebar.Controls.Add(onlineButton);
        sidebar.Controls.Add(posButton);
        sidebar.Controls.Add(productButton);
        sidebar.Controls.Add(homeButton);
        sidebar.Controls.Add(userPanel);
        sidebar.Controls.Add(brandPanel);
        sidebar.Dock = DockStyle.Left;
        sidebar.Location = new Point(0, 0);
        sidebar.Name = "sidebar";
        sidebar.Size = new Size(238, 900);
        sidebar.TabIndex = 1;
        // 
        // accountButton
        // 
        accountButton.Location = new Point(16, 458);
        accountButton.Name = "accountButton";
        accountButton.Size = new Size(206, 52);
        accountButton.TabIndex = 6;
        accountButton.Text = "Nhân viên & phân quyền";
        accountButton.Click += accountButton_Click;
        // 
        // reportButton
        // 
        reportButton.Location = new Point(16, 395);
        reportButton.Name = "reportButton";
        reportButton.Size = new Size(206, 44);
        reportButton.TabIndex = 5;
        reportButton.Text = "Thống kê & báo cáo";
        reportButton.Click += moduleButton_Click;
        // 
        // inventoryButton
        // 
        inventoryButton.Location = new Point(16, 333);
        inventoryButton.Name = "inventoryButton";
        inventoryButton.Size = new Size(206, 44);
        inventoryButton.TabIndex = 4;
        inventoryButton.Text = "Kho & nhập hàng";
        inventoryButton.Click += moduleButton_Click;
        // 
        // onlineButton
        // 
        onlineButton.Location = new Point(16, 274);
        onlineButton.Name = "onlineButton";
        onlineButton.Size = new Size(206, 44);
        onlineButton.TabIndex = 3;
        onlineButton.Text = "Đơn hàng online";
        onlineButton.Click += moduleButton_Click;
        // 
        // posButton
        // 
        posButton.Location = new Point(16, 220);
        posButton.Name = "posButton";
        posButton.Size = new Size(206, 44);
        posButton.TabIndex = 2;
        posButton.Text = "Bán hàng tại quầy";
        posButton.Click += moduleButton_Click;
        // 
        // productButton
        // 
        productButton.Location = new Point(16, 166);
        productButton.Name = "productButton";
        productButton.Size = new Size(206, 44);
        productButton.TabIndex = 1;
        productButton.Text = "Sản phẩm";
        productButton.Click += moduleButton_Click;
        // 
        // homeButton
        // 
        homeButton.Location = new Point(16, 112);
        homeButton.Name = "homeButton";
        homeButton.Size = new Size(206, 44);
        homeButton.TabIndex = 0;
        homeButton.Text = "Tổng quan";
        homeButton.Click += homeButton_Click;
        // 
        // userPanel
        // 
        userPanel.Controls.Add(userLabel);
        userPanel.Controls.Add(logoutButton);
        userPanel.Dock = DockStyle.Bottom;
        userPanel.Location = new Point(0, 784);
        userPanel.Name = "userPanel";
        userPanel.Padding = new Padding(16, 8, 16, 12);
        userPanel.Size = new Size(238, 116);
        userPanel.TabIndex = 7;
        // 
        // userLabel
        // 
        userLabel.Dock = DockStyle.Top;
        userLabel.ForeColor = Color.FromArgb(198, 210, 224);
        userLabel.Location = new Point(16, 8);
        userLabel.Name = "userLabel";
        userLabel.Size = new Size(206, 46);
        userLabel.TabIndex = 0;
        userLabel.Text = "Chưa đăng nhập";
        userLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // logoutButton
        // 
        logoutButton.Dock = DockStyle.Bottom;
        logoutButton.Location = new Point(16, 64);
        logoutButton.Name = "logoutButton";
        logoutButton.Size = new Size(206, 40);
        logoutButton.TabIndex = 1;
        logoutButton.Text = "Đăng xuất";
        logoutButton.Click += logoutButton_Click;
        // 
        // brandPanel
        // 
        brandPanel.Controls.Add(brandLabel);
        brandPanel.Dock = DockStyle.Top;
        brandPanel.Location = new Point(0, 0);
        brandPanel.Name = "brandPanel";
        brandPanel.Size = new Size(238, 104);
        brandPanel.TabIndex = 8;
        // 
        // brandLabel
        // 
        brandLabel.Dock = DockStyle.Fill;
        brandLabel.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        brandLabel.ForeColor = Color.White;
        brandLabel.Location = new Point(0, 0);
        brandLabel.Name = "brandLabel";
        brandLabel.Padding = new Padding(20, 0, 8, 0);
        brandLabel.Size = new Size(238, 104);
        brandLabel.TabIndex = 0;
        brandLabel.Text = "SHOESTORE";
        brandLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // mainPanel
        // 
        mainPanel.BackColor = Color.FromArgb(244, 247, 251);
        mainPanel.Controls.Add(pageHost);
        mainPanel.Controls.Add(dashboardPanel);
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Location = new Point(238, 0);
        mainPanel.Name = "mainPanel";
        mainPanel.Padding = new Padding(24);
        mainPanel.Size = new Size(1202, 900);
        mainPanel.TabIndex = 0;
        // 
        // pageHost
        // 
        pageHost.BackColor = Color.FromArgb(244, 247, 251);
        pageHost.Dock = DockStyle.Fill;
        pageHost.Location = new Point(24, 24);
        pageHost.Name = "pageHost";
        pageHost.Size = new Size(1154, 852);
        pageHost.TabIndex = 0;
        pageHost.Visible = false;
        // 
        // dashboardPanel
        // 
        dashboardPanel.Controls.Add(dashboardLayout);
        dashboardPanel.Dock = DockStyle.Fill;
        dashboardPanel.Location = new Point(24, 24);
        dashboardPanel.Name = "dashboardPanel";
        dashboardPanel.Size = new Size(1154, 852);
        dashboardPanel.TabIndex = 1;
        // 
        // dashboardLayout
        // 
        dashboardLayout.ColumnCount = 1;
        dashboardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        dashboardLayout.Controls.Add(headerPanel, 0, 0);
        dashboardLayout.Controls.Add(cardLayout, 0, 1);
        dashboardLayout.Controls.Add(ordersHeaderPanel, 0, 2);
        dashboardLayout.Controls.Add(recentOrdersGrid, 0, 3);
        dashboardLayout.Dock = DockStyle.Fill;
        dashboardLayout.Location = new Point(0, 0);
        dashboardLayout.Margin = new Padding(0);
        dashboardLayout.Name = "dashboardLayout";
        dashboardLayout.RowCount = 4;
        dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
        dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 132F));
        dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        dashboardLayout.Size = new Size(1154, 852);
        dashboardLayout.TabIndex = 0;
        // 
        // headerPanel
        // 
        headerPanel.Controls.Add(pageTitle);
        headerPanel.Controls.Add(subtitleLabel);
        headerPanel.Controls.Add(headerActions);
        headerPanel.Dock = DockStyle.Fill;
        headerPanel.Location = new Point(3, 3);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(1148, 88);
        headerPanel.TabIndex = 0;
        // 
        // pageTitle
        // 
        pageTitle.AutoSize = true;
        pageTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
        pageTitle.ForeColor = Color.FromArgb(34, 53, 76);
        pageTitle.Location = new Point(0, 0);
        pageTitle.Name = "pageTitle";
        pageTitle.Size = new Size(447, 54);
        pageTitle.TabIndex = 0;
        pageTitle.Text = "Tổng quan kinh doanh";
        // 
        // subtitleLabel
        // 
        subtitleLabel.AutoSize = true;
        subtitleLabel.ForeColor = Color.FromArgb(105, 120, 139);
        subtitleLabel.Location = new Point(3, 54);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Size = new Size(471, 21);
        subtitleLabel.TabIndex = 1;
        subtitleLabel.Text = "Theo dõi đơn hàng, doanh thu và tồn kho trên cùng một màn hình.";
        // 
        // headerActions
        // 
        headerActions.Controls.Add(dateLabel);
        headerActions.Controls.Add(refreshButton);
        headerActions.Dock = DockStyle.Right;
        headerActions.Location = new Point(898, 0);
        headerActions.Name = "headerActions";
        headerActions.Size = new Size(250, 88);
        headerActions.TabIndex = 2;
        // 
        // dateLabel
        // 
        dateLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        dateLabel.Location = new Point(0, 6);
        dateLabel.Name = "dateLabel";
        dateLabel.Size = new Size(250, 30);
        dateLabel.TabIndex = 0;
        dateLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // refreshButton
        // 
        refreshButton.Location = new Point(132, 44);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(118, 36);
        refreshButton.TabIndex = 1;
        refreshButton.Text = "Làm mới";
        refreshButton.Click += refreshButton_Click;
        // 
        // cardLayout
        // 
        cardLayout.ColumnCount = 4;
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        cardLayout.Controls.Add(cardSales, 0, 0);
        cardLayout.Controls.Add(cardOrders, 1, 0);
        cardLayout.Controls.Add(cardStock, 2, 0);
        cardLayout.Controls.Add(cardProducts, 3, 0);
        cardLayout.Dock = DockStyle.Fill;
        cardLayout.Location = new Point(0, 94);
        cardLayout.Margin = new Padding(0);
        cardLayout.Name = "cardLayout";
        cardLayout.RowCount = 1;
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        cardLayout.Size = new Size(1154, 132);
        cardLayout.TabIndex = 1;
        // 
        // cardSales
        // 
        cardSales.BackColor = Color.White;
        cardSales.BorderStyle = BorderStyle.FixedSingle;
        cardSales.Controls.Add(salesTitle);
        cardSales.Controls.Add(salesValue);
        cardSales.Dock = DockStyle.Fill;
        cardSales.Location = new Point(8, 4);
        cardSales.Margin = new Padding(8, 4, 8, 4);
        cardSales.Name = "cardSales";
        cardSales.Size = new Size(272, 124);
        cardSales.TabIndex = 0;
        // 
        // salesTitle
        // 
        salesTitle.AutoSize = true;
        salesTitle.ForeColor = Color.FromArgb(105, 120, 139);
        salesTitle.Location = new Point(16, 14);
        salesTitle.Name = "salesTitle";
        salesTitle.Size = new Size(175, 21);
        salesTitle.TabIndex = 0;
        salesTitle.Text = "DOANH THU HÔM NAY";
        // 
        // salesValue
        // 
        salesValue.AutoSize = true;
        salesValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        salesValue.ForeColor = Color.FromArgb(32, 105, 82);
        salesValue.Location = new Point(16, 48);
        salesValue.Name = "salesValue";
        salesValue.Size = new Size(54, 46);
        salesValue.TabIndex = 1;
        salesValue.Text = "—";
        // 
        // cardOrders
        // 
        cardOrders.BackColor = Color.White;
        cardOrders.BorderStyle = BorderStyle.FixedSingle;
        cardOrders.Controls.Add(ordersTitle);
        cardOrders.Controls.Add(ordersValue);
        cardOrders.Dock = DockStyle.Fill;
        cardOrders.Location = new Point(296, 4);
        cardOrders.Margin = new Padding(8, 4, 8, 4);
        cardOrders.Name = "cardOrders";
        cardOrders.Size = new Size(272, 124);
        cardOrders.TabIndex = 1;
        // 
        // ordersTitle
        // 
        ordersTitle.AutoSize = true;
        ordersTitle.ForeColor = Color.FromArgb(105, 120, 139);
        ordersTitle.Location = new Point(16, 14);
        ordersTitle.Name = "ordersTitle";
        ordersTitle.Size = new Size(137, 21);
        ordersTitle.TabIndex = 0;
        ordersTitle.Text = "ĐƠN ĐANG XỬ LÝ";
        // 
        // ordersValue
        // 
        ordersValue.AutoSize = true;
        ordersValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        ordersValue.ForeColor = Color.FromArgb(47, 104, 163);
        ordersValue.Location = new Point(16, 48);
        ordersValue.Name = "ordersValue";
        ordersValue.Size = new Size(54, 46);
        ordersValue.TabIndex = 1;
        ordersValue.Text = "—";
        // 
        // cardStock
        // 
        cardStock.BackColor = Color.White;
        cardStock.BorderStyle = BorderStyle.FixedSingle;
        cardStock.Controls.Add(stockTitle);
        cardStock.Controls.Add(stockValue);
        cardStock.Dock = DockStyle.Fill;
        cardStock.Location = new Point(584, 4);
        cardStock.Margin = new Padding(8, 4, 8, 4);
        cardStock.Name = "cardStock";
        cardStock.Size = new Size(272, 124);
        cardStock.TabIndex = 2;
        // 
        // stockTitle
        // 
        stockTitle.AutoSize = true;
        stockTitle.ForeColor = Color.FromArgb(105, 120, 139);
        stockTitle.Location = new Point(16, 14);
        stockTitle.Name = "stockTitle";
        stockTitle.Size = new Size(117, 21);
        stockTitle.TabIndex = 0;
        stockTitle.Text = "SẮP HẾT HÀNG";
        // 
        // stockValue
        // 
        stockValue.AutoSize = true;
        stockValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        stockValue.ForeColor = Color.FromArgb(190, 115, 32);
        stockValue.Location = new Point(16, 48);
        stockValue.Name = "stockValue";
        stockValue.Size = new Size(54, 46);
        stockValue.TabIndex = 1;
        stockValue.Text = "—";
        // 
        // cardProducts
        // 
        cardProducts.BackColor = Color.White;
        cardProducts.BorderStyle = BorderStyle.FixedSingle;
        cardProducts.Controls.Add(productsTitle);
        cardProducts.Controls.Add(productsValue);
        cardProducts.Dock = DockStyle.Fill;
        cardProducts.Location = new Point(872, 4);
        cardProducts.Margin = new Padding(8, 4, 8, 4);
        cardProducts.Name = "cardProducts";
        cardProducts.Size = new Size(274, 124);
        cardProducts.TabIndex = 3;
        // 
        // productsTitle
        // 
        productsTitle.AutoSize = true;
        productsTitle.ForeColor = Color.FromArgb(105, 120, 139);
        productsTitle.Location = new Point(16, 14);
        productsTitle.Name = "productsTitle";
        productsTitle.Size = new Size(165, 21);
        productsTitle.TabIndex = 0;
        productsTitle.Text = "MẪU GIÀY ĐANG BÁN";
        // 
        // productsValue
        // 
        productsValue.AutoSize = true;
        productsValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        productsValue.ForeColor = Color.FromArgb(34, 53, 76);
        productsValue.Location = new Point(16, 48);
        productsValue.Name = "productsValue";
        productsValue.Size = new Size(54, 46);
        productsValue.TabIndex = 1;
        productsValue.Text = "—";
        // 
        // ordersHeaderPanel
        // 
        ordersHeaderPanel.Controls.Add(ordersHeading);
        ordersHeaderPanel.Controls.Add(connectionLabel);
        ordersHeaderPanel.Dock = DockStyle.Fill;
        ordersHeaderPanel.Location = new Point(3, 229);
        ordersHeaderPanel.Name = "ordersHeaderPanel";
        ordersHeaderPanel.Size = new Size(1148, 48);
        ordersHeaderPanel.TabIndex = 2;
        // 
        // ordersHeading
        // 
        ordersHeading.AutoSize = true;
        ordersHeading.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        ordersHeading.Location = new Point(0, 8);
        ordersHeading.Name = "ordersHeading";
        ordersHeading.Size = new Size(242, 35);
        ordersHeading.TabIndex = 0;
        ordersHeading.Text = "Đơn hàng mới nhất";
        // 
        // connectionLabel
        // 
        connectionLabel.Dock = DockStyle.Right;
        connectionLabel.ForeColor = Color.FromArgb(105, 120, 139);
        connectionLabel.Location = new Point(628, 0);
        connectionLabel.Name = "connectionLabel";
        connectionLabel.Size = new Size(520, 48);
        connectionLabel.TabIndex = 1;
        connectionLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // recentOrdersGrid
        // 
        recentOrdersGrid.AllowUserToAddRows = false;
        recentOrdersGrid.AllowUserToDeleteRows = false;
        recentOrdersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        recentOrdersGrid.BackgroundColor = Color.White;
        recentOrdersGrid.ColumnHeadersHeight = 36;
        recentOrdersGrid.Dock = DockStyle.Fill;
        recentOrdersGrid.Location = new Point(0, 280);
        recentOrdersGrid.Margin = new Padding(0);
        recentOrdersGrid.MultiSelect = false;
        recentOrdersGrid.Name = "recentOrdersGrid";
        recentOrdersGrid.ReadOnly = true;
        recentOrdersGrid.RowHeadersVisible = false;
        recentOrdersGrid.RowHeadersWidth = 51;
        recentOrdersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        recentOrdersGrid.Size = new Size(1154, 572);
        recentOrdersGrid.TabIndex = 3;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(9F, 21F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(244, 247, 251);
        ClientSize = new Size(1440, 900);
        Controls.Add(mainPanel);
        Controls.Add(sidebar);
        Font = new Font("Segoe UI", 9.5F);
        MinimumSize = new Size(1120, 720);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "ShoeStore | Quản lý bán hàng đa kênh";
        WindowState = FormWindowState.Maximized;
        sidebar.ResumeLayout(false);
        userPanel.ResumeLayout(false);
        brandPanel.ResumeLayout(false);
        mainPanel.ResumeLayout(false);
        dashboardPanel.ResumeLayout(false);
        dashboardLayout.ResumeLayout(false);
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        headerActions.ResumeLayout(false);
        cardLayout.ResumeLayout(false);
        cardSales.ResumeLayout(false);
        cardSales.PerformLayout();
        cardOrders.ResumeLayout(false);
        cardOrders.PerformLayout();
        cardStock.ResumeLayout(false);
        cardStock.PerformLayout();
        cardProducts.ResumeLayout(false);
        cardProducts.PerformLayout();
        ordersHeaderPanel.ResumeLayout(false);
        ordersHeaderPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)recentOrdersGrid).EndInit();
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
}
