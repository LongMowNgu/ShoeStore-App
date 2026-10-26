#nullable enable
namespace ShoeStore.WinForms.UI;

partial class ReportsForm
{
    private System.ComponentModel.IContainer? components;private DateTimePicker fromDate=null!;private DateTimePicker toDate=null!;private Label summaryLabel=null!;private DataGridView salesGrid=null!;private DataGridView sellersGrid=null!;private DataGridView lowStockGrid=null!;private DataGridView orderStatusGrid=null!;
    private void InitializeComponent()
    {
        components=new System.ComponentModel.Container();fromDate=new DateTimePicker();toDate=new DateTimePicker();summaryLabel=new Label();salesGrid=new DataGridView();sellersGrid=new DataGridView();lowStockGrid=new DataGridView();orderStatusGrid=new DataGridView();var title=new Label();var load=new Button();var export=new Button();var tabs=new TabControl();
        ((System.ComponentModel.ISupportInitialize)salesGrid).BeginInit();((System.ComponentModel.ISupportInitialize)sellersGrid).BeginInit();((System.ComponentModel.ISupportInitialize)lowStockGrid).BeginInit();((System.ComponentModel.ISupportInitialize)orderStatusGrid).BeginInit();SuspendLayout();
        Text="Thống kê và báo cáo";StartPosition=FormStartPosition.CenterParent;WindowState=FormWindowState.Maximized;MinimumSize=new Size(1050,700);Font=new Font("Segoe UI",9.5F);
        title.AutoSize=true;title.Font=new Font("Segoe UI",20F,FontStyle.Bold);title.Location=new Point(24,18);title.Text="THỐNG KÊ VÀ BÁO CÁO";
        fromDate.Location=new Point(28,68);fromDate.Size=new Size(220,28);toDate.Location=new Point(270,68);toDate.Size=new Size(220,28);var fromLabel=new Label{Text="Từ ngày",Location=new Point(28,48),AutoSize=true};var toLabel=new Label{Text="Đến ngày",Location=new Point(270,48),AutoSize=true};
        load.Location=new Point(510,66);load.Size=new Size(120,34);load.Text="Tạo báo cáo";load.Click+=load_Click;export.Location=new Point(640,66);export.Size=new Size(120,34);export.Text="Xuất CSV";export.Click+=export_Click;
        summaryLabel.Location=new Point(28,112);summaryLabel.Size=new Size(980,52);summaryLabel.Font=new Font("Segoe UI",12F,FontStyle.Bold);
        Configure(salesGrid);Configure(sellersGrid);Configure(lowStockGrid);Configure(orderStatusGrid);AddTab(tabs,"Doanh thu / giá vốn",salesGrid);AddTab(tabs,"Sản phẩm bán chạy",sellersGrid);AddTab(tabs,"Cảnh báo tồn",lowStockGrid);AddTab(tabs,"Đơn theo trạng thái",orderStatusGrid);tabs.Location=new Point(28,175);tabs.Size=new Size(1000,445);tabs.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
        Controls.AddRange(new Control[]{title,fromLabel,fromDate,toLabel,toDate,load,export,summaryLabel,tabs});((System.ComponentModel.ISupportInitialize)salesGrid).EndInit();((System.ComponentModel.ISupportInitialize)sellersGrid).EndInit();((System.ComponentModel.ISupportInitialize)lowStockGrid).EndInit();((System.ComponentModel.ISupportInitialize)orderStatusGrid).EndInit();ResumeLayout(false);PerformLayout();
    }
    private static void Configure(DataGridView g){g.Dock=DockStyle.Fill;g.ReadOnly=true;g.AllowUserToAddRows=false;g.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;g.RowHeadersVisible=false;}
    private static void AddTab(TabControl t,string name,Control c){var tab=new TabPage(name);tab.Controls.Add(c);t.TabPages.Add(tab);}
    protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}
}
