using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class CreateAccountForm : Form
{
    private readonly bool _initialManager;
    private readonly UserSession? _actor;
    private readonly AuthRepository _auth = new();
    public string CreatedUsername => username.Text.Trim();

    public CreateAccountForm(bool initialManager, UserSession? actor)
    {
        _initialManager = initialManager;
        _actor = actor;
        InitializeComponent();
        role.Items.AddRange(new object[] { "Quản lý", "Nhân viên bán hàng", "Nhân viên kho" });
        role.SelectedIndex = initialManager ? 0 : 1;
        role.Enabled = !initialManager;
        role_SelectedIndexChanged(this, EventArgs.Empty);
        titleLabel.Text = initialManager ? "Khởi tạo quản lý đầu tiên" : "Đăng ký tài khoản nhân viên";
        Text = titleLabel.Text;
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        if (password.Text != confirmPassword.Text)
        { MessageBox.Show("Mật khẩu nhập lại chưa khớp.", "Kiểm tra mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        try
        {
            if (_initialManager) _auth.CreateFirstManager(username.Text, password.Text);
            else
            {
                if (_actor is null) throw new UnauthorizedAccessException("Chỉ quản lý mới được đăng ký tài khoản nhân viên.");
                var roleCode = role.SelectedIndex switch { 0 => "MANAGER", 1 => "CASHIER", 2 => "WAREHOUSE", _ => throw new ArgumentException("Chọn vai trò.") };
                _auth.RegisterEmployeeAccount(_actor, username.Text, password.Text, roleCode, employeeName.Text, employeePhone.Text);
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Không thể tạo tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void role_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var needsEmployee = !_initialManager && role.SelectedIndex != 0;
        employeeName.Enabled = needsEmployee;
        employeePhone.Enabled = needsEmployee;
    }
}
