#nullable enable
namespace ShoeStore.WinForms.UI;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components;
    private Label heading = null!;
    private Label intro = null!;
    private TextBox username = null!;
    private TextBox password = null!;
    private Button signInButton = null!;
    private Button createManagerButton = null!;
    private Button connectionButton = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        heading = new Label(); intro = new Label(); username = new TextBox(); password = new TextBox();
        signInButton = new Button(); createManagerButton = new Button(); connectionButton = new Button();
        var userLabel = new Label(); var passwordLabel = new Label();
        SuspendLayout();
        Text = "ShoeStore | Đăng nhập"; StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        ClientSize = new Size(460, 440); BackColor = Color.White; Font = new Font("Segoe UI", 10F);
        heading.AutoSize = true; heading.Font = new Font("Segoe UI", 22F, FontStyle.Bold); heading.ForeColor = Color.FromArgb(30, 58, 84);
        heading.Location = new Point(42, 34); heading.Text = "SHOESTORE";
        intro.Location = new Point(44, 83); intro.Size = new Size(365, 42); intro.Text = "Đăng nhập để quản lý bán hàng và kho giày.";
        userLabel.AutoSize = true; userLabel.Location = new Point(44, 143); userLabel.Text = "Tên đăng nhập";
        username.Location = new Point(44, 168); username.Size = new Size(370, 30); username.MaxLength = 40;
        passwordLabel.AutoSize = true; passwordLabel.Location = new Point(44, 214); passwordLabel.Text = "Mật khẩu";
        password.Location = new Point(44, 239); password.Size = new Size(370, 30); password.MaxLength = 128; password.UseSystemPasswordChar = true;
        signInButton.Location = new Point(44, 293); signInButton.Size = new Size(370, 42); signInButton.Text = "Đăng nhập"; signInButton.Click += signInButton_Click;
        createManagerButton.Location = new Point(44, 345); createManagerButton.Size = new Size(370, 32); createManagerButton.Text = "Khởi tạo quản lý đầu tiên"; createManagerButton.Click += createManagerButton_Click;
        connectionButton.Location = new Point(44, 386); connectionButton.Size = new Size(370, 30); connectionButton.Text = "Kiểm tra kết nối Oracle"; connectionButton.Click += connectionButton_Click;
        AcceptButton = signInButton;
        Controls.AddRange(new Control[] { heading, intro, userLabel, username, passwordLabel, password, signInButton, createManagerButton, connectionButton });
        ResumeLayout(false); PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
}
