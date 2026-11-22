using System.Data;
using System.Runtime.Versioning;
using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class MainForm : Form
{
    private UserSession? _session;
    private Form? _activePage;

    public MainForm()
    {
        InitializeComponent();
        ConfigureDashboardStyle();
        dateLabel.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy");
        recentOrdersGrid.DataSource = EmptyRecentOrders();
        ConfigureRecentOrdersGrid();
        dashboardPanel.Visible = true;
    }

    internal MainForm(UserSession session) : this()
    {
        _session = session;
        userLabel.Text = $"{session.Username}\r\n{RoleName(session.RoleCode)}";
        productButton.Enabled = true;
        inventoryButton.Enabled = session.IsManager;
        posButton.Enabled = session.IsManager;
        onlineButton.Enabled = true;
        reportButton.Enabled = session.IsManager;
        accountButton.Visible = session.IsManager;
        accountButton.Text = "Khách hàng & nhân viên";
        RefreshDashboard();
    }

    private void ConfigureDashboardStyle()
    {
        UiTheme.Apply(this);
        sidebar.BackColor = Color.FromArgb(24, 39, 58);
        brandPanel.BackColor = sidebar.BackColor;
        userPanel.BackColor = sidebar.BackColor;
        mainPanel.BackColor = Color.FromArgb(244, 247, 251);
        dashboardPanel.BackColor = mainPanel.BackColor;
        headerPanel.BackColor = mainPanel.BackColor;
        headerActions.BackColor = mainPanel.BackColor;
        ordersHeaderPanel.BackColor = mainPanel.BackColor;
        brandLabel.ForeColor = Color.White;
        userLabel.ForeColor = Color.FromArgb(198, 210, 224);
        foreach (var button in new[] { homeButton, productButton, posButton, inventoryButton, onlineButton, reportButton, accountButton })
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(24, 39, 58);
            button.ForeColor = Color.FromArgb(226, 234, 242);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(14, 0, 0, 0);
            button.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }
        homeButton.BackColor = Color.FromArgb(44, 105, 162);
        logoutButton.BackColor = Color.FromArgb(49, 67, 88);
        logoutButton.ForeColor = Color.White;
    }

    private static string RoleName(string role) => role switch
    {
        "MANAGER" => "Quản lý",
        "EMPLOYEE" => "Nhân viên",
        _ => role
    };

    private void homeButton_Click(object? sender, EventArgs e)
    {
        CloseActivePage();
        pageHost.Visible = false;
        dashboardPanel.Visible = true;
        dashboardPanel.BringToFront();
        homeButton.BackColor = Color.FromArgb(44, 105, 162);
        RefreshDashboard();
    }

    private void refreshButton_Click(object? sender, EventArgs e) => RefreshDashboard();

    private void RefreshDashboard()
    {
        if (_session is null) return;
        try
        {
            using var connection = OracleConnectionFactory.Create();
            connection.Open();
            salesValue.Text = ReadScalar(connection,
                "SELECT NVL(SUM(TotalAmount),0) FROM Orders WHERE Status='Completed' AND OrderDate >= TRUNC(SYSDATE)").ToString("N0") + " đ";
            ordersValue.Text = ReadScalar(connection,
                "SELECT COUNT(*) FROM Orders WHERE Status IN ('Pending','Shipping')").ToString("N0");
            stockValue.Text = ReadScalar(connection,
                "SELECT COUNT(*) FROM ProductVariants WHERE StockQuantity=0").ToString("N0");
            productsValue.Text = ReadScalar(connection,
                "SELECT COUNT(*) FROM Products").ToString("N0");
            recentOrdersGrid.DataSource = LoadRecentOrders(connection);
            ConfigureRecentOrdersGrid();
            if (recentOrdersGrid.Columns.Contains("CREATED_AT"))
                recentOrdersGrid.Columns["CREATED_AT"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (recentOrdersGrid.Columns.Contains("TOTAL_AMOUNT"))
                recentOrdersGrid.Columns["TOTAL_AMOUNT"].DefaultCellStyle.Format = "N0 'đ'";
            connectionLabel.Text = "Đã cập nhật trực tiếp từ các bảng Oracle";
            connectionLabel.ForeColor = Color.FromArgb(38, 128, 92);
        }
        catch (Exception ex)
        {
            if (recentOrdersGrid.DataSource is null) recentOrdersGrid.DataSource = EmptyRecentOrders();
            ConfigureRecentOrdersGrid();
            connectionLabel.Text = "Không tải được dữ liệu Oracle: " + ex.Message;
            connectionLabel.ForeColor = Color.FromArgb(177, 56, 56);
        }
    }

    private static DataTable EmptyRecentOrders()
    {
        var table = new DataTable();
        table.Columns.Add("CHANNEL", typeof(string));
        table.Columns.Add("ORDER_CODE", typeof(string));
        table.Columns.Add("CUSTOMER", typeof(string));
        table.Columns.Add("ORDER_STATUS", typeof(string));
        table.Columns.Add("TOTAL_AMOUNT", typeof(decimal));
        table.Columns.Add("CREATED_AT", typeof(DateTime));
        return table;
    }

    private void ConfigureRecentOrdersGrid()
    {
        var headers = new[] { ("CHANNEL", "Kênh"), ("ORDER_CODE", "Mã đơn"), ("CUSTOMER", "Khách hàng"), ("ORDER_STATUS", "Trạng thái"), ("TOTAL_AMOUNT", "Tổng tiền"), ("CREATED_AT", "Ngày tạo") };
        foreach (var (key, label) in headers)
            if (recentOrdersGrid.Columns.Contains(key)) recentOrdersGrid.Columns[key].HeaderText = label;
        if (recentOrdersGrid.Columns.Contains("CREATED_AT")) recentOrdersGrid.Columns["CREATED_AT"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        if (recentOrdersGrid.Columns.Contains("TOTAL_AMOUNT")) recentOrdersGrid.Columns["TOTAL_AMOUNT"].DefaultCellStyle.Format = "N0";
    }
    private static decimal ReadScalar(OracleConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        var result = command.ExecuteScalar();
        return result is null or DBNull ? 0 : Convert.ToDecimal(result);
    }

    private static DataTable LoadRecentOrders(OracleConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT CASE WHEN o.EmployeeID IS NULL THEN 'Online' ELSE 'Tại quầy' END AS CHANNEL,
                   TO_CHAR(o.OrderID) AS ORDER_CODE, c.FullName AS CUSTOMER,
                   o.Status AS ORDER_STATUS, o.TotalAmount AS TOTAL_AMOUNT, o.OrderDate AS CREATED_AT
            FROM Orders o JOIN Customers c ON c.CustomerID=o.CustomerID
            ORDER BY o.OrderDate DESC FETCH FIRST 15 ROWS ONLY";
        var table = new DataTable();
        using var adapter = new OracleDataAdapter(command);
        adapter.Fill(table);
        return table;
    }

    private void moduleButton_Click(object? sender, EventArgs e)
    {
        if (_session is null || sender is not Button button) return;
        if (button == productButton)
        {
            OpenPage(new ProductsForm(_session));
        }
        else if (button == posButton)
        {
            OpenPage(new PosForm(_session));
        }
        else if (button == inventoryButton)
        {
            OpenPage(new InventoryForm(_session));
        }
        else if (button == onlineButton)
        {
            OpenPage(new OnlineOrdersForm(_session));
        }
        else if (button == reportButton)
        {
            OpenPage(new ReportsForm(_session));
        }
        foreach (var navButton in new[] { productButton, posButton, inventoryButton, onlineButton, reportButton, accountButton, homeButton })
            navButton.BackColor = ReferenceEquals(navButton, button) ? Color.FromArgb(44, 105, 162) : Color.FromArgb(24, 39, 58);
    }

    private void OpenPage(Form form)
    {
        CloseActivePage();
        dashboardPanel.Visible = false;
        homeButton.BackColor = Color.FromArgb(24, 39, 58);
        _activePage = form;
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.WindowState = FormWindowState.Normal;
        form.Dock = DockStyle.Fill;
        form.FormClosed += ActivePage_FormClosed;
        pageHost.Controls.Add(form);
        pageHost.Visible = true;
        pageHost.BringToFront();
        form.Show();
    }

    private void ActivePage_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (sender is Form form)
        {
            pageHost.Controls.Remove(form);
            form.Dispose();
        }
        _activePage = null;
        pageHost.Visible = false;
        dashboardPanel.Visible = true;
        dashboardPanel.BringToFront();
        RefreshDashboard();
    }

    private void CloseActivePage()
    {
        if (_activePage is not null)
        {
            _activePage.FormClosed -= ActivePage_FormClosed;
            var page = _activePage;
            _activePage = null;
            pageHost.Controls.Remove(page);
            page.Close();
            page.Dispose();
        }
        pageHost.Visible = false;
    }

    private void accountButton_Click(object? sender, EventArgs e)
    {
        if (_session is null || !_session.IsManager) return;
        OpenPage(new CustomerEmployeeForm(_session));
        foreach (var navButton in new[] { productButton, posButton, inventoryButton, onlineButton, reportButton, accountButton, homeButton })
            navButton.BackColor = ReferenceEquals(navButton, accountButton) ? Color.FromArgb(44, 105, 162) : Color.FromArgb(24, 39, 58);
    }

    private void logoutButton_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Bạn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            CloseActivePage();
            _session = null;
            DialogResult = DialogResult.Retry;
            Close();
        }
    }

}

