using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

internal sealed class CustomerEmployeeForm : Form
{
    private readonly UserSession _actor;
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _customers = Grid();
    private readonly DataGridView _employees = Grid();
    private readonly TextBox[] _customerFields = Fields(4);
    private readonly TextBox[] _employeeFields = Fields(5);
    private readonly DateTimePicker _hireDate = new() { Format = DateTimePickerFormat.Short, Width = 150 };
    private readonly NumericUpDown _salary = new() { Minimum = 1, Maximum = 999999999999, DecimalPlaces = 0, ThousandsSeparator = true, Width = 180 };

    public CustomerEmployeeForm(UserSession actor)
    {
        actor.Require("MANAGER"); _actor = actor;
        _customerFields[0].MaxLength = 100; _customerFields[1].MaxLength = 15; _customerFields[2].MaxLength = 100; _customerFields[3].MaxLength = 200;
        _employeeFields[0].MaxLength = 100; _employeeFields[1].MaxLength = 50; _employeeFields[2].MaxLength = 15; _salary.Value = 8000000;
        Text = "Quản lý khách hàng và nhân viên"; MinimumSize = new Size(900, 600); Size = new Size(1100, 720);
        UiTheme.Apply(this);
        var customersTab = new TabPage("Khách hàng"); customersTab.Controls.Add(BuildCustomerPage());
        var employeesTab = new TabPage("Nhân viên"); employeesTab.Controls.Add(BuildEmployeePage());
        _tabs.TabPages.Add(customersTab); _tabs.TabPages.Add(employeesTab); Controls.Add(_tabs);
        _customers.SelectionChanged += (_, _) => FillCustomer(); _employees.SelectionChanged += (_, _) => FillEmployee();
        Load += (_, _) => Reload();
    }

