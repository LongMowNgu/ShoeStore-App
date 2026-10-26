#nullable enable
namespace ShoeStore.WinForms.UI;

partial class InventoryForm
{
    private System.ComponentModel.IContainer? components; private DataGridView stockGrid=null!; private DataGridView movementsGrid=null!; private TextBox searchBox=null!; private TextBox supplierBox=null!; private NumericUpDown quantity=null!; private NumericUpDown unitCost=null!; private NumericUpDown employeeId=null!; private Label statusLabel=null!; private Button receiveButton=null!;
    private void InitializeComponent()
    {
        components=new System.ComponentModel.Container(); stockGrid=new DataGridView(); movementsGrid=new DataGridView(); searchBox=new TextBox(); supplierBox=new TextBox(); quantity=new NumericUpDown(); unitCost=new NumericUpDown(); employeeId=new NumericUpDown(); statusLabel=new Label(); receiveButton=new Button(); var title=new Label(); var search=new Button(); var refresh=new Button(); var qtyLabel=new Label(); var costLabel=new Label(); var supplierLabel=new Label(); var empLabel=new Label(); var tabs=new TabControl();
        ((System.ComponentModel.ISupportInitialize)stockGrid).BeginInit();((System.ComponentModel.ISupportInitialize)movementsGrid).BeginInit();((System.ComponentModel.ISupportInitialize)quantity).BeginInit();((System.ComponentModel.ISupportInitialize)unitCost).BeginInit();((System.ComponentModel.ISupportInitialize)employeeId).BeginInit();SuspendLayout();
        Text="Nhập hàng và tồn kho";StartPosition=FormStartPosition.CenterParent;WindowState=FormWindowState.Maximized;MinimumSize=new Size(1080,700);Font=new Font("Segoe UI",9.5F);
        title.AutoSize=true;title.Font=new Font("Segoe UI",20F,FontStyle.Bold);title.Location=new Point(24,18);title.Text="NHẬP HÀNG · TỒN KHO · CẢNH BÁO";
        searchBox.Location=new Point(28,68);searchBox.Size=new Size(300,28);searchBox.PlaceholderText="Tìm SKU hoặc tên mẫu";search.Location=new Point(338,66);search.Size=new Size(88,32);search.Text="Tìm";search.Click+=search_Click;refresh.Location=new Point(434,66);refresh.Size=new Size(92,32);refresh.Text="Tải lại";refresh.Click+=refresh_Click;
        stockGrid.Dock=DockStyle.Fill;Configure(stockGrid);stockGrid.SelectionChanged+=stockGrid_SelectionChanged;movementsGrid.Dock=DockStyle.Fill;Configure(movementsGrid);
        var stockTab=new TabPage("Tồn kho và nhập hàng");var movementTab=new TabPage("Lịch sử nhập/bán/đơn");stockTab.Controls.Add(stockGrid);movementTab.Controls.Add(movementsGrid);tabs.Location=new Point(28,112);tabs.Size=new Size(1040,420);tabs.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;tabs.TabPages.Add(stockTab);tabs.TabPages.Add(movementTab);
        qtyLabel.Location=new Point(28,550);qtyLabel.AutoSize=true;qtyLabel.Text="Số lượng nhập";quantity.Location=new Point(145,546);quantity.Maximum=1000000;quantity.Minimum=1;quantity.Value=1;
        costLabel.Location=new Point(300,550);costLabel.AutoSize=true;costLabel.Text="Giá nhập/đôi";unitCost.Location=new Point(405,546);unitCost.Maximum=1000000000;unitCost.DecimalPlaces=0;unitCost.ThousandsSeparator=true;
        supplierLabel.Location=new Point(28,592);supplierLabel.AutoSize=true;supplierLabel.Text="Nhà cung cấp";supplierBox.Location=new Point(145,588);supplierBox.Size=new Size(280,28);supplierBox.MaxLength=150;
        empLabel.Location=new Point(455,592);empLabel.AutoSize=true;empLabel.Text="Mã NV lập phiếu (quản lý)";employeeId.Location=new Point(630,588);employeeId.Maximum=1000000000;
        receiveButton.Location=new Point(820,580);receiveButton.Size=new Size(180,40);receiveButton.Text="Lưu phiếu nhập";receiveButton.Click+=receiveButton_Click;
        statusLabel.AutoSize=true;statusLabel.Location=new Point(30,640);statusLabel.Anchor=AnchorStyles.Bottom|AnchorStyles.Left;
        Controls.AddRange(new Control[]{title,searchBox,search,refresh,tabs,qtyLabel,quantity,costLabel,unitCost,supplierLabel,supplierBox,empLabel,employeeId,receiveButton,statusLabel});
        ((System.ComponentModel.ISupportInitialize)stockGrid).EndInit();((System.ComponentModel.ISupportInitialize)movementsGrid).EndInit();((System.ComponentModel.ISupportInitialize)quantity).EndInit();((System.ComponentModel.ISupportInitialize)unitCost).EndInit();((System.ComponentModel.ISupportInitialize)employeeId).EndInit();ResumeLayout(false);PerformLayout();
    }
    private static void Configure(DataGridView g){g.ReadOnly=true;g.AllowUserToAddRows=false;g.SelectionMode=DataGridViewSelectionMode.FullRowSelect;g.MultiSelect=false;g.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;g.RowHeadersVisible=false;}
    protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}
}
