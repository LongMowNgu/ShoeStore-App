using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class LoginForm : Form
{
    private readonly AuthRepository _auth = new();
    public UserSession? Session { get; private set; }

    public LoginForm() => InitializeComponent();

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        try { createManagerButton.Visible = !_auth.HasAnyAccount(); }
        catch (Exception ex) { ShowOracleError(ex); }
    }

    private void signInButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(username.Text) || password.Text.Length == 0)
        { MessageBox.Show("Nhập tên đăng nhập và mật khẩu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        try
        {
            Session = _auth.SignIn(username.Text, password.Text);
            if (Session is null)
            { MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.", "Không đăng nhập được", MessageBoxButtons.OK, MessageBoxIcon.Warning); password.SelectAll(); password.Focus(); return; }
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex) { ShowOracleError(ex); }
    }

    private void createManagerButton_Click(object? sender, EventArgs e)
    {
        using var form = new CreateAccountForm(initialManager: true, actor: null);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            username.Text = form.CreatedUsername;
            password.Clear();
            createManagerButton.Visible = false;
            MessageBox.Show("Đã tạo tài khoản quản lý. Đăng nhập bằng tài khoản vừa tạo.", "Khởi tạo hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void connectionButton_Click(object? sender, EventArgs e)
    {
        try
        {
            using var connection = OracleConnectionFactory.Create();
            connection.Open();
            MessageBox.Show($"Đã kết nối Oracle: {connection.DataSource}", "Kết nối thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            createManagerButton.Visible = !_auth.HasAnyAccount();
        }
        catch (Exception ex) { ShowOracleError(ex); }
    }

    private void ShowOracleError(Exception ex) => MessageBox.Show(
        "Không kết nối hoặc không đọc được schema Oracle. Kiểm tra listener/service, connection string và chạy script Database/01_Schema.sql.\n\nChi tiết: " + ex.Message,
        "Lỗi Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