    private Control BuildCustomerPage()
    {
        var layout = PageLayout(); layout.Controls.Add(_customers, 0, 0);
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(12), WrapContents = true };
        var labels = new[] { "Họ tên", "Điện thoại", "Email", "Địa chỉ" };
        for (var i = 0; i < labels.Length; i++) AddField(panel, labels[i], _customerFields[i]);
        panel.Controls.Add(ActionButton("Thêm khách", (_, _) => SaveCustomer(false)));
        panel.Controls.Add(ActionButton("Cập nhật khách", (_, _) => SaveCustomer(true)));
        panel.Controls.Add(ActionButton("Làm mới", (_, _) => { _customerFields.ToList().ForEach(x => x.Clear()); ReloadCustomers(); }));
        layout.Controls.Add(panel, 0, 1); return layout;
    }

    private Control BuildEmployeePage()
    {
        var layout = PageLayout(); layout.Controls.Add(_employees, 0, 0);
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(12), WrapContents = true };
        var labels = new[] { "Họ tên", "Chức vụ", "Điện thoại", "Ngày vào làm", "Lương" };
        for (var i = 0; i < 3; i++) AddField(panel, labels[i], _employeeFields[i]);
        AddField(panel, labels[3], _hireDate); AddField(panel, labels[4], _salary);
        panel.Controls.Add(ActionButton("Thêm nhân viên", (_, _) => SaveEmployee(false)));
        panel.Controls.Add(ActionButton("Cập nhật nhân viên", (_, _) => SaveEmployee(true)));
        panel.Controls.Add(ActionButton("Làm mới", (_, _) => { _employeeFields.ToList().ForEach(x => x.Clear()); _salary.Value = 8000000; ReloadEmployees(); }));
        layout.Controls.Add(panel, 0, 1); return layout;
    }

    private static TableLayoutPanel PageLayout()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(10) };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 68)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 32)); return layout;
    }
    private static DataGridView Grid() => new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
    private static TextBox[] Fields(int count) => Enumerable.Range(0, count).Select(_ => new TextBox { Width = 220, MaxLength = 200 }).ToArray();
    private static void AddField(FlowLayoutPanel panel, string title, Control input)
    { var box = new Panel { Width = 240, Height = 58, Margin = new Padding(5) }; var label = new Label { Text = title, Dock = DockStyle.Top, Height = 22 }; input.Location = new Point(0, 24); box.Controls.Add(input); box.Controls.Add(label); panel.Controls.Add(box); }
    private static Button ActionButton(string text, EventHandler click) { var b = new Button { Text = text, AutoSize = true, Height = 38, Margin = new Padding(5, 28, 5, 5) }; b.Click += click; return b; }
    private static void Param(OracleCommand q, string name, OracleDbType type, object value) => q.Parameters.Add(name, type).Value = value;

    private void Reload() { ReloadCustomers(); ReloadEmployees(); }
    private void ReloadCustomers()
    { try { _customers.DataSource = Read("SELECT CustomerID AS ID,FullName AS HO_TEN,Phone AS DIEN_THOAI,Email AS EMAIL,Address AS DIA_CHI,RegisterDate AS NGAY_DANG_KY FROM Customers ORDER BY CustomerID"); _customers.Columns["ID"].Visible = false; } catch (Exception e) { ShowError(e); } }
    private void ReloadEmployees()
    { try { _employees.DataSource = Read("SELECT EmployeeID AS ID,FullName AS HO_TEN,Position AS CHUC_VU,Phone AS DIEN_THOAI,HireDate AS NGAY_VAO_LAM,Salary AS LUONG FROM Employees ORDER BY EmployeeID"); _employees.Columns["ID"].Visible = false; } catch (Exception e) { ShowError(e); } }
    private static DataTable Read(string sql) { using var c = OracleConnectionFactory.Create(); using var q = c.CreateCommand(); q.CommandText = sql; var t = new DataTable(); using var a = new OracleDataAdapter(q); a.Fill(t); return t; }
    private int? SelectedId(DataGridView grid) => grid.CurrentRow?.Cells["ID"].Value is null ? null : Convert.ToInt32(grid.CurrentRow.Cells["ID"].Value);
    private void FillCustomer() { if (SelectedId(_customers) is null) return; var r = _customers.CurrentRow!; _customerFields[0].Text = Convert.ToString(r.Cells["HO_TEN"].Value) ?? ""; _customerFields[1].Text = Convert.ToString(r.Cells["DIEN_THOAI"].Value) ?? ""; _customerFields[2].Text = Convert.ToString(r.Cells["EMAIL"].Value) ?? ""; _customerFields[3].Text = Convert.ToString(r.Cells["DIA_CHI"].Value) ?? ""; }
    private void FillEmployee() { if (SelectedId(_employees) is null) return; var r = _employees.CurrentRow!; _employeeFields[0].Text = Convert.ToString(r.Cells["HO_TEN"].Value) ?? ""; _employeeFields[1].Text = Convert.ToString(r.Cells["CHUC_VU"].Value) ?? ""; _employeeFields[2].Text = Convert.ToString(r.Cells["DIEN_THOAI"].Value) ?? ""; if (r.Cells["NGAY_VAO_LAM"].Value is DateTime d) _hireDate.Value = d; if (r.Cells["LUONG"].Value is not DBNull) _salary.Value = Math.Clamp(Convert.ToDecimal(r.Cells["LUONG"].Value), _salary.Minimum, _salary.Maximum); }

    private void SaveCustomer(bool update)
    {
        _actor.Require("MANAGER"); var id = SelectedId(_customers); if (update && id is null) { Warn("Chọn khách hàng cần cập nhật."); return; }
        var name = _customerFields[0].Text.Trim(); var phone = _customerFields[1].Text.Trim(); var email = Db(_customerFields[2].Text); var address = Db(_customerFields[3].Text); if (name.Length == 0 || phone.Length == 0) { Warn("Họ tên và số điện thoại là bắt buộc."); return; }
        try
        {
            using var c=OracleConnectionFactory.Create(); c.Open(); using var tx=c.BeginTransaction();
            if (CustomerAlreadyExists(c, tx, phone, email, update ? id : null)) { tx.Rollback(); Warn("Số điện thoại hoặc email đã được khách hàng khác sử dụng."); return; }
            if(update) { using var q=c.CreateCommand(); q.Transaction=tx; q.BindByName=true; q.CommandText="UPDATE Customers SET FullName=:name,Phone=:phone,Email=:email,Address=:address WHERE CustomerID=:id"; Param(q,"name",OracleDbType.Varchar2,name); Param(q,"phone",OracleDbType.Varchar2,phone); Param(q,"email",OracleDbType.Varchar2,email); Param(q,"address",OracleDbType.Varchar2,address); Param(q,"id",OracleDbType.Int32,id!.Value); q.ExecuteNonQuery(); }
            else { SalesService.Lock(c,tx,"Customers"); using var q=c.CreateCommand(); q.Transaction=tx; q.BindByName=true; q.CommandText="SELECT NVL(MAX(CustomerID),0)+1 FROM Customers"; var next=Convert.ToInt32(q.ExecuteScalar()); q.CommandText="INSERT INTO Customers(CustomerID,FullName,Phone,Email,Address,RegisterDate) VALUES(:id,:name,:phone,:email,:address,SYSDATE)"; Param(q,"id",OracleDbType.Int32,next); Param(q,"name",OracleDbType.Varchar2,name); Param(q,"phone",OracleDbType.Varchar2,phone); Param(q,"email",OracleDbType.Varchar2,email); Param(q,"address",OracleDbType.Varchar2,address); q.ExecuteNonQuery(); }
            tx.Commit(); ReloadCustomers(); MessageBox.Show("Đã lưu khách hàng.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
        } catch(OracleException e) when (e.Number == 1) { Warn("Số điện thoại hoặc email đã được khách hàng khác sử dụng."); }
        catch(Exception e) { ShowError(e); }
    }

    private static bool CustomerAlreadyExists(OracleConnection c, OracleTransaction tx, string phone, object email, int? excludeId)
    {
        using var q = c.CreateCommand(); q.Transaction = tx; q.BindByName = true;
        q.CommandText = "SELECT COUNT(*) FROM Customers WHERE (Phone=:customer_phone OR (:customer_email IS NOT NULL AND Email=:customer_email)) AND (:exclude_customer_id IS NULL OR CustomerID<>:exclude_customer_id)";
        Param(q, "customer_phone", OracleDbType.Varchar2, phone);
        Param(q, "customer_email", OracleDbType.Varchar2, email);
        Param(q, "exclude_customer_id", OracleDbType.Int32, (object?)excludeId ?? DBNull.Value);
        return Convert.ToInt32(q.ExecuteScalar()) > 0;
    }

    private void SaveEmployee(bool update)
    {
        _actor.Require("MANAGER"); var id=SelectedId(_employees); if(update && id is null) { Warn("Chọn nhân viên cần cập nhật."); return; }
        var name=_employeeFields[0].Text.Trim(); var position=_employeeFields[1].Text.Trim(); var phone=_employeeFields[2].Text.Trim(); if(name.Length==0||position.Length==0||phone.Length==0) { Warn("Họ tên, chức vụ và điện thoại là bắt buộc."); return; }
        try
        {
            using var c=OracleConnectionFactory.Create(); c.Open(); using var tx=c.BeginTransaction();
            if(update) { using var q=c.CreateCommand(); q.Transaction=tx; q.BindByName=true; q.CommandText="UPDATE Employees SET FullName=:name,Position=:position,Phone=:phone,HireDate=:hire,Salary=:salary WHERE EmployeeID=:id"; Param(q,"name",OracleDbType.Varchar2,name); Param(q,"position",OracleDbType.Varchar2,position); Param(q,"phone",OracleDbType.Varchar2,phone); Param(q,"hire",OracleDbType.Date,_hireDate.Value.Date); Param(q,"salary",OracleDbType.Decimal,_salary.Value); Param(q,"id",OracleDbType.Int32,id!.Value); q.ExecuteNonQuery(); }
            else { SalesService.Lock(c,tx,"Employees"); using var q=c.CreateCommand(); q.Transaction=tx; q.BindByName=true; q.CommandText="SELECT NVL(MAX(EmployeeID),0)+1 FROM Employees"; var next=Convert.ToInt32(q.ExecuteScalar()); q.CommandText="INSERT INTO Employees(EmployeeID,FullName,Position,Phone,HireDate,Salary) VALUES(:id,:name,:position,:phone,:hire,:salary)"; Param(q,"id",OracleDbType.Int32,next); Param(q,"name",OracleDbType.Varchar2,name); Param(q,"position",OracleDbType.Varchar2,position); Param(q,"phone",OracleDbType.Varchar2,phone); Param(q,"hire",OracleDbType.Date,_hireDate.Value.Date); Param(q,"salary",OracleDbType.Decimal,_salary.Value); q.ExecuteNonQuery(); }
            tx.Commit(); ReloadEmployees(); MessageBox.Show("Đã lưu nhân viên.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
        } catch(Exception e) { ShowError(e); }
    }
    private static object Db(string text) => string.IsNullOrWhiteSpace(text) ? DBNull.Value : text.Trim();
    private static void Warn(string text) => MessageBox.Show(text,"Kiểm tra dữ liệu",MessageBoxButtons.OK,MessageBoxIcon.Warning);
    private static void ShowError(Exception e) => MessageBox.Show("Không thể thao tác dữ liệu Oracle. Kiểm tra quyền, dữ liệu duy nhất và cấu hình kết nối.\n\n"+e.Message,"Lỗi database",MessageBoxButtons.OK,MessageBoxIcon.Error);
}

