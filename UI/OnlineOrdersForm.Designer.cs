#nullable enable
namespace ShoeStore.WinForms.UI;

partial class OnlineOrdersForm
{
    private System.ComponentModel.IContainer? components;
    private DataGridView ordersGrid = null!;
    private DataGridView historyGrid = null!;
    private TextBox customerName = null!;
    private TextBox phone = null!;
    private TextBox address = null!;
    private TextBox searchBox = null!;
    private ComboBox variant = null!;
    private NumericUpDown quantity = null!;
    private ComboBox status = null!;
    private ComboBox statusFilter = null!;
    private ComboBox assignee = null!;
    private TextBox note = null!;
    private Label statusLabel = null!;
    private Label emptyOrdersLabel = null!;
    private Button assignButton = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        ordersGrid = Grid(); historyGrid = Grid(); customerName = new TextBox(); phone = new TextBox(); address = new TextBox(); searchBox = new TextBox();
        variant = new ComboBox(); quantity = new NumericUpDown(); status = new ComboBox(); statusFilter = new ComboBox(); assignee = new ComboBox(); note = new TextBox(); statusLabel = new Label();
        var title = new Label(); var search = new Button(); var refresh = new Button(); var create = new Button(); var saveStatus = new Button(); assignButton = new Button();
        ((System.ComponentModel.ISupportInitialize)quantity).BeginInit(); SuspendLayout();
        Text = "Đơn hàng online"; StartPosition = FormStartPosition.CenterScreen; WindowState = FormWindowState.Maximized; MinimumSize = new Size(1050, 700); Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(245, 247, 250);
        title.Text = "ĐƠN HÀNG ONLINE"; title.Dock = DockStyle.Top; title.Height = 52; title.Padding = new Padding(20, 7, 0, 0); title.Font = new Font("Segoe UI", 20F, FontStyle.Bold); title.ForeColor = Color.FromArgb(39, 91, 145);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20, 0, 20, 15), ColumnCount = 1, RowCount = 4 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 56)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 44)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
        searchBox.Width = 280; searchBox.PlaceholderText = "Tìm mã đơn, khách hàng, điện thoại";
        statusFilter.Width = 170; statusFilter.DropDownStyle = ComboBoxStyle.DropDownList; statusFilter.Items.AddRange(new object[] { "Tất cả trạng thái", "PENDING", "CONFIRMED", "PREPARING", "SHIPPING", "COMPLETED", "CANCELLED", "FAILED" }); statusFilter.SelectedIndex = 0;
        StyleButton(search, "Tìm kiếm"); StyleButton(refresh, "Tải lại"); search.Click += search_Click; refresh.Click += reload_Click;
        toolbar.Controls.AddRange(new Control[] { searchBox, statusFilter, search, refresh });
        var split = new SplitContainer { Size = new Size(1200, 600), Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, Panel1MinSize = 130, Panel2MinSize = 120, SplitterDistance = 330, SplitterWidth = 8 };
        ordersGrid.Dock = DockStyle.Fill; emptyOrdersLabel = new Label { Text = "Chưa có dữ liệu đơn hàng\u000aKết nối Oracle và tải lại để xem danh sách.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(105, 117, 132), BackColor = Color.White, Font = new Font("Segoe UI", 12F) }; var orderPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White }; orderPanel.Controls.Add(ordersGrid); orderPanel.Controls.Add(emptyOrdersLabel); split.Panel1.Controls.Add(orderPanel);
        var historyPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(0, 3, 0, 0) }; historyPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28)); historyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        historyPanel.Controls.Add(new Label { Text = "LỊCH SỬ XỬ LÝ ĐƠN ĐANG CHỌN", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(39, 91, 145) }, 0, 0); historyGrid.Dock = DockStyle.Fill; historyPanel.Controls.Add(historyGrid, 0, 1); split.Panel2.Controls.Add(historyPanel);
        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(0, 8, 0, 0) }; bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52)); bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
        var createBox = new GroupBox { Text = "Tiếp nhận đơn online", Dock = DockStyle.Fill, Padding = new Padding(12) }; var createLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 3 }; createLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); createLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); createLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); createLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125)); createLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); createLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); createLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        customerName.PlaceholderText = "Tên khách hàng"; phone.PlaceholderText = "Số điện thoại"; address.PlaceholderText = "Địa chỉ giao hàng"; variant.DropDownStyle = ComboBoxStyle.DropDownList; quantity.Minimum = 1; quantity.Maximum = 100000;
        createLayout.Controls.Add(customerName, 0, 0); createLayout.SetColumnSpan(customerName, 2); createLayout.Controls.Add(phone, 2, 0); createLayout.SetColumnSpan(phone, 2); createLayout.Controls.Add(address, 0, 1); createLayout.SetColumnSpan(address, 2); createLayout.Controls.Add(variant, 2, 1); createLayout.SetColumnSpan(variant, 2); createLayout.Controls.Add(quantity, 0, 2); StyleButton(create, "Tạo đơn chờ"); create.Dock = DockStyle.Fill; create.Click += create_Click; createLayout.Controls.Add(create, 3, 2); createBox.Controls.Add(createLayout);
        var processBox = new GroupBox { Text = "Phân công và xử lý", Dock = DockStyle.Fill, Padding = new Padding(12) }; var processLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 }; processLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44)); processLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28)); processLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28)); processLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); processLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); processLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        assignee.DropDownStyle = ComboBoxStyle.DropDownList; status.DropDownStyle = ComboBoxStyle.DropDownList; status.Items.AddRange(new object[] { "CONFIRMED", "PREPARING", "SHIPPING", "COMPLETED", "CANCELLED", "FAILED" }); note.PlaceholderText = "Ghi chú xử lý";
        assignee.Dock = DockStyle.Fill; status.Dock = DockStyle.Fill; note.Dock = DockStyle.Fill; note.Multiline = true; processLayout.Controls.Add(assignee, 0, 0); StyleButton(assignButton, "Giao đơn"); assignButton.Dock = DockStyle.Fill; assignButton.Click += assignButton_Click; processLayout.Controls.Add(assignButton, 1, 0); processLayout.Controls.Add(status, 2, 0); processLayout.Controls.Add(note, 0, 1); processLayout.SetColumnSpan(note, 3); StyleButton(saveStatus, "Lưu trạng thái"); saveStatus.Dock = DockStyle.Fill; saveStatus.Click += saveStatus_Click; processLayout.Controls.Add(saveStatus, 2, 2); processBox.Controls.Add(processLayout);
        bottom.Controls.Add(createBox, 0, 0); bottom.Controls.Add(processBox, 1, 0);
        statusLabel.Dock = DockStyle.Fill; statusLabel.ForeColor = Color.FromArgb(80, 90, 105); statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        root.Controls.Add(toolbar, 0, 0); root.Controls.Add(split, 0, 1); root.Controls.Add(bottom, 0, 2); root.Controls.Add(statusLabel, 0, 3);
        Controls.Add(root); Controls.Add(title); ((System.ComponentModel.ISupportInitialize)quantity).EndInit(); ResumeLayout(false);
    }
    private static DataGridView Grid() => new() { ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill };
    private static void StyleButton(Button b, string text) { b.Text = text; b.AutoSize = false; b.Size = new Size(112, 30); b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0; b.BackColor = Color.FromArgb(39, 91, 145); b.ForeColor = Color.White; b.Margin = new Padding(5, 2, 5, 2); }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}




