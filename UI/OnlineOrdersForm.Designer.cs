#nullable enable
namespace QuanLyBanGiay;

partial class OnlineOrdersForm
{
    private System.ComponentModel.IContainer? components;
    private DataGridView ordersGrid = null!;
    private TextBox customerName = null!;
    private TextBox phone = null!;
    private TextBox address = null!;
    private ComboBox variant = null!;
    private NumericUpDown quantity = null!;
    private ComboBox status = null!;
    private TextBox note = null!;
    private Label statusLabel = null!;
    private Label title = null!;
    private Label variantLabel = null!;
    private Label quantityLabel = null!;
    private Label statusLabelField = null!;
    private Button create = null!;
    private Button saveStatus = null!;
    private Button reload = null!;
    private GroupBox createBox = null!;
    private GroupBox stateBox = null!;
    private TableLayoutPanel layout = null!;
    private FlowLayoutPanel filterPanel = null!;
    private TextBox orderSearch = null!;
    private ComboBox statusFilter = null!;
    private Button filterOrders = null!;
    private Button clearFilters = null!;
    private Panel createLayout = null!;
    private Panel stateLayout = null!;
    private TabControl detailsTabs = null!;
    private TabPage processTab = null!;
    private TabPage assignmentTab = null!;
    private TabPage historyTab = null!;
    private ComboBox assignee = null!;
    private Button assignButton = null!;
    private Label assignmentInfo = null!;
    private DataGridView historyGrid = null!;
    private TableLayoutPanel bottom = null!;
    private FlowLayoutPanel stateActions = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        ordersGrid = new DataGridView();
        customerName = new TextBox();
        phone = new TextBox();
        address = new TextBox();
        variant = new ComboBox();
        quantity = new NumericUpDown();
        status = new ComboBox();
        note = new TextBox();
        statusLabel = new Label();
        title = new Label();
        variantLabel = new Label();
        quantityLabel = new Label();
        statusLabelField = new Label();
        create = new Button();
        saveStatus = new Button();
        reload = new Button();
        createBox = new GroupBox();
        createLayout = new Panel();
        stateBox = new GroupBox();
        stateLayout = new Panel();
        detailsTabs = new TabControl();
        processTab = new TabPage();
        stateActions = new FlowLayoutPanel();
        assignmentTab = new TabPage();
        assignmentInfo = new Label();
        assignee = new ComboBox();
        assignButton = new Button();
        historyTab = new TabPage();
        historyGrid = new DataGridView();
        layout = new TableLayoutPanel();
        filterPanel = new FlowLayoutPanel();
        orderSearch = new TextBox();
        statusFilter = new ComboBox();
        filterOrders = new Button();
        clearFilters = new Button();
        bottom = new TableLayoutPanel();
        ((System.ComponentModel.ISupportInitialize)ordersGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)quantity).BeginInit();
        createBox.SuspendLayout();
        createLayout.SuspendLayout();
        stateBox.SuspendLayout();
        stateLayout.SuspendLayout();
        detailsTabs.SuspendLayout();
        processTab.SuspendLayout();
        stateActions.SuspendLayout();
        assignmentTab.SuspendLayout();
        historyTab.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)historyGrid).BeginInit();
        layout.SuspendLayout();
        filterPanel.SuspendLayout();
        bottom.SuspendLayout();
        SuspendLayout();
        // 
        // ordersGrid
        // 
        ordersGrid.AllowUserToAddRows = false;
        ordersGrid.AllowUserToDeleteRows = false;
        ordersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        ordersGrid.BackgroundColor = Color.White;
        ordersGrid.ColumnHeadersHeight = 29;
        ordersGrid.Dock = DockStyle.Fill;
        ordersGrid.Location = new Point(27, 121);
        ordersGrid.MultiSelect = false;
        ordersGrid.Name = "ordersGrid";
        ordersGrid.ReadOnly = true;
        ordersGrid.RowHeadersVisible = false;
        ordersGrid.RowHeadersWidth = 51;
        ordersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        ordersGrid.Size = new Size(1186, 378);
        ordersGrid.TabIndex = 1;
        ordersGrid.CellFormatting += ordersGrid_CellFormatting;
        ordersGrid.SelectionChanged += ordersGrid_SelectionChanged;
        // 
        // customerName
        // 
        customerName.Location = new Point(3, 2);
        customerName.Margin = new Padding(3, 2, 6, 2);
        customerName.MaxLength = 100;
        customerName.Name = "customerName";
        customerName.PlaceholderText = "Tên khách hàng";
        customerName.Size = new Size(346, 29);
        customerName.TabIndex = 0;
        // 
        // phone
        // 
        phone.Location = new Point(361, 2);
        phone.Margin = new Padding(6, 2, 3, 2);
        phone.MaxLength = 15;
        phone.Name = "phone";
        phone.PlaceholderText = "Số điện thoại";
        phone.Size = new Size(347, 29);
        phone.TabIndex = 1;
        // 
        // address
        // 
        address.Location = new Point(3, 38);
        address.Margin = new Padding(3, 2, 3, 2);
        address.MaxLength = 300;
        address.Name = "address";
        address.PlaceholderText = "Địa chỉ giao hàng";
        address.Size = new Size(705, 29);
        address.TabIndex = 2;
        // 
        // variant
        // 
        variant.DropDownStyle = ComboBoxStyle.DropDownList;
        variant.Location = new Point(3, 96);
        variant.Margin = new Padding(3, 2, 6, 2);
        variant.Name = "variant";
        variant.Size = new Size(346, 29);
        variant.TabIndex = 5;
        // 
        // quantity
        // 
        quantity.Location = new Point(361, 96);
        quantity.Margin = new Padding(6, 2, 3, 2);
        quantity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        quantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        quantity.Name = "quantity";
        quantity.Size = new Size(347, 29);
        quantity.TabIndex = 6;
        quantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // status
        // 
        status.DropDownStyle = ComboBoxStyle.DropDownList;
        status.Items.AddRange(new object[] { "Đã xác nhận", "Đang chuẩn bị", "Đang giao", "Hoàn tất", "Đã hủy", "Thất bại" });
        status.Location = new Point(6, 23);
        status.Margin = new Padding(0, 2, 0, 3);
        status.Name = "status";
        status.Size = new Size(390, 29);
        status.TabIndex = 1;
        // 
        // note
        // 
        note.Location = new Point(6, 53);
        note.Margin = new Padding(0, 3, 0, 3);
        note.MaxLength = 300;
        note.Multiline = true;
        note.Name = "note";
        note.PlaceholderText = "Ghi chú xử lý";
        note.Size = new Size(390, 42);
        note.TabIndex = 2;
        // 
        // statusLabel
        // 
        statusLabel.AutoSize = true;
        statusLabel.Dock = DockStyle.Fill;
        statusLabel.ForeColor = Color.FromArgb(105, 120, 139);
        statusLabel.Location = new Point(27, 736);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1186, 30);
        statusLabel.TabIndex = 3;
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // title
        // 
        title.Dock = DockStyle.Fill;
        title.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        title.ForeColor = Color.FromArgb(40, 91, 145);
        title.Location = new Point(27, 18);
        title.Name = "title";
        title.Size = new Size(1186, 58);
        title.TabIndex = 0;
        title.Text = "TIẾP NHẬN VÀ XỬ LÝ ĐƠN ONLINE";
        title.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // variantLabel
        // 
        variantLabel.AutoSize = true;
        variantLabel.Location = new Point(3, 72);
        variantLabel.Name = "variantLabel";
        variantLabel.Size = new Size(150, 21);
        variantLabel.TabIndex = 3;
        variantLabel.Text = "Sản phẩm / biến thể";
        variantLabel.TextAlign = ContentAlignment.BottomLeft;
        // 
        // quantityLabel
        // 
        quantityLabel.AutoSize = true;
        quantityLabel.Location = new Point(358, 72);
        quantityLabel.Name = "quantityLabel";
        quantityLabel.Size = new Size(73, 21);
        quantityLabel.TabIndex = 4;
        quantityLabel.Text = "Số lượng";
        quantityLabel.TextAlign = ContentAlignment.BottomLeft;
        // 
        // statusLabelField
        // 
        statusLabelField.AutoSize = true;
        statusLabelField.Location = new Point(6, 4);
        statusLabelField.Name = "statusLabelField";
        statusLabelField.Size = new Size(111, 21);
        statusLabelField.TabIndex = 0;
        statusLabelField.Text = "Trạng thái đơn";
        statusLabelField.TextAlign = ContentAlignment.BottomLeft;
        // 
        // create
        // 
        create.Location = new Point(3, 134);
        create.Margin = new Padding(3, 4, 0, 0);
        create.Name = "create";
        create.Size = new Size(145, 34);
        create.TabIndex = 7;
        create.Text = "Tạo đơn chờ";
        create.Click += create_Click;
        // 
        // saveStatus
        // 
        saveStatus.Location = new Point(199, 0);
        saveStatus.Margin = new Padding(0);
        saveStatus.Name = "saveStatus";
        saveStatus.Size = new Size(130, 27);
        saveStatus.TabIndex = 1;
        saveStatus.Text = "Lưu trạng thái";
        saveStatus.Click += saveStatus_Click;
        // 
        // reload
        // 
        reload.Location = new Point(329, 0);
        reload.Margin = new Padding(0, 0, 6, 0);
        reload.Name = "reload";
        reload.Size = new Size(90, 27);
        reload.TabIndex = 0;
        reload.Text = "Tải lại";
        reload.Click += reload_Click;
        // 
        // createBox
        // 
        createBox.Controls.Add(createLayout);
        createBox.Dock = DockStyle.Fill;
        createBox.Location = new Point(0, 0);
        createBox.Margin = new Padding(0, 0, 8, 0);
        createBox.Name = "createBox";
        createBox.Padding = new Padding(10);
        createBox.Size = new Size(731, 216);
        createBox.TabIndex = 0;
        createBox.TabStop = false;
        createBox.Text = "Thông tin đơn hàng";
        // 
        // createLayout
        // 
        createLayout.Controls.Add(customerName);
        createLayout.Controls.Add(phone);
        createLayout.Controls.Add(address);
        createLayout.Controls.Add(variantLabel);
        createLayout.Controls.Add(quantityLabel);
        createLayout.Controls.Add(variant);
        createLayout.Controls.Add(quantity);
        createLayout.Controls.Add(create);
        createLayout.Dock = DockStyle.Fill;
        createLayout.Location = new Point(10, 32);
        createLayout.Name = "createLayout";
        createLayout.Size = new Size(711, 174);
        createLayout.TabIndex = 0;
        // 
        // stateBox
        // 
        stateBox.Controls.Add(stateLayout);
        stateBox.Dock = DockStyle.Fill;
        stateBox.Location = new Point(747, 0);
        stateBox.Margin = new Padding(8, 0, 0, 0);
        stateBox.Name = "stateBox";
        stateBox.Padding = new Padding(10);
        stateBox.Size = new Size(445, 216);
        stateBox.TabIndex = 1;
        stateBox.TabStop = false;
        stateBox.Text = "Trạng thái · phân công · lịch sử";
        // 
        // stateLayout
        // 
        stateLayout.Controls.Add(detailsTabs);
        stateLayout.Dock = DockStyle.Fill;
        stateLayout.Location = new Point(10, 32);
        stateLayout.Name = "stateLayout";
        stateLayout.Size = new Size(425, 174);
        stateLayout.TabIndex = 0;
        // 
        // detailsTabs
        // 
        detailsTabs.Controls.Add(processTab);
        detailsTabs.Controls.Add(assignmentTab);
        detailsTabs.Controls.Add(historyTab);
        detailsTabs.Dock = DockStyle.Fill;
        detailsTabs.Location = new Point(0, 0);
        detailsTabs.Name = "detailsTabs";
        detailsTabs.SelectedIndex = 0;
        detailsTabs.Size = new Size(425, 174);
        detailsTabs.TabIndex = 0;
        detailsTabs.SelectedIndexChanged += detailsTabs_SelectedIndexChanged;
        // 
        // processTab
        // 
        processTab.Controls.Add(statusLabelField);
        processTab.Controls.Add(status);
        processTab.Controls.Add(note);
        processTab.Controls.Add(stateActions);
        processTab.Location = new Point(4, 30);
        processTab.Name = "processTab";
        processTab.Padding = new Padding(6);
        processTab.Size = new Size(417, 140);
        processTab.TabIndex = 0;
        processTab.Text = "Xử lý";
        // 
        // stateActions
        // 
        stateActions.Controls.Add(reload);
        stateActions.Controls.Add(saveStatus);
        stateActions.FlowDirection = FlowDirection.RightToLeft;
        stateActions.Location = new Point(0, 107);
        stateActions.Margin = new Padding(0, 3, 0, 0);
        stateActions.Name = "stateActions";
        stateActions.Size = new Size(425, 37);
        stateActions.TabIndex = 3;
        stateActions.WrapContents = false;
        // 
        // assignmentTab
        // 
        assignmentTab.Controls.Add(assignmentInfo);
        assignmentTab.Controls.Add(assignee);
        assignmentTab.Controls.Add(assignButton);
        assignmentTab.Location = new Point(4, 30);
        assignmentTab.Name = "assignmentTab";
        assignmentTab.Padding = new Padding(8);
        assignmentTab.Size = new Size(417, 140);
        assignmentTab.TabIndex = 1;
        assignmentTab.Text = "Phân công";
        // 
        // assignmentInfo
        // 
        assignmentInfo.Location = new Point(8, 8);
        assignmentInfo.Name = "assignmentInfo";
        assignmentInfo.Size = new Size(380, 32);
        assignmentInfo.TabIndex = 0;
        assignmentInfo.Text = "Chọn nhân viên phụ trách đơn đang chọn.";
        // 
        // assignee
        // 
        assignee.DropDownStyle = ComboBoxStyle.DropDownList;
        assignee.Location = new Point(8, 44);
        assignee.Name = "assignee";
        assignee.Size = new Size(380, 29);
        assignee.TabIndex = 1;
        // 
        // assignButton
        // 
        assignButton.Location = new Point(8, 82);
        assignButton.Name = "assignButton";
        assignButton.Size = new Size(150, 34);
        assignButton.TabIndex = 2;
        assignButton.Text = "Giao cho nhân viên";
        assignButton.Click += assignButton_Click;
        // 
        // historyTab
        // 
        historyTab.Controls.Add(historyGrid);
        historyTab.Location = new Point(4, 30);
        historyTab.Name = "historyTab";
        historyTab.Padding = new Padding(4);
        historyTab.Size = new Size(417, 140);
        historyTab.TabIndex = 2;
        historyTab.Text = "Lịch sử";
        // 
        // historyGrid
        // 
        historyGrid.AllowUserToAddRows = false;
        historyGrid.AllowUserToDeleteRows = false;
        historyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        historyGrid.BackgroundColor = Color.White;
        historyGrid.ColumnHeadersHeight = 29;
        historyGrid.Dock = DockStyle.Fill;
        historyGrid.Location = new Point(4, 4);
        historyGrid.MultiSelect = false;
        historyGrid.Name = "historyGrid";
        historyGrid.ReadOnly = true;
        historyGrid.RowHeadersVisible = false;
        historyGrid.RowHeadersWidth = 51;
        historyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        historyGrid.Size = new Size(409, 132);
        historyGrid.TabIndex = 0;
        // 
        // layout
        // 
        layout.ColumnCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(filterPanel, 0, 1);
        layout.Controls.Add(ordersGrid, 0, 2);
        layout.Controls.Add(bottom, 0, 3);
        layout.Controls.Add(statusLabel, 0, 4);
        layout.Dock = DockStyle.Fill;
        layout.Location = new Point(0, 0);
        layout.Name = "layout";
        layout.Padding = new Padding(24, 18, 24, 14);
        layout.RowCount = 5;
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 234F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        layout.Size = new Size(1240, 780);
        layout.TabIndex = 0;
        // 
        // filterPanel
        // 
        filterPanel.Controls.Add(orderSearch);
        filterPanel.Controls.Add(statusFilter);
        filterPanel.Controls.Add(filterOrders);
        filterPanel.Controls.Add(clearFilters);
        filterPanel.Dock = DockStyle.Fill;
        filterPanel.Location = new Point(24, 76);
        filterPanel.Margin = new Padding(0, 0, 0, 4);
        filterPanel.Name = "filterPanel";
        filterPanel.Size = new Size(1192, 38);
        filterPanel.TabIndex = 1;
        filterPanel.WrapContents = false;
        // 
        // orderSearch
        // 
        orderSearch.Location = new Point(0, 2);
        orderSearch.Margin = new Padding(0, 2, 8, 0);
        orderSearch.Name = "orderSearch";
        orderSearch.PlaceholderText = "Tìm mã đơn, khách hàng, điện thoại";
        orderSearch.Size = new Size(360, 29);
        orderSearch.TabIndex = 0;
        orderSearch.KeyDown += orderSearch_KeyDown;
        // 
        // statusFilter
        // 
        statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        statusFilter.Items.AddRange(new object[] { "Tất cả trạng thái", "Chờ xác nhận", "Đã xác nhận", "Đang chuẩn bị", "Đang giao", "Hoàn tất", "Đã hủy", "Thất bại" });
        statusFilter.Location = new Point(368, 2);
        statusFilter.Margin = new Padding(0, 2, 8, 0);
        statusFilter.Name = "statusFilter";
        statusFilter.Size = new Size(180, 29);
        statusFilter.TabIndex = 1;
        // 
        // filterOrders
        // 
        filterOrders.Location = new Point(556, 0);
        filterOrders.Margin = new Padding(0, 0, 8, 0);
        filterOrders.Name = "filterOrders";
        filterOrders.Size = new Size(90, 32);
        filterOrders.TabIndex = 2;
        filterOrders.Text = "Lọc đơn";
        filterOrders.Click += filterOrders_Click;
        // 
        // clearFilters
        // 
        clearFilters.Location = new Point(657, 3);
        clearFilters.Name = "clearFilters";
        clearFilters.Size = new Size(96, 32);
        clearFilters.TabIndex = 3;
        clearFilters.Text = "Xóa lọc";
        clearFilters.Click += clearFilters_Click;
        // 
        // bottom
        // 
        bottom.ColumnCount = 2;
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
        bottom.Controls.Add(createBox, 0, 0);
        bottom.Controls.Add(stateBox, 1, 0);
        bottom.Dock = DockStyle.Fill;
        bottom.Location = new Point(24, 512);
        bottom.Margin = new Padding(0, 10, 0, 8);
        bottom.Name = "bottom";
        bottom.RowCount = 1;
        bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        bottom.Size = new Size(1192, 216);
        bottom.TabIndex = 2;
        // 
        // OnlineOrdersForm
        // 
        AutoScaleDimensions = new SizeF(9F, 21F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(244, 247, 251);
        ClientSize = new Size(1240, 780);
        Controls.Add(layout);
        Font = new Font("Segoe UI", 9.5F);
        MinimumSize = new Size(1040, 680);
        Name = "OnlineOrdersForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Đơn hàng online";
        WindowState = FormWindowState.Maximized;
        ((System.ComponentModel.ISupportInitialize)ordersGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)quantity).EndInit();
        createBox.ResumeLayout(false);
        createLayout.ResumeLayout(false);
        createLayout.PerformLayout();
        stateBox.ResumeLayout(false);
        stateLayout.ResumeLayout(false);
        detailsTabs.ResumeLayout(false);
        processTab.ResumeLayout(false);
        processTab.PerformLayout();
        stateActions.ResumeLayout(false);
        assignmentTab.ResumeLayout(false);
        historyTab.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)historyGrid).EndInit();
        layout.ResumeLayout(false);
        layout.PerformLayout();
        filterPanel.ResumeLayout(false);
        filterPanel.PerformLayout();
        bottom.ResumeLayout(false);
        ResumeLayout(false);
    }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
