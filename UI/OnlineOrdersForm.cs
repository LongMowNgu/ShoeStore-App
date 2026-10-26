using System.Data;
using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class OnlineOrdersForm : Form
{
    private readonly UserSession _actor;
    private readonly OrderService _orders = new();
    private int? _orderId;

    public OnlineOrdersForm(UserSession actor)
    {
        _actor = actor; actor.Require("CASHIER", "WAREHOUSE"); InitializeComponent();
        Load += (_, _) => { LoadVariants(); LoadStaff(); Reload(); };
        ordersGrid.SelectionChanged += ordersGrid_SelectionChanged;
    }

    private void ordersGrid_SelectionChanged(object? sender, EventArgs e)
    {
        if (ordersGrid.CurrentRow?.DataBoundItem is not DataRowView row) { _orderId = null; return; }
        _orderId = Convert.ToInt32(row["ORDER_ID"]);
        statusLabel.Text = $"Đơn {row["ORDER_CODE"]} · {row["CUSTOMER_NAME"]} · trạng thái {row["ORDER_STATUS"]} · {row["ASSIGNEE"]}";
        LoadHistory(_orderId.Value);
    }

    private void create_Click(object? sender, EventArgs e)
    {
        try
        {
            if (variant.SelectedValue is null) throw new InvalidOperationException("Chọn sản phẩm cần đặt.");
            var code = _orders.Create(_actor, customerName.Text, phone.Text, address.Text, Convert.ToInt32(variant.SelectedValue), (int)quantity.Value);
            customerName.Clear(); phone.Clear(); address.Clear(); Reload(); statusLabel.Text = $"Đã tiếp nhận đơn {code} ở trạng thái chờ xác nhận.";
        }
        catch (Exception ex) { ShowError("Không thể tạo đơn", ex); }
    }

    private void saveStatus_Click(object? sender, EventArgs e)
    {
        if (_orderId is null || status.SelectedItem is null) { Warn("Chọn đơn và trạng thái tiếp theo."); return; }
        var target = status.SelectedItem.ToString()!;
        if (MessageBox.Show($"Chuyển đơn sang {target}? Hệ thống sẽ giữ hoặc hoàn hàng theo trạng thái.", "Xác nhận xử lý", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try { _orders.UpdateStatus(_actor, _orderId.Value, target, note.Text); note.Clear(); Reload(); statusLabel.Text = "Đã cập nhật trạng thái và lịch sử đơn."; }
        catch (Exception ex) { ShowError("Không thể đổi trạng thái", ex); }
    }

    private void assignButton_Click(object? sender, EventArgs e)
    {
        if (_orderId is null || assignee.SelectedValue is null) { Warn("Chọn đơn và nhân viên cần phân công."); return; }
        try { _orders.Assign(_actor, _orderId.Value, Convert.ToInt32(assignee.SelectedValue)); Reload(); statusLabel.Text = "Đã phân công đơn."; }
        catch (Exception ex) { ShowError("Không thể phân công", ex); }
    }

    private void search_Click(object? sender, EventArgs e) => Reload();
    private void reload_Click(object? sender, EventArgs e) => Reload();

    private void LoadVariants()
    {
        try { variant.DataSource = _orders.OrderableVariants(); variant.DisplayMember = "ITEM"; variant.ValueMember = "VARIANT_ID"; }
        catch (Exception ex) { statusLabel.Text = "Chưa tải được danh sách sản phẩm: " + ex.Message; statusLabel.ForeColor = Color.Firebrick; }
    }

    private void LoadStaff()
    {
        try { assignee.DataSource = _orders.AssignableStaff(); assignee.DisplayMember = "DISPLAY_NAME"; assignee.ValueMember = "USER_ID"; }
        catch (Exception ex) { statusLabel.Text = "Chưa tải được nhân viên: " + ex.Message; statusLabel.ForeColor = Color.Firebrick; }
    }

    private void Reload()
    {
        try
        {
            var selected = _orderId;
            ordersGrid.DataSource = _orders.List(searchBox.Text.Trim(), statusFilter.SelectedIndex <= 0 ? "" : statusFilter.SelectedItem?.ToString() ?? "");
            emptyOrdersLabel.Visible = ordersGrid.Rows.Count == 0;
            var headers = new (string Key, string Caption)[] { ("ORDER_ID", "ID"), ("ORDER_CODE", "Mã đơn"), ("CUSTOMER_NAME", "Khách hàng"), ("CUSTOMER_PHONE", "Điện thoại"), ("SHIPPING_ADDRESS", "Địa chỉ"), ("ORDER_STATUS", "Trạng thái"), ("STOCK_RESERVED", "Giữ hàng"), ("CREATED_AT", "Ngày tạo"), ("TOTAL_AMOUNT", "Tổng tiền"), ("ASSIGNEE", "Nhân viên xử lý") };
            foreach (var (key, caption) in headers) if (ordersGrid.Columns.Contains(key)) { ordersGrid.Columns[key].HeaderText = caption; if (key == "ORDER_ID" || key == "STOCK_RESERVED") ordersGrid.Columns[key].Visible = false; }
            if (ordersGrid.Columns.Contains("TOTAL_AMOUNT")) ordersGrid.Columns["TOTAL_AMOUNT"].DefaultCellStyle.Format = "N0";
            if (ordersGrid.Columns.Contains("CREATED_AT")) ordersGrid.Columns["CREATED_AT"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (selected.HasValue && ordersGrid.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => Convert.ToInt32(((DataRowView)r.DataBoundItem!).Row["ORDER_ID"]) == selected.Value) is { } selectedRow) selectedRow.Selected = true;
        }
        catch (Exception ex) { ordersGrid.DataSource = null; emptyOrdersLabel.Visible = true; emptyOrdersLabel.Text = "Không thể tải đơn hàng.\u000aKiểm tra kết nối Oracle và tải lại."; statusLabel.Text = "Ngoại tuyến: " + ex.Message; statusLabel.ForeColor = Color.Firebrick; }
    }

    private void LoadHistory(int orderId)
    {
        try
        {
            historyGrid.DataSource = _orders.History(orderId);
            var headers = new (string Key, string Caption)[] { ("CHANGED_AT", "Thời gian"), ("OLD_STATUS", "Từ trạng thái"), ("NEW_STATUS", "Sang trạng thái"), ("ACTOR", "Người thực hiện"), ("NOTE", "Ghi chú") };
            foreach (var (key, caption) in headers) if (historyGrid.Columns.Contains(key)) historyGrid.Columns[key].HeaderText = caption;
            if (historyGrid.Columns.Contains("CHANGED_AT")) historyGrid.Columns["CHANGED_AT"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
        }
        catch (Exception ex) { ShowError("Không tải được lịch sử", ex); }
    }

    private static void ShowError(string title, Exception ex) => MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    private static void Warn(string text) => MessageBox.Show(text, "Kiểm tra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}




