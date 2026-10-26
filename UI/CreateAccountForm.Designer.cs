#nullable enable
namespace ShoeStore.WinForms.UI;

partial class CreateAccountForm
{
    private System.ComponentModel.IContainer? components;
    private Label titleLabel = null!;
    private Label roleLabel = null!;
    private TextBox username = null!;
    private TextBox password = null!;
    private TextBox confirmPassword = null!;
    private TextBox employeeName = null!;
    private TextBox employeePhone = null!;
    private ComboBox role = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        titleLabel = new Label(); roleLabel = new Label(); username = new TextBox(); password = new TextBox();
        confirmPassword = new TextBox(); employeeName = new TextBox(); employeePhone = new TextBox(); role = new ComboBox(); saveButton = new Button(); cancelButton = new Button();
        var userLabel = new Label(); var passwordLabel = new Label(); var confirmLabel = new Label(); var nameLabel = new Label(); var phoneLabel = new Label();
        SuspendLayout();
        Text = "Tạo tài khoản nhân viên"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false; ClientSize = new Size(440, 520); Font = new Font("Segoe UI", 10F); BackColor = Color.White;
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI", 15F, FontStyle.Bold); titleLabel.Location = new Point(28, 24); titleLabel.Text = "Tạo tài khoản";
        userLabel.AutoSize = true; userLabel.Location = new Point(30, 82); userLabel.Text = "Tên đăng nhập";
        username.Location = new Point(30, 106); username.Size = new Size(375, 30); username.MaxLength = 40;
        passwordLabel.AutoSize = true; passwordLabel.Location = new Point(30, 150); passwordLabel.Text = "Mật khẩu (ít nhất 8 ký tự)";
        password.Location = new Point(30, 174); password.Size = new Size(375, 30); password.MaxLength = 128; password.UseSystemPasswordChar = true;
        confirmLabel.AutoSize = true; confirmLabel.Location = new Point(30, 218); confirmLabel.Text = "Nhập lại mật khẩu";
        confirmPassword.Location = new Point(30, 242); confirmPassword.Size = new Size(375, 30); confirmPassword.MaxLength = 128; confirmPassword.UseSystemPasswordChar = true;
        roleLabel.AutoSize = true; roleLabel.Location = new Point(30, 286); roleLabel.Text = "Vai trò";
        role.Location = new Point(30, 310); role.Size = new Size(375, 30); role.DropDownStyle = ComboBoxStyle.DropDownList; role.SelectedIndexChanged += role_SelectedIndexChanged;
        nameLabel.AutoSize = true; nameLabel.Location = new Point(30, 352); nameLabel.Text = "Họ và tên nhân viên";
        employeeName.Location = new Point(30, 376); employeeName.Size = new Size(375, 30); employeeName.MaxLength = 100;
        phoneLabel.AutoSize = true; phoneLabel.Location = new Point(30, 420); phoneLabel.Text = "Số điện thoại";
        employeePhone.Location = new Point(30, 444); employeePhone.Size = new Size(375, 30); employeePhone.MaxLength = 20;
        saveButton.Location = new Point(190, 480); saveButton.Size = new Size(105, 32); saveButton.Text = "Tạo tài khoản"; saveButton.Click += saveButton_Click;
        cancelButton.Location = new Point(305, 480); cancelButton.Size = new Size(100, 32); cancelButton.Text = "Hủy"; cancelButton.DialogResult = DialogResult.Cancel;
        AcceptButton = saveButton; CancelButton = cancelButton;
        Controls.AddRange(new Control[] { titleLabel, userLabel, username, passwordLabel, password, confirmLabel, confirmPassword, roleLabel, role, nameLabel, employeeName, phoneLabel, employeePhone, saveButton, cancelButton });
        ResumeLayout(false); PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
}
