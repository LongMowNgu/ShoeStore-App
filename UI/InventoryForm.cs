using System.Data;
using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class InventoryForm : Form
{
    private readonly UserSession _actor; private readonly InventoryService _service=new(); private int? _variantId;
    public InventoryForm(UserSession actor)
    {
        _actor=actor;actor.Require("WAREHOUSE");InitializeComponent();employeeId.Value=actor.EmployeeId??0;employeeId.Enabled=actor.IsManager;
        Load+=(_,_)=>Reload();
    }
    private void search_Click(object? s,EventArgs e)=>Reload(searchBox.Text);
    private void refresh_Click(object? s,EventArgs e){searchBox.Clear();Reload();}
    private void stockGrid_SelectionChanged(object? s,EventArgs e)
    {if(stockGrid.CurrentRow?.DataBoundItem is DataRowView row){_variantId=Convert.ToInt32(row["VARIANT_ID"]);unitCost.Value=Math.Clamp(Convert.ToDecimal(row["COST_PRICE"]),unitCost.Minimum,unitCost.Maximum);statusLabel.Text=Convert.ToString(row["STOCK_ALERT"])=="LOW"?"Cảnh báo: tồn kho đang bằng hoặc thấp hơn ngưỡng nhập.":"Tồn kho đang trên ngưỡng cảnh báo.";}}
    private void receiveButton_Click(object? s,EventArgs e)
    {
        if(_variantId is null){Warn("Chọn biến thể giày cần nhập.");return;}
        try{var emp=_actor.IsManager&&employeeId.Value>0?(int?)Convert.ToInt32(employeeId.Value):_actor.EmployeeId;var code=_service.Receive(_actor,emp,_variantId.Value,(int)quantity.Value,unitCost.Value,supplierBox.Text);Reload(searchBox.Text);statusLabel.Text=$"Đã lưu phiếu nhập {code}; tồn kho đã tăng.";}
        catch(Exception ex){MessageBox.Show(ex.Message,"Không thể nhập kho",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private void Reload(string keyword="")
    {
        try{stockGrid.DataSource=_service.LoadStock(keyword);Header("SKU","SKU");Header("ITEM","Sản phẩm / cỡ / màu");Header("STOCK_QTY","Tồn");Header("REORDER_LEVEL","Ngưỡng");Header("COST_PRICE","Giá vốn");Header("SELL_PRICE","Giá bán");Header("STOCK_ALERT","Cảnh báo");if(stockGrid.Columns.Contains("VARIANT_ID"))stockGrid.Columns["VARIANT_ID"].Visible=false;movementsGrid.DataSource=_service.Movements();if(movementsGrid.Columns.Count>0){movementsGrid.Columns["MOVED_AT"].HeaderText="Thời gian";movementsGrid.Columns["MOVEMENT_TYPE"].HeaderText="Nghiệp vụ";movementsGrid.Columns["SKU"].HeaderText="SKU";movementsGrid.Columns["QUANTITY_DELTA"].HeaderText="Thay đổi";movementsGrid.Columns["STOCK_AFTER"].HeaderText="Tồn sau";movementsGrid.Columns["REFERENCE_CODE"].HeaderText="Chứng từ";movementsGrid.Columns["USERNAME"].HeaderText="Tài khoản";} }
        catch(Exception ex){MessageBox.Show(ex.Message,"Không tải được kho",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private void Header(string key,string value){if(stockGrid.Columns.Contains(key))stockGrid.Columns[key].HeaderText=value;}
    private static void Warn(string text)=>MessageBox.Show(text,"Kiểm tra",MessageBoxButtons.OK,MessageBoxIcon.Warning);
}
