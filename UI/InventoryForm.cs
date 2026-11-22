using System.Data;
using System.Runtime.Versioning;

using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class InventoryForm : Form
{
    private readonly UserSession _actor = null!;
    private readonly InventoryService _service = new();
    private int? _variantId;

    public InventoryForm()
    {
        InitializeComponent();
        UiTheme.Apply(this);
        unitCost.Visible = costLabel.Visible = supplierBox.Visible = supplierLabel.Visible = employeeId.Visible = empLabel.Visible = false;
        receiveButton.Text = "Tăng tồn kho";
    }

    internal InventoryForm(UserSession actor)
    {
        _actor = actor;
        actor.Require("MANAGER");
        InitializeComponent();
        UiTheme.Apply(this);
        unitCost.Visible = costLabel.Visible = supplierBox.Visible = supplierLabel.Visible = employeeId.Visible = empLabel.Visible = false;
        receiveButton.Text = "Tăng tồn kho";
        employeeId.Value = actor.EmployeeId ?? 0;
        employeeId.Enabled = actor.IsManager;
        tabs.TabPages.Remove(movementTab); receiveBox.Text = "Điều chỉnh tồn kho (database không lưu lịch sử nhập)"; Load += (_, _) => Reload();
    }

    private void search_Click(object? sender, EventArgs e) => Reload(searchBox.Text, lowStockOnly.Checked);

    private void refresh_Click(object? sender, EventArgs e)
    {
        searchBox.Clear();
        if (lowStockOnly.Checked) lowStockOnly.Checked = false;
        else Reload();
    }

    private void lowStockOnly_CheckedChanged(object? sender, EventArgs e) => Reload(searchBox.Text, lowStockOnly.Checked);

    private void stockGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || stockGrid.Columns[e.ColumnIndex].Name != "STOCK_ALERT") return;
        var style = e.CellStyle;
        if (style is null) return;
        var low = Convert.ToString(e.Value) == "LOW";
        e.Value = low ? "Hết hàng" : "Còn hàng";
        e.FormattingApplied = true;
        style.ForeColor = low ? Color.FromArgb(177, 56, 56) : Color.FromArgb(38, 128, 92);
    }

    private void stockGrid_SelectionChanged(object? sender, EventArgs e)
    {
        if (stockGrid.CurrentRow?.DataBoundItem is not DataRowView row) return;
        _variantId = Convert.ToInt32(row["VARIANT_ID"]);
        // The handover schema does not include a cost-price column.
    }

    private void receiveButton_Click(object? sender, EventArgs e)
    {
        if (_variantId is null) { Warn("Chọn biến thể giày cần nhập."); return; }
        try
        {
            var employee = _actor.IsManager && employeeId.Value > 0 ? (int?)Convert.ToInt32(employeeId.Value) : _actor.EmployeeId;
            var code = _service.Receive(_actor, employee, _variantId.Value, (int)quantity.Value, unitCost.Value, supplierBox.Text);
            Reload(searchBox.Text, lowStockOnly.Checked);
            ShowStatus($"Đã tăng tồn kho biến thể {code}. Database không lưu phiếu/lịch sử nhập.", false);
        }
        catch (Exception ex) { ShowStatus("Không thể nhập kho: " + ex.Message, true); }
    }

    private void Reload(string keyword = "", bool lowOnly = false)
    {
        try
        {
            var stock = _service.LoadStock(keyword, lowOnly);
            _variantId = null;
            stockGrid.DataSource = stock;
            Header("SKU", "Mã biến thể");
            Header("ITEM", "Sản phẩm / cỡ / màu");
            Header("STOCK_QTY", "Tồn");
            Header("REORDER_LEVEL", "Ngưỡng");
            Header("COST_PRICE", "Giá vốn");
            Header("SELL_PRICE", "Giá bán");
            Header("STOCK_ALERT", "Cảnh báo");
            if (stockGrid.Columns.Contains("VARIANT_ID")) stockGrid.Columns["VARIANT_ID"].Visible = false;
            if (stockGrid.Columns.Contains("REORDER_LEVEL")) stockGrid.Columns["REORDER_LEVEL"].Visible = false;

            movementsGrid.DataSource = _service.Movements();
            if (movementsGrid.Columns.Count > 0)
            {
                movementsGrid.Columns["MOVED_AT"].HeaderText = "Thời gian";
                movementsGrid.Columns["MOVEMENT_TYPE"].HeaderText = "Nghiệp vụ";
                movementsGrid.Columns["SKU"].HeaderText = "SKU";
                movementsGrid.Columns["QUANTITY_DELTA"].HeaderText = "Thay đổi";
                movementsGrid.Columns["STOCK_AFTER"].HeaderText = "Tồn sau";
                movementsGrid.Columns["REFERENCE_CODE"].HeaderText = "Chứng từ";
                movementsGrid.Columns["USERNAME"].HeaderText = "Tài khoản";
            }

            var lowCount = stock.Select("STOCK_ALERT = 'LOW'").Length;
            var message = stock.Rows.Count == 0
                ? "Không tìm thấy biến thể phù hợp với bộ lọc."
                : $"{stock.Rows.Count} biến thể · {lowCount} cảnh báo tồn";
            ShowStatus(message, lowCount > 0);
        }
        catch (Exception ex) { ShowStatus("Không tải được kho: " + ex.Message, true); }
    }

    private void Header(string key, string value)
    {
        if (stockGrid.Columns.Contains(key)) stockGrid.Columns[key].HeaderText = value;
    }

    private void ShowStatus(string text, bool isError)
    {
        statusLabel.Text = text;
        statusLabel.ForeColor = isError ? Color.FromArgb(177, 56, 56) : Color.FromArgb(91, 107, 126);
    }

    private static void Warn(string text) => MessageBox.Show(text, "Kiểm tra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
internal sealed class InventoryService
{
    public DataTable LoadStock(string keyword="",bool lowOnly=false)
    {
        using var c=OracleConnectionFactory.Create();using var q=c.CreateCommand();q.BindByName=true;
        q.CommandText="SELECT v.VariantID AS VARIANT_ID,TO_CHAR(v.VariantID) AS SKU,p.ProductName||' / '||sz.SizeValue||' / '||cl.ColorName AS ITEM,v.StockQuantity AS STOCK_QTY,0 AS REORDER_LEVEL,p.Price AS SELL_PRICE,CASE WHEN v.StockQuantity=0 THEN 'LOW' ELSE 'OK' END AS STOCK_ALERT FROM ProductVariants v JOIN Products p ON p.ProductID=v.ProductID JOIN Sizes sz ON sz.SizeID=v.SizeID JOIN Colors cl ON cl.ColorID=v.ColorID WHERE p.Status='Active' AND (:q IS NULL OR UPPER(p.ProductName) LIKE '%'||UPPER(:q)||'%' OR TO_CHAR(v.VariantID) LIKE '%'||:q||'%') AND (:low=0 OR v.StockQuantity=0) ORDER BY v.StockQuantity,p.ProductName";
        q.Parameters.Add("q",OracleDbType.NVarchar2,150).Value=string.IsNullOrWhiteSpace(keyword)?DBNull.Value:keyword.Trim();q.Parameters.Add("low",OracleDbType.Int32).Value=lowOnly?1:0;var d=new DataTable();using var a=new OracleDataAdapter(q);a.Fill(d);return d;
    }
    public string Receive(UserSession actor,int? employeeId,int variantId,int quantity,decimal unitCost,string supplier)
    {
        actor.Require("MANAGER");if(quantity<=0)throw new ArgumentException("Số lượng phải lớn hơn 0.");using var c=OracleConnectionFactory.Create();c.Open();using var tx=c.BeginTransaction();try{using var q=c.CreateCommand();q.Transaction=tx;q.BindByName=true;q.CommandText="UPDATE ProductVariants SET StockQuantity=StockQuantity+:qty WHERE VariantID=:id AND StockQuantity+:qty<=99999";q.Parameters.Add("qty",OracleDbType.Int32).Value=quantity;q.Parameters.Add("id",OracleDbType.Int32).Value=variantId;if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Không tìm thấy biến thể hoặc tổng tồn vượt cột NUMBER(5) (99.999).");tx.Commit();return variantId.ToString();}catch{tx.Rollback();throw;}
    }
    public DataTable Movements(int take=300)=>new();
}