internal sealed record UserSession(int UserId, string Username, string RoleCode, int? EmployeeId)
{
    public bool IsManager => RoleCode == "MANAGER";

    public bool HasAnyRole(params string[] roles) => IsManager ||
        (RoleCode == "EMPLOYEE" && roles.Any(x => x is "CASHIER" or "WAREHOUSE" or "EMPLOYEE"));

    public void Require(params string[] roles)
    {
        if (!HasAnyRole(roles))
            throw new UnauthorizedAccessException("Tài khoản hiện tại không có quyền thực hiện chức năng này.");
    }
}

/// <summary>Shared visual palette for the shoe store forms.</summary>
[SupportedOSPlatform("windows")]
internal static class UiTheme
{
    private static readonly Color Canvas = Color.FromArgb(244, 247, 251);
    private static readonly Color Surface = Color.White;
    private static readonly Color Ink = Color.FromArgb(37, 52, 71);
    private static readonly Color MutedInk = Color.FromArgb(91, 107, 126);
    private static readonly Color Accent = Color.FromArgb(40, 91, 145);
    private static readonly Color AccentHover = Color.FromArgb(54, 113, 172);
    private static readonly Color SoftBlue = Color.FromArgb(231, 240, 250);
    private static readonly Color Border = Color.FromArgb(211, 222, 235);
    private static readonly Color Danger = Color.FromArgb(177, 56, 56);
    private static readonly Color DangerHover = Color.FromArgb(198, 69, 69);

