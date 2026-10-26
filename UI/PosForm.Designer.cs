#nullable enable
namespace ShoeStore.WinForms.UI;

partial class PosForm
{
    private System.ComponentModel.IContainer? components;
    private TextBox searchBox = null!; private DataGridView productsGrid = null!; private DataGridView cartGrid = null!;
    private NumericUpDown quantity = null!; private NumericUpDown employeeId = null!; private Label totalLabel = null!; private Label statusLabel = null!;
    private Button addButton = null!; private Button checkoutButton = null!;
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container(); searchBox = new TextBox(); productsGrid = new DataGridView(); cartGrid = new DataGridView();
        quantity = new NumericUpDown(); employeeId = new NumericUpDown(); totalLabel = new Label(); statusLabel = new Label(); addButton = new Button(); checkoutButton = new Button();
        var search = new Button(); var reload = new Button(); var title = new Label(); var qtyLabel = new Label(); var empLabel = new Label();
        ((System.ComponentModel.ISupportInitialize)productsGrid).BeginInit(); ((System.ComponentModel.ISupportInitialize)cartGrid).BeginInit(); ((System.ComponentModel.ISupportInitialize)quantity).BeginInit(); ((System.ComponentModel.ISupportInitialize)employeeId).BeginInit(); SuspendLayout();
        Text = "Bán hàng tại quầy"; StartPosition = FormStartPosition.CenterParent; WindowState = FormWindowState.Maximized; MinimumSize = new Size(1080, 700); Font = new Font("Segoe UI", 9.5F);
        title.AutoSize = true; title.Font = new Font("Segoe UI", 20F, FontStyle.Bold); title.Location = new Point(24, 18); title.Text = "BÁN HÀNG TẠI QUẦY";
        searchBox.Location = new Point(28, 70); searchBox.Size = new Size(320, 28); searchBox.PlaceholderText = "Tìm tên giày hoặc SKU";
        search.Location = new Point(360, 68); search.Size = new Size(90, 32); search.Text = "Tìm"; search.Click += search_Click;
        reload.Location = new Point(458, 68); reload.Size = new Size(90, 32); reload.Text = "Tải lại"; reload.Click += reload_Click;
        productsGrid.Location = new Point(28, 112); productsGrid.Size = new Size(510, 270); productsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; ConfigureGrid(productsGrid); productsGrid.SelectionChanged += productsGrid_SelectionChanged;
        qtyLabel.AutoSize = true; qtyLabel.Location = new Point(28, 390); qtyLabel.Text = "Số lượng"; quantity.Location = new Point(100, 386); quantity.Size = new Size(90, 28); quantity.Minimum = 1; quantity.Maximum = 100000;
        addButton.Location = new Point(205, 384); addButton.Size = new Size(145, 34); addButton.Text = "Thêm vào giỏ"; addButton.Click += addButton_Click;
        empLabel.AutoSize = true; empLabel.Location = new Point(28, 432); empLabel.Text = "Mã nhân viên (quản lý)"; employeeId.Location = new Point(205, 428); employeeId.Size = new Size(145, 28); employeeId.Maximum = 1000000000;
        cartGrid.Location = new Point(565, 70); cartGrid.Size = new Size(600, 430); cartGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; ConfigureGrid(cartGrid);
        totalLabel.AutoSize = true; totalLabel.Font = new Font("Segoe UI", 17F, FontStyle.Bold); totalLabel.Location = new Point(565, 516); totalLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; totalLabel.Text = "Tổng: 0 VNĐ";
        checkoutButton.Location = new Point(960, 510); checkoutButton.Size = new Size(205, 44); checkoutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right; checkoutButton.Text = "Thanh toán / Xuất hóa đơn"; checkoutButton.Click += checkoutButton_Click;
        statusLabel.AutoSize = true; statusLabel.Location = new Point(30, 630); statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        Controls.AddRange(new Control[] { title, searchBox, search, reload, productsGrid, qtyLabel, quantity, addButton, empLabel, employeeId, cartGrid, totalLabel, checkoutButton, statusLabel });
        ((System.ComponentModel.ISupportInitialize)productsGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)cartGrid).EndInit(); ((System.ComponentModel.ISupportInitialize)quantity).EndInit(); ((System.ComponentModel.ISupportInitialize)employeeId).EndInit(); ResumeLayout(false); PerformLayout();
    }
    private static void ConfigureGrid(DataGridView grid) { grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.MultiSelect = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.RowHeadersVisible = false; }
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
}
