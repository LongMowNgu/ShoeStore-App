#nullable enable
namespace QuanLyBanGiay;

partial class ReportsForm
{
    private System.ComponentModel.IContainer? components;
    private DateTimePicker fromDate = null!;
    private DateTimePicker toDate = null!;
    private Label summaryLabel = null!;
    private DataGridView salesGrid = null!;
    private DataGridView sellersGrid = null!;
    private DataGridView lowStockGrid = null!;
    private DataGridView orderStatusGrid = null!;
    private Label title = null!;
    private Label fromLabel = null!;
    private Label toLabel = null!;
    private Button load = null!;
    private Button export = null!;
    private TabControl tabs = null!;
    private TabPage salesTab = null!;
    private TabPage sellersTab = null!;
    private TabPage lowStockTab = null!;
    private TabPage orderStatusTab = null!;
    private TableLayoutPanel rootLayout = null!;
    private FlowLayoutPanel filterPanel = null!;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        fromDate = new DateTimePicker();
        toDate = new DateTimePicker();
        summaryLabel = new Label();
        salesGrid = new DataGridView();
        sellersGrid = new DataGridView();
        lowStockGrid = new DataGridView();
        orderStatusGrid = new DataGridView();
        title = new Label();
        fromLabel = new Label();
        toLabel = new Label();
        load = new Button();
        export = new Button();
        tabs = new TabControl();
        salesTab = new TabPage();
        sellersTab = new TabPage();
        lowStockTab = new TabPage();
        orderStatusTab = new TabPage();
        rootLayout = new TableLayoutPanel();
        filterPanel = new FlowLayoutPanel();
        ((System.ComponentModel.ISupportInitialize)salesGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sellersGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lowStockGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)orderStatusGrid).BeginInit();
        tabs.SuspendLayout();
        salesTab.SuspendLayout();
        sellersTab.SuspendLayout();
        lowStockTab.SuspendLayout();
        orderStatusTab.SuspendLayout();
        rootLayout.SuspendLayout();
        filterPanel.SuspendLayout();
        SuspendLayout();
        // 
        // fromDate
        // 
        fromDate.Format = DateTimePickerFormat.Short;
        fromDate.Location = new Point(71, 7);
        fromDate.Margin = new Padding(0, 4, 18, 0);
        fromDate.Name = "fromDate";
        fromDate.Size = new Size(150, 29);
        fromDate.TabIndex = 1;
        // 
        // toDate
        // 
        toDate.Format = DateTimePickerFormat.Short;
        toDate.Location = new Point(321, 7);
        toDate.Margin = new Padding(0, 4, 18, 0);
        toDate.Name = "toDate";
        toDate.Size = new Size(150, 29);
        toDate.TabIndex = 3;
        // 
        // summaryLabel
        // 
        summaryLabel.AutoEllipsis = true;
        summaryLabel.BackColor = Color.White;
        summaryLabel.Dock = DockStyle.Fill;
        summaryLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        summaryLabel.ForeColor = Color.FromArgb(40, 91, 145);
        summaryLabel.Location = new Point(27, 134);
        summaryLabel.Name = "summaryLabel";
        summaryLabel.Padding = new Padding(8);
        summaryLabel.Size = new Size(1186, 54);
        summaryLabel.TabIndex = 2;
        summaryLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // salesGrid
        // 
        salesGrid.AllowUserToAddRows = false;
        salesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        salesGrid.BackgroundColor = Color.White;
        salesGrid.ColumnHeadersHeight = 29;
        salesGrid.Dock = DockStyle.Fill;
        salesGrid.Location = new Point(0, 0);
        salesGrid.Name = "salesGrid";
        salesGrid.ReadOnly = true;
        salesGrid.RowHeadersVisible = false;
        salesGrid.RowHeadersWidth = 51;
        salesGrid.Size = new Size(1184, 542);
        salesGrid.TabIndex = 0;
        // 
        // sellersGrid
        // 
        sellersGrid.AllowUserToAddRows = false;
        sellersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        sellersGrid.BackgroundColor = Color.White;
        sellersGrid.ColumnHeadersHeight = 29;
        sellersGrid.Dock = DockStyle.Fill;
        sellersGrid.Location = new Point(0, 0);
        sellersGrid.Name = "sellersGrid";
        sellersGrid.ReadOnly = true;
        sellersGrid.RowHeadersVisible = false;
        sellersGrid.RowHeadersWidth = 51;
        sellersGrid.Size = new Size(144, 0);
        sellersGrid.TabIndex = 0;
        // 
        // lowStockGrid
        // 
        lowStockGrid.AllowUserToAddRows = false;
        lowStockGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        lowStockGrid.BackgroundColor = Color.White;
        lowStockGrid.ColumnHeadersHeight = 29;
        lowStockGrid.Dock = DockStyle.Fill;
        lowStockGrid.Location = new Point(0, 0);
        lowStockGrid.Name = "lowStockGrid";
        lowStockGrid.ReadOnly = true;
        lowStockGrid.RowHeadersVisible = false;
        lowStockGrid.RowHeadersWidth = 51;
        lowStockGrid.Size = new Size(144, 0);
        lowStockGrid.TabIndex = 0;
        // 
        // orderStatusGrid
        // 
        orderStatusGrid.AllowUserToAddRows = false;
        orderStatusGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        orderStatusGrid.BackgroundColor = Color.White;
        orderStatusGrid.ColumnHeadersHeight = 29;
        orderStatusGrid.Dock = DockStyle.Fill;
        orderStatusGrid.Location = new Point(0, 0);
        orderStatusGrid.Name = "orderStatusGrid";
        orderStatusGrid.ReadOnly = true;
        orderStatusGrid.RowHeadersVisible = false;
        orderStatusGrid.RowHeadersWidth = 51;
        orderStatusGrid.Size = new Size(144, 0);
        orderStatusGrid.TabIndex = 0;
        // 
        // title
        // 
        title.Dock = DockStyle.Fill;
        title.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        title.ForeColor = Color.FromArgb(40, 91, 145);
        title.Location = new Point(27, 18);
        title.Name = "title";
        title.Size = new Size(1186, 58);
        title.TabIndex = 0;
        title.Text = "THỐNG KÊ VÀ BÁO CÁO";
        title.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // fromLabel
        // 
        fromLabel.AutoSize = true;
        fromLabel.Location = new Point(0, 12);
        fromLabel.Margin = new Padding(0, 9, 6, 0);
        fromLabel.Name = "fromLabel";
        fromLabel.Size = new Size(65, 21);
        fromLabel.TabIndex = 0;
        fromLabel.Text = "Từ ngày";
        // 
        // toLabel
        // 
        toLabel.AutoSize = true;
        toLabel.Location = new Point(239, 12);
        toLabel.Margin = new Padding(0, 9, 6, 0);
        toLabel.Name = "toLabel";
        toLabel.Size = new Size(76, 21);
        toLabel.TabIndex = 2;
        toLabel.Text = "Đến ngày";
        // 
        // load
        // 
        load.Location = new Point(489, 3);
        load.Margin = new Padding(0, 0, 8, 0);
        load.Name = "load";
        load.Size = new Size(125, 34);
        load.TabIndex = 4;
        load.Text = "Tạo báo cáo";
        load.Click += load_Click;
        // 
        // export
        // 
        export.Location = new Point(625, 6);
        export.Name = "export";
        export.Size = new Size(120, 34);
        export.TabIndex = 5;
        export.Text = "Xuất CSV";
        export.Click += export_Click;
        // 
        // tabs
        // 
        tabs.Controls.Add(salesTab);
        tabs.Controls.Add(sellersTab);
        tabs.Controls.Add(lowStockTab);
        tabs.Controls.Add(orderStatusTab);
        tabs.Dock = DockStyle.Fill;
        tabs.Location = new Point(24, 188);
        tabs.Margin = new Padding(0);
        tabs.Name = "tabs";
        tabs.SelectedIndex = 0;
        tabs.Size = new Size(1192, 576);
        tabs.TabIndex = 3;
        // 
        // salesTab
        // 
        salesTab.Controls.Add(salesGrid);
        salesTab.Location = new Point(4, 30);
        salesTab.Name = "salesTab";
        salesTab.Size = new Size(1184, 542);
        salesTab.TabIndex = 0;
        salesTab.Text = "Doanh thu / giá vốn";
        // 
        // sellersTab
        // 
        sellersTab.Controls.Add(sellersGrid);
        sellersTab.Location = new Point(4, 30);
        sellersTab.Name = "sellersTab";
        sellersTab.Size = new Size(144, 0);
        sellersTab.TabIndex = 1;
        sellersTab.Text = "Sản phẩm bán chạy";
        // 
        // lowStockTab
        // 
        lowStockTab.Controls.Add(lowStockGrid);
        lowStockTab.Location = new Point(4, 30);
        lowStockTab.Name = "lowStockTab";
        lowStockTab.Size = new Size(144, 0);
        lowStockTab.TabIndex = 2;
        lowStockTab.Text = "Cảnh báo tồn";
        // 
        // orderStatusTab
        // 
        orderStatusTab.Controls.Add(orderStatusGrid);
        orderStatusTab.Location = new Point(4, 30);
        orderStatusTab.Name = "orderStatusTab";
        orderStatusTab.Size = new Size(144, 0);
        orderStatusTab.TabIndex = 3;
        orderStatusTab.Text = "Đơn theo trạng thái";
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(title, 0, 0);
        rootLayout.Controls.Add(filterPanel, 0, 1);
        rootLayout.Controls.Add(summaryLabel, 0, 2);
        rootLayout.Controls.Add(tabs, 0, 3);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(24, 18, 24, 16);
        rootLayout.RowCount = 4;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.Size = new Size(1240, 780);
        rootLayout.TabIndex = 0;
        // 
        // filterPanel
        // 
        filterPanel.Controls.Add(fromLabel);
        filterPanel.Controls.Add(fromDate);
        filterPanel.Controls.Add(toLabel);
        filterPanel.Controls.Add(toDate);
        filterPanel.Controls.Add(load);
        filterPanel.Controls.Add(export);
        filterPanel.Dock = DockStyle.Fill;
        filterPanel.Location = new Point(24, 76);
        filterPanel.Margin = new Padding(0);
        filterPanel.Name = "filterPanel";
        filterPanel.Padding = new Padding(0, 3, 0, 0);
        filterPanel.Size = new Size(1192, 58);
        filterPanel.TabIndex = 1;
        filterPanel.WrapContents = false;
        // 
        // ReportsForm
        // 
        AutoScaleDimensions = new SizeF(9F, 21F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(244, 247, 251);
        ClientSize = new Size(1240, 780);
        Controls.Add(rootLayout);
        Font = new Font("Segoe UI", 9.5F);
        MinimumSize = new Size(1050, 680);
        Name = "ReportsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thống kê và báo cáo";
        WindowState = FormWindowState.Maximized;
        ((System.ComponentModel.ISupportInitialize)salesGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)sellersGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)lowStockGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)orderStatusGrid).EndInit();
        tabs.ResumeLayout(false);
        salesTab.ResumeLayout(false);
        sellersTab.ResumeLayout(false);
        lowStockTab.ResumeLayout(false);
        orderStatusTab.ResumeLayout(false);
        rootLayout.ResumeLayout(false);
        filterPanel.ResumeLayout(false);
        filterPanel.PerformLayout();
        ResumeLayout(false);
    }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