    public static void Apply(Form form)
    {
        form.BackColor = Canvas;
        form.ForeColor = Ink;
        form.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        foreach (Control control in form.Controls)
            ApplyControl(control);
    }

    private static void ApplyControl(Control control)
    {
        switch (control)
        {
            case Button button:
                StyleButton(button);
                break;
            case DataGridView grid:
                StyleGrid(grid);
                break;
            case Label label:
                label.ForeColor = label.Text.Contains("cảnh báo", StringComparison.OrdinalIgnoreCase)
                    ? Color.FromArgb(170, 102, 19)
                    : (label.Font.Bold || label.Font.Size >= 16F ? Accent : Ink);
                break;
            case TextBoxBase textBox:
                textBox.BackColor = Surface;
                textBox.ForeColor = Ink;
                break;
            case ComboBox comboBox:
                comboBox.BackColor = Surface;
                comboBox.ForeColor = Ink;
                comboBox.FlatStyle = FlatStyle.Flat;
                break;
            case NumericUpDown numeric:
                numeric.BackColor = Surface;
                numeric.ForeColor = Ink;
                break;
            case DateTimePicker datePicker:
                datePicker.CalendarTitleBackColor = Accent;
                datePicker.CalendarTitleForeColor = Color.White;
                datePicker.CalendarMonthBackground = Surface;
                datePicker.CalendarForeColor = Ink;
                break;
            case GroupBox group:
                group.ForeColor = Accent;
                group.BackColor = Surface;
                break;
            case TabPage page:
                page.BackColor = Canvas;
                page.ForeColor = Ink;
                break;
            case Panel panel:
                panel.BackColor = Surface;
                break;
        }

        foreach (Control child in control.Controls)
            ApplyControl(child);
    }

    private static void StyleButton(Button button)
    {
        var text = button.Text;
        var isDanger = text.Contains("xóa", StringComparison.OrdinalIgnoreCase)
            || text.Contains("ngừng bán", StringComparison.OrdinalIgnoreCase)
            || text.Contains("hủy", StringComparison.OrdinalIgnoreCase);
        var isPrimary = new[] { "đăng nhập", "thanh toán", "lưu", "thêm", "tạo", "khởi tạo", "nhập hàng" }
            .Any(word => text.Contains(word, StringComparison.OrdinalIgnoreCase));

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = isDanger ? Danger : (isPrimary ? Accent : Border);
        button.FlatAppearance.MouseOverBackColor = isDanger ? DangerHover : (isPrimary ? AccentHover : SoftBlue);
        button.UseVisualStyleBackColor = false;
        button.BackColor = isDanger ? Danger : (isPrimary ? Accent : SoftBlue);
        button.ForeColor = isDanger || isPrimary ? Color.White : Accent;
    }

    private static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = Border;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Accent;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Accent;
        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Ink;
        grid.DefaultCellStyle.SelectionBackColor = SoftBlue;
        grid.DefaultCellStyle.SelectionForeColor = Ink;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
    }
}


