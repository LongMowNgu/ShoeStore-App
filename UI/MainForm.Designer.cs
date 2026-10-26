#nullable enable
namespace ShoeStore.WinForms.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer? components;
    private Label titleLabel = null!;
    private Label userLabel = null!;
    private Button productButton = null!;
    private Button posButton = null!;
    private Button inventoryButton = null!;
    private Button onlineButton = null!;
    private Button reportButton = null!;
    private Button accountButton = null!;
    private Button logoutButton = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        titleLabel = new Label(); userLabel = new Label(); productButton = new Button(); posButton = new Button();
        inventoryButton = new Button(); onlineButton = new Button(); reportButton = new Button(); accountButton = new Button(); logoutButton = new Button();
        SuspendLayout();
        Text = "ShoeStore | Quản lý cửa hàng giày"; StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized; MinimumSize = new Size(980, 650); BackColor = Color.FromArgb(245, 247, 250); Font = new Font("Segoe UI", 10F);
        titleLabel.AutoSize = true; titleLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold); titleLabel.ForeColor = Color.FromArgb(26, 45, 72);
        titleLabel.Location = new Point(42, 34); titleLabel.Text = "QUẢN LÝ CỬA HÀNG GIÀY";
        userLabel.AutoSize = true; userLabel.ForeColor = Color.DimGray; userLabel.Location = new Point(45, 88); userLabel.Text = "";
        productButton.Location = new Point(48, 160); productButton.Size = new Size(260, 76); productButton.Text = "Sản phẩm & biến thể"; productButton.Click += moduleButton_Click;
        posButton.Location = new Point(340, 160); posButton.Size = new Size(260, 76); posButton.Text = "Bán hàng tại quầy"; posButton.Click += moduleButton_Click;
        inventoryButton.Location = new Point(632, 160); inventoryButton.Size = new Size(260, 76); inventoryButton.Text = "Kho & nhập hàng"; inventoryButton.Click += moduleButton_Click;
        onlineButton.Location = new Point(48, 270); onlineButton.Size = new Size(260, 76); onlineButton.Text = "Đơn hàng online"; onlineButton.Click += moduleButton_Click;
        reportButton.Location = new Point(340, 270); reportButton.Size = new Size(260, 76); reportButton.Text = "Thống kê & báo cáo"; reportButton.Click += moduleButton_Click;
        accountButton.Location = new Point(632, 270); accountButton.Size = new Size(260, 76); accountButton.Text = "Tài khoản & phân quyền"; accountButton.Click += accountButton_Click;
        foreach (var button in new[] { productButton, posButton, inventoryButton, onlineButton, reportButton, accountButton })
        {
            button.FlatStyle = FlatStyle.Flat; button.FlatAppearance.BorderSize = 0; button.BackColor = Color.FromArgb(39, 91, 145);
            button.ForeColor = Color.White; button.Font = new Font("Segoe UI", 12F, FontStyle.Bold); button.Anchor = AnchorStyles.Top;
        }
        logoutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right; logoutButton.Location = new Point(780, 540); logoutButton.Size = new Size(150, 42);
        logoutButton.Text = "Đăng xuất"; logoutButton.Click += logoutButton_Click;
        Controls.AddRange(new Control[] { titleLabel, userLabel, productButton, posButton, inventoryButton, onlineButton, reportButton, accountButton, logoutButton });
        ResumeLayout(false); PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
}
