using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class MainForm : Form
{
    private readonly UserSession _session;

    public MainForm(UserSession session)
    {
        _session = session;
        InitializeComponent();
        userLabel.Text = $"Đang đăng nhập: {session.Username} · {RoleName(session.RoleCode)}";
        productButton.Enabled = session.HasAnyRole("WAREHOUSE");
        inventoryButton.Enabled = session.HasAnyRole("WAREHOUSE");
        posButton.Enabled = session.HasAnyRole("CASHIER");
        onlineButton.Enabled = session.HasAnyRole("WAREHOUSE", "CASHIER");
        reportButton.Enabled = session.IsManager;
        accountButton.Enabled = session.IsManager;
    }

    private static string RoleName(string role) => role switch
    {
        "MANAGER" => "Quản lý",
        "CASHIER" => "Nhân viên bán hàng",
        "WAREHOUSE" => "Nhân viên kho",
        _ => role
    };

    private void moduleButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button == productButton)
        {
            using var products = new ProductsForm(_session);
            products.ShowDialog(this);
            return;
        }
        if (button == posButton) { using var form = new PosForm(_session); form.ShowDialog(this); return; }
        if (button == inventoryButton) { using var form = new InventoryForm(_session); form.ShowDialog(this); return; }
        if (button == onlineButton) { using var form = new OnlineOrdersForm(_session); form.ShowDialog(this); return; }
        if (button == reportButton) { using var form = new ReportsForm(_session); form.ShowDialog(this); return; }
        MessageBox.Show($"Mục “{button.Text}” sẽ được nối với module Oracle tiếp theo.", "Khung chương trình", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void accountButton_Click(object? sender, EventArgs e)
    {
        if (!_session.IsManager) return;
        using var form = new AccountManagementForm(_session);
        form.ShowDialog(this);
    }

    private void logoutButton_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Bạn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }
    }
}
