#nullable enable
namespace QuanLyBanGiay;

partial class ReceiptForm
{
    private System.ComponentModel.IContainer? components; private Label headerLabel = null!; private DataGridView linesGrid = null!; private Button printButton = null!; private Button closeButton = null!;
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container(); headerLabel = new Label(); linesGrid = new DataGridView(); printButton = new Button(); closeButton = new Button();
        ((System.ComponentModel.ISupportInitialize)linesGrid).BeginInit(); SuspendLayout();
        Text="Hóa đơn bán hàng"; StartPosition=FormStartPosition.CenterParent; ClientSize=new Size(760,520); MinimumSize=new Size(680,450); Font=new Font("Segoe UI",9.5F);
        headerLabel.Location=new Point(22,18); headerLabel.Size=new Size(710,80); headerLabel.Font=new Font("Segoe UI",12F,FontStyle.Bold); headerLabel.Text="Hóa đơn";
        linesGrid.Location=new Point(22,108); linesGrid.Size=new Size(710,330); linesGrid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right; linesGrid.ReadOnly=true; linesGrid.AllowUserToAddRows=false; linesGrid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill; linesGrid.RowHeadersVisible=false;
        printButton.Location=new Point(480,460); printButton.Size=new Size(120,36); printButton.Anchor=AnchorStyles.Bottom|AnchorStyles.Right; printButton.Text="Xem trước / In"; printButton.Click+=printButton_Click;
        closeButton.Location=new Point(612,460); closeButton.Size=new Size(120,36); closeButton.Anchor=AnchorStyles.Bottom|AnchorStyles.Right; closeButton.Text="Đóng"; closeButton.Click+=closeButton_Click;
        Controls.AddRange(new Control[]{headerLabel,linesGrid,printButton,closeButton}); ((System.ComponentModel.ISupportInitialize)linesGrid).EndInit(); ResumeLayout(false);
    }
    protected override void Dispose(bool disposing) { if(disposing) components?.Dispose(); base.Dispose(disposing); }
}

