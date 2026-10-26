#nullable enable
namespace ShoeStore.WinForms.UI;

partial class AccountManagementForm
{
    private System.ComponentModel.IContainer? components;private DataGridView accountsGrid=null!;private ComboBox role=null!;private ComboBox employee=null!;private CheckBox active=null!;private Label statusLabel=null!;
    private void InitializeComponent()
    {
        components=new System.ComponentModel.Container();accountsGrid=new DataGridView();role=new ComboBox();employee=new ComboBox();active=new CheckBox();statusLabel=new Label();var title=new Label();var save=new Button();var add=new Button();var refresh=new Button();
        ((System.ComponentModel.ISupportInitialize)accountsGrid).BeginInit();SuspendLayout();
        Text="Tài khoản và phân quyền";StartPosition=FormStartPosition.CenterParent;WindowState=FormWindowState.Maximized;MinimumSize=new Size(1000,650);Font=new Font("Segoe UI",9.5F);
        title.AutoSize=true;title.Font=new Font("Segoe UI",20F,FontStyle.Bold);title.Location=new Point(24,18);title.Text="TÀI KHOẢN · KHÓA/MỞ · PHÂN QUYỀN";
        accountsGrid.Location=new Point(28,70);accountsGrid.Size=new Size(920,420);accountsGrid.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;accountsGrid.ReadOnly=true;accountsGrid.AllowUserToAddRows=false;accountsGrid.SelectionMode=DataGridViewSelectionMode.FullRowSelect;accountsGrid.MultiSelect=false;accountsGrid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;accountsGrid.RowHeadersVisible=false;accountsGrid.SelectionChanged+=accountsGrid_SelectionChanged;
        role.Location=new Point(28,520);role.Size=new Size(190,30);role.DropDownStyle=ComboBoxStyle.DropDownList;role.Items.AddRange(new object[]{"MANAGER","CASHIER","WAREHOUSE"});
        employee.Location=new Point(238,520);employee.Size=new Size(250,30);employee.DropDownStyle=ComboBoxStyle.DropDownList;active.Location=new Point(510,522);active.AutoSize=true;active.Text="Được đăng nhập";
        save.Location=new Point(28,565);save.Size=new Size(140,36);save.Text="Lưu quyền";save.Click+=save_Click;add.Location=new Point(180,565);add.Size=new Size(160,36);add.Text="Đăng ký tài khoản";add.Click+=add_Click;refresh.Location=new Point(352,565);refresh.Size=new Size(110,36);refresh.Text="Tải lại";refresh.Click+=refresh_Click;
        statusLabel.AutoSize=true;statusLabel.Location=new Point(28,615);statusLabel.Anchor=AnchorStyles.Bottom|AnchorStyles.Left;
        Controls.AddRange(new Control[]{title,accountsGrid,role,employee,active,save,add,refresh,statusLabel});((System.ComponentModel.ISupportInitialize)accountsGrid).EndInit();ResumeLayout(false);PerformLayout();
    }
    protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}
}
