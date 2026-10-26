using System.Data;
using System.Runtime.Versioning;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms.UI;

[SupportedOSPlatform("windows")]
internal partial class AccountManagementForm:Form
{
    private readonly UserSession _actor;private readonly AccountRepository _repository=new();private int? _userId;
    public AccountManagementForm(UserSession actor){_actor=actor;actor.Require("MANAGER");InitializeComponent();Load+=(_,_)=>Reload();}
    private void accountsGrid_SelectionChanged(object? s,EventArgs e)
    {if(accountsGrid.CurrentRow?.DataBoundItem is not DataRowView r)return;_userId=Convert.ToInt32(r["USER_ID"]);role.SelectedItem=Convert.ToString(r["ROLE_CODE"]);active.Checked=Convert.ToInt32(r["IS_ACTIVE"])==1;if(r["EMPLOYEE_ID"]==DBNull.Value)employee.SelectedIndex=-1;else employee.SelectedValue=Convert.ToInt32(r["EMPLOYEE_ID"]);statusLabel.Text=$"Chọn tài khoản {r["USERNAME"]} để đổi vai trò hoặc khóa/mở.";}
    private void save_Click(object? s,EventArgs e)
    {if(_userId is null||role.SelectedItem is null){Warn("Chọn tài khoản cần cập nhật.");return;}try{int? emp=employee.SelectedValue is null||employee.SelectedValue==DBNull.Value?-1:Convert.ToInt32(employee.SelectedValue);_repository.Update(_actor,_userId.Value,role.SelectedItem.ToString()!,emp<0?null:emp,active.Checked);Reload();statusLabel.Text="Đã cập nhật vai trò/tình trạng tài khoản.";}catch(Exception ex){MessageBox.Show(ex.Message,"Không thể cập nhật tài khoản",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
    private void add_Click(object? s,EventArgs e){using var form=new CreateAccountForm(false,_actor);if(form.ShowDialog(this)==DialogResult.OK){Reload();statusLabel.Text="Đã tạo tài khoản nhân viên.";}}
    private void refresh_Click(object? s,EventArgs e)=>Reload();
    private void Reload()
    {
        try{var employees=_repository.Employees(_actor);employee.DataSource=employees;employee.DisplayMember="FULL_NAME";employee.ValueMember="EMPLOYEE_ID";employee.SelectedIndex=-1;accountsGrid.DataSource=_repository.Accounts(_actor);var labels=new[]{("USERNAME","Tên đăng nhập"),("ROLE_CODE","Vai trò"),("EMPLOYEE_ID","Mã NV"),("FULL_NAME","Nhân viên"),("IS_ACTIVE","Được đăng nhập"),("CREATED_AT","Ngày tạo")};foreach(var(k,v)in labels)if(accountsGrid.Columns.Contains(k))accountsGrid.Columns[k].HeaderText=v;if(accountsGrid.Columns.Contains("USER_ID"))accountsGrid.Columns["USER_ID"].Visible=false;_userId=null;}
        catch(Exception ex){MessageBox.Show(ex.Message,"Không tải được tài khoản",MessageBoxButtons.OK,MessageBoxIcon.Error);}
    }
    private static void Warn(string text)=>MessageBox.Show(text,"Kiểm tra",MessageBoxButtons.OK,MessageBoxIcon.Warning);
}
