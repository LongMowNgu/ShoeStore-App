using System.Configuration;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class LoginForm : Form
{
    internal UserSession? Session { get; private set; }

    public LoginForm()
    {
        InitializeComponent();
        UiTheme.Apply(this);
        createManagerButton.Visible = false;
        intro.Text = "Đăng nhập bằng tài khoản Oracle được cấp role quản lý hoặc nhân viên.";
        username.PlaceholderText = "QL_GIAYTHE hoặc NV_GIAYTHE";
    }

    private void signInButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(username.Text) || password.Text.Length == 0)
        {
            MessageBox.Show("Nhập tên đăng nhập và mật khẩu Oracle.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Session = AuthRepository.SignIn(username.Text.Trim(), password.Text);
            OracleConnectionFactory.SetCredentials(username.Text.Trim(), password.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Đăng nhập Oracle thất bại. Kiểm tra tài khoản, mật khẩu, service và role đã được cấp.\n\n" + ex.Message,
                "Không đăng nhập được", MessageBoxButtons.OK, MessageBoxIcon.Error);
            password.SelectAll();
            password.Focus();
        }
    }

    private void createManagerButton_Click(object? sender, EventArgs e) { }

    private void connectionButton_Click(object? sender, EventArgs e)
    {
        try
        {
            using var connection = OracleConnectionFactory.Create(username.Text.Trim(), password.Text);
            connection.Open();
            var session = AuthRepository.ReadSession(connection);
            MessageBox.Show($"Kết nối thành công: {connection.DataSource}\nTài khoản: {session.Username}\nRole ứng dụng: {session.RoleCode}",
                "Kết nối Oracle", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không kết nối được Oracle.\n\n" + ex.Message, "Lỗi Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

internal static class OracleConnectionFactory
{
    private static string? _username;
    private static string? _password;

    public static OracleConnection Create() => Create(_username, _password);

    public static OracleConnection Create(string? username, string? password)
    {
        var configured = ConfigurationManager.ConnectionStrings["Oracle"]?.ConnectionString;
        if (string.IsNullOrWhiteSpace(configured))
            throw new InvalidOperationException("Chưa cấu hình connection string 'Oracle' trong App.config.");
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new InvalidOperationException("Nhập tài khoản Oracle trước khi kết nối.");

        var builder = new OracleConnectionStringBuilder(configured)
        {
            UserID = username,
            Password = password,
            PersistSecurityInfo = false
        };
        var schemaOwner = GetSchemaOwner();
        var connection = new OracleConnection(builder.ConnectionString);
        // Login users do not own the imported tables; keep name resolution on that owner.
        connection.StateChange += (_, args) =>
        {
            if (args.CurrentState != System.Data.ConnectionState.Open) return;
            using var command = connection.CreateCommand();
            command.CommandText = $"ALTER SESSION SET CURRENT_SCHEMA = {schemaOwner}";
            command.ExecuteNonQuery();
        };
        return connection;
    }

    private static string GetSchemaOwner()
    {
        var schema = (ConfigurationManager.AppSettings["OracleSchemaOwner"] ?? "BTL_GIAYTHETHAO")
            .Trim().ToUpperInvariant();
        if (!Regex.IsMatch(schema, @"^[A-Z][A-Z0-9_$#]{0,29}$"))
            throw new InvalidOperationException("OracleSchemaOwner phải là tên schema Oracle hợp lệ.");
        return schema;
    }

    public static void SetCredentials(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public static void ClearCredentials()
    {
        _username = null;
        _password = null;
    }
}

internal static class AuthRepository
{
    public static UserSession SignIn(string username, string password)
    {
        var connection = OracleConnectionFactory.Create(username, password);
        try
        {
            connection.Open();
            return ReadSession(connection);
        }
        catch
        {
            connection.Dispose();
            throw;
        }
        finally
        {
            // The connection is only used to validate the Oracle login and enabled roles.
            connection.Dispose();
        }
    }

    public static UserSession ReadSession(OracleConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT USER FROM DUAL";
        var username = Convert.ToString(command.ExecuteScalar())?.ToUpperInvariant() ?? string.Empty;
        using var roles = connection.CreateCommand();
        roles.CommandText = "SELECT ROLE FROM SESSION_ROLES WHERE ROLE IN ('RL_GIAYTHE_QUANLY','RL_GIAYTHE_NHANVIEN')";
        var granted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var reader = roles.ExecuteReader())
            while (reader.Read()) granted.Add(reader.GetString(0));

        var roleCode = granted.Contains("RL_GIAYTHE_QUANLY") ? "MANAGER"
            : granted.Contains("RL_GIAYTHE_NHANVIEN") ? "EMPLOYEE" : null;
        if (roleCode is null)
            throw new UnauthorizedAccessException("Tài khoản Oracle chưa được cấp role RL_GIAYTHE_QUANLY hoặc RL_GIAYTHE_NHANVIEN.");
        return new UserSession(0, username, roleCode, null);
    }
}
