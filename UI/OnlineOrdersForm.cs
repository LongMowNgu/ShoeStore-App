using System.Data;
using System.Runtime.Versioning;

using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class OnlineOrdersForm : Form
{
    private readonly UserSession _actor = null!;
    private readonly OrderService _orders = new();
    private int? _orderId;

    public OnlineOrdersForm()
    {
        InitializeComponent();
        UiTheme.Apply(this);
    }

    internal OnlineOrdersForm(UserSession actor)
    {
        _actor = actor;
        actor.Require("EMPLOYEE", "MANAGER");
        InitializeComponent();
        UiTheme.Apply(this);
        customerName.Visible = false; address.Visible = false;
        phone.PlaceholderText = "Số điện thoại khách đã đăng ký";
        statusFilter.Items.Clear(); statusFilter.Items.AddRange(new object[] { "Tất cả trạng thái", "Chờ xác nhận", "Đang giao", "Hoàn tất", "Đã hủy" }); statusFilter.SelectedIndex = 0;
        status.Items.Clear(); status.Items.AddRange(new object[] { "Đang giao", "Hoàn tất", "Đã hủy" });
        assignButton.Visible = _actor.IsManager;
        assignee.Enabled = _actor.IsManager;
        if(!_actor.IsManager){ saveStatus.Visible=false; status.Visible=false; note.Visible=false; }
        Load += (_, _) => { LoadVariants(); LoadStaff(); Reload(); };
    }

    private void ordersGrid_SelectionChanged(object? sender, EventArgs e)
    {
        if (ordersGrid.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            _orderId = null;
            assignmentInfo.Text = "Chọn đơn hàng để xem chi tiết.";
            return;
        }

        _orderId = Convert.ToInt32(row["ORDER_ID"]);
        var orderCode = Convert.ToString(row["ORDER_CODE"]) ?? string.Empty;
        var orderStatus = Convert.ToString(row["ORDER_STATUS"]) ?? string.Empty;
        statusLabel.Text = $"Đơn {orderCode} · {StatusDisplay(orderStatus)}.";
        statusLabel.ForeColor = Color.FromArgb(91, 107, 126);
        assignmentInfo.Text = Convert.ToString(row["ASSIGNED_TO"]) is { Length: > 0 } assigneeName
            ? $"Nhân viên phụ trách: {assigneeName}"
            : "Đơn hàng chưa được phân công.";
        if (detailsTabs.SelectedTab == historyTab) LoadHistory(_orderId.Value);
    }

    private void filterOrders_Click(object? sender, EventArgs e) => Reload();

    private void orderSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        Reload();
    }

    private void clearFilters_Click(object? sender, EventArgs e)
    {
        orderSearch.Clear();
        statusFilter.SelectedIndex = 0;
        Reload();
    }

    private void detailsTabs_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (detailsTabs.SelectedTab == historyTab && _orderId.HasValue)
            LoadHistory(_orderId.Value);
    }

    private void create_Click(object? sender, EventArgs e)
    {
        try
        {
            if (variant.SelectedValue is null) throw new InvalidOperationException("Chọn sản phẩm cần đặt.");
            var code = _orders.Create(_actor, customerName.Text, phone.Text, address.Text, Convert.ToInt32(variant.SelectedValue), (int)quantity.Value);
            Reload();
            ShowStatus($"Đã tiếp nhận đơn {code} ở trạng thái chờ xác nhận. Tồn kho chưa bị giữ.");
        }
        catch (Exception ex) { ShowStatus("Không thể tạo đơn: " + ex.Message, true); }
    }

    private void saveStatus_Click(object? sender, EventArgs e)
    {
        if (_orderId is null || status.SelectedItem is null) { Warn("Chọn đơn và trạng thái tiếp theo."); return; }
        var targetLabel = status.SelectedItem.ToString()!;
        string target;
        try { target = StatusCode(targetLabel); }
        catch (Exception ex) { ShowStatus(ex.Message, true); return; }
        var effect = target == "Completed" ? " Tồn kho sẽ được trừ và thanh toán chuyển sang Đã thanh toán." : string.Empty;
        if (MessageBox.Show($"Chuyển đơn sang {targetLabel}?{effect}", "Xác nhận xử lý", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            _orders.UpdateStatus(_actor, _orderId.Value, target, note.Text);
            Reload();
            ShowStatus(target == "Completed" ? "Đã hoàn tất đơn, trừ tồn kho và cập nhật thanh toán." : "Đã cập nhật trạng thái đơn.");
        }
        catch (Exception ex) { ShowStatus("Không thể đổi trạng thái: " + ex.Message, true); }
    }

    private void assignButton_Click(object? sender, EventArgs e)
    {
        if (_orderId is null) { Warn("Chọn đơn hàng cần phân công."); return; }
        if (assignee.SelectedValue is null) { Warn("Chọn nhân viên phụ trách."); return; }
        try
        {
            _orders.Assign(_actor, _orderId.Value, Convert.ToInt32(assignee.SelectedValue));
            Reload();
            ShowStatus("Đã phân công đơn và ghi vào lịch sử.");
        }
        catch (Exception ex) { ShowStatus("Không thể phân công: " + ex.Message, true); }
    }

    private void reload_Click(object? sender, EventArgs e) => Reload();

    private void LoadVariants()
    {
        try
        {
            var table = _orders.OrderableVariants();
            variant.DataSource = table;
            variant.DisplayMember = "ITEM";
            variant.ValueMember = "VARIANT_ID";
            if (variant.Items.Count > 0) variant.SelectedIndex = 0;
            if (table.Rows.Count == 0) ShowStatus("Chưa có biến thể đang kinh doanh để đặt online.");
        }
        catch (Exception ex) { ShowStatus("Không tải được sản phẩm từ Oracle: " + ex.Message, true); }
    }

    private void LoadStaff()
    {
        try
        {
            var staff = _orders.AssignableStaff();
            assignee.DataSource = staff;
            assignee.DisplayMember = "DISPLAY_NAME";
            assignee.ValueMember = "USER_ID";
            if (staff.Rows.Count == 0) assignmentInfo.Text = "Chưa có tài khoản nhân viên bán hàng/kho đang hoạt động.";
        }
        catch (Exception ex) { assignmentInfo.Text = "Không tải được danh sách nhân viên: " + ex.Message; }
    }

    private void Reload()
    {
        try
        {
            ordersGrid.DataSource = _orders.List(GetStatusCode(), orderSearch.Text.Trim());
            var headers = new[]
            {
                ("ORDER_ID", "ORDER_ID"), ("ORDER_CODE", "Mã đơn"), ("CUSTOMER_NAME", "Khách hàng"),
                ("CUSTOMER_PHONE", "Điện thoại"), ("SHIPPING_ADDRESS", "Địa chỉ"), ("ORDER_STATUS", "Trạng thái"),
                ("STOCK_RESERVED", "Đã giữ hàng"), ("ASSIGNED_TO", "Nhân viên phụ trách"),
                ("CREATED_AT", "Ngày tạo"), ("TOTAL_AMOUNT", "Tổng tiền")
            };
            foreach (var (key, label) in headers)
                if (ordersGrid.Columns.Contains(key)) ordersGrid.Columns[key].HeaderText = label;
            if (ordersGrid.Columns.Contains("ORDER_ID")) ordersGrid.Columns["ORDER_ID"].Visible = false;
            if (ordersGrid.Columns.Contains("ASSIGNED_TO_USER_ID")) ordersGrid.Columns["ASSIGNED_TO_USER_ID"].Visible = false;
            if (ordersGrid.Columns.Contains("STOCK_RESERVED")) ordersGrid.Columns["STOCK_RESERVED"].Visible = false;
            if (ordersGrid.Columns.Contains("ASSIGNED_TO")) ordersGrid.Columns["ASSIGNED_TO"].Visible = false;
            if (ordersGrid.Columns.Contains("CREATED_AT")) ordersGrid.Columns["CREATED_AT"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (ordersGrid.Columns.Contains("TOTAL_AMOUNT")) ordersGrid.Columns["TOTAL_AMOUNT"].DefaultCellStyle.Format = "N0";
            _orderId = null;
            if (ordersGrid.Rows.Count == 0) ShowStatus("Không tìm thấy đơn phù hợp với bộ lọc.");
            else ShowStatus($"Đang hiển thị {ordersGrid.Rows.Count} đơn hàng.");
        }
        catch (Exception ex) { ShowStatus("Không tải được đơn hàng: " + ex.Message, true); }
    }

    private void LoadHistory(int orderId)
    {
        try
        {
            historyGrid.DataSource = _orders.History(orderId);
            var headers = new[]
            {
                ("CHANGED_AT", "Thời gian"), ("EVENT_TYPE", "Sự kiện"), ("OLD_STATUS", "Trạng thái cũ"),
                ("NEW_STATUS", "Trạng thái mới"), ("NOTE", "Ghi chú"), ("CHANGED_BY", "Người thực hiện")
            };
            foreach (var (key, label) in headers)
                if (historyGrid.Columns.Contains(key)) historyGrid.Columns[key].HeaderText = label;
            if (historyGrid.Columns.Contains("HISTORY_ID")) historyGrid.Columns["HISTORY_ID"].Visible = false;
            if (historyGrid.Columns.Contains("CHANGED_AT")) historyGrid.Columns["CHANGED_AT"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            if (historyGrid.Columns.Contains("OLD_STATUS")) historyGrid.Columns["OLD_STATUS"].DefaultCellStyle.NullValue = "—";
            foreach (DataGridViewRow row in historyGrid.Rows)
            {
                if (row.Cells["OLD_STATUS"].Value is string oldCode) row.Cells["OLD_STATUS"].Value = StatusDisplay(oldCode);
                if (row.Cells["NEW_STATUS"].Value is string newCode) row.Cells["NEW_STATUS"].Value = StatusDisplay(newCode);
                if (row.Cells["EVENT_TYPE"].Value is string eventType)
                    row.Cells["EVENT_TYPE"].Value = eventType switch { "CREATED" => "Tạo đơn", "STATUS" => "Đổi trạng thái", "ASSIGNED" => "Phân công", "BASELINE" => "Ghi nhận ban đầu", _ => eventType };
            }
        }
        catch (Exception ex) { ShowStatus("Không tải được lịch sử đơn: " + ex.Message, true); }
    }

    private void ordersGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || ordersGrid.Columns[e.ColumnIndex].Name != "ORDER_STATUS") return;
        e.Value = StatusDisplay(Convert.ToString(e.Value) ?? string.Empty);
        e.FormattingApplied = true;
    }

    private static string StatusDisplay(string code) => code switch
    {
        "PENDING" or "Pending" => "Chờ xác nhận", "SHIPPING" or "Shipping" => "Đang giao", "COMPLETED" or "Completed" => "Hoàn tất", "CANCELLED" or "Cancelled" => "Đã hủy",
        _ => code
    };

    private static string StatusCode(string label) => label switch
    {
        "Đang giao" => "Shipping", "Hoàn tất" => "Completed", "Đã hủy" => "Cancelled", "Chờ xác nhận" => "Pending",
        _ => throw new InvalidOperationException("Trạng thái đơn không hợp lệ.")
    };

    private string? GetStatusCode() => statusFilter.SelectedItem?.ToString() switch
    {
        "Chờ xác nhận" => "Pending", "Đang giao" => "Shipping", "Hoàn tất" => "Completed", "Đã hủy" => "Cancelled",
        _ => null
    };

    private void ShowStatus(string text, bool isError = false)
    {
        statusLabel.Text = text;
        statusLabel.ForeColor = isError ? Color.FromArgb(177, 56, 56) : Color.FromArgb(91, 107, 126);
    }

    private static void Warn(string text) => MessageBox.Show(text, "Kiểm tra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
internal sealed class OrderService
{
    public DataTable List(string? status=null,string keyword="")
    {
        using var c=OracleConnectionFactory.Create();using var q=c.CreateCommand();q.BindByName=true;
        q.CommandText="SELECT o.OrderID AS ORDER_ID,TO_CHAR(o.OrderID) AS ORDER_CODE,c.FullName AS CUSTOMER_NAME,c.Phone AS CUSTOMER_PHONE,c.Address AS SHIPPING_ADDRESS,o.Status AS ORDER_STATUS,0 AS STOCK_RESERVED,a.EmployeeID AS ASSIGNED_TO_USER_ID,e.FullName AS ASSIGNED_TO,o.OrderDate AS CREATED_AT,o.TotalAmount AS TOTAL_AMOUNT FROM Orders o JOIN Customers c ON c.CustomerID=o.CustomerID LEFT JOIN OrderAssignments a ON a.OrderID=o.OrderID LEFT JOIN Employees e ON e.EmployeeID=a.EmployeeID WHERE (:s IS NULL OR UPPER(o.Status)=UPPER(:s)) AND (:q IS NULL OR TO_CHAR(o.OrderID) LIKE '%'||:q||'%' OR UPPER(c.FullName) LIKE '%'||UPPER(:q)||'%' OR c.Phone LIKE '%'||:q||'%') ORDER BY o.OrderDate DESC";
        q.Parameters.Add("s",OracleDbType.Varchar2).Value=string.IsNullOrWhiteSpace(status)?DBNull.Value:status;
        q.Parameters.Add("q",OracleDbType.NVarchar2,150).Value=string.IsNullOrWhiteSpace(keyword)?DBNull.Value:keyword.Trim();var dt=new DataTable();using var a=new OracleDataAdapter(q);a.Fill(dt);return dt;
    }
    public DataTable OrderableVariants(){using var c=OracleConnectionFactory.Create();using var q=c.CreateCommand();q.CommandText="SELECT v.VariantID AS VARIANT_ID,TO_CHAR(v.VariantID) AS SKU,p.ProductName||' / '||sz.SizeValue||' / '||cl.ColorName AS ITEM,p.Price AS SELL_PRICE,v.StockQuantity AS STOCK_QTY FROM ProductVariants v JOIN Products p ON p.ProductID=v.ProductID JOIN Sizes sz ON sz.SizeID=v.SizeID JOIN Colors cl ON cl.ColorID=v.ColorID WHERE p.Status='Active' ORDER BY p.ProductName,sz.SizeValue,cl.ColorName";var d=new DataTable();using var a=new OracleDataAdapter(q);a.Fill(d);return d;}
    public DataTable AssignableStaff(){using var c=OracleConnectionFactory.Create();using var q=c.CreateCommand();q.CommandText="SELECT EmployeeID AS USER_ID,FullName||' · '||Position AS DISPLAY_NAME FROM Employees ORDER BY FullName";var d=new DataTable();using var a=new OracleDataAdapter(q);a.Fill(d);return d;}
    public DataTable History(int orderId){using var c=OracleConnectionFactory.Create();using var q=c.CreateCommand();q.BindByName=true;q.CommandText="SELECT HistoryID AS HISTORY_ID,ChangedAt AS CHANGED_AT,EventType AS EVENT_TYPE,OldStatus AS OLD_STATUS,NewStatus AS NEW_STATUS,Note AS NOTE,ChangedBy AS CHANGED_BY FROM OrderHistory WHERE OrderID=:id ORDER BY ChangedAt DESC,HistoryID DESC";q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;var d=new DataTable();using var a=new OracleDataAdapter(q);a.Fill(d);return d;}
    public void Assign(UserSession actor,int orderId,int assigneeEmployeeId)
    {
        actor.Require("MANAGER");using var c=OracleConnectionFactory.Create();c.Open();using var tx=c.BeginTransaction();
        try
        {
            string orderStatus;using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT Status FROM Orders WHERE OrderID=:id FOR UPDATE";q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;var result=q.ExecuteScalar();if(result is null or DBNull)throw new InvalidOperationException("Không tìm thấy đơn hàng.");orderStatus=Convert.ToString(result)!;}
            if(orderStatus is "Completed" or "Cancelled")throw new InvalidOperationException("Không thể phân công đơn đã hoàn tất hoặc đã hủy.");
            string employeeName;using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT FullName FROM Employees WHERE EmployeeID=:id";q.Parameters.Add("id",OracleDbType.Int32).Value=assigneeEmployeeId;var result=q.ExecuteScalar();if(result is null or DBNull)throw new InvalidOperationException("Không tìm thấy nhân viên được chọn.");employeeName=Convert.ToString(result)!;}
            int? previous=null;using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT EmployeeID FROM OrderAssignments WHERE OrderID=:id FOR UPDATE";q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;var result=q.ExecuteScalar();if(result is not null and not DBNull)previous=Convert.ToInt32(result);}
            if(previous==assigneeEmployeeId)throw new InvalidOperationException("Đơn đã được giao cho nhân viên này.");
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;if(previous.HasValue){q.CommandText="UPDATE OrderAssignments SET EmployeeID=:employee,AssignedAt=SYSDATE,AssignedBy=:actor WHERE OrderID=:orderId";}else{q.CommandText="INSERT INTO OrderAssignments(OrderID,EmployeeID,AssignedAt,AssignedBy) VALUES(:orderId,:employee,SYSDATE,:actor)";}q.Parameters.Add("employee",OracleDbType.Int32).Value=assigneeEmployeeId;q.Parameters.Add("actor",OracleDbType.Varchar2,128).Value=actor.Username;q.Parameters.Add("orderId",OracleDbType.Int32).Value=orderId;q.ExecuteNonQuery();}
            InsertHistory(c,tx,orderId,"ASSIGNED",null,null,$"Phân công cho {employeeName}.",actor.Username,previous,assigneeEmployeeId);tx.Commit();
        }
        catch{tx.Rollback();throw;}
    }
    public string Create(UserSession actor,string customerName,string phone,string address,int variantId,int quantity)
    {
        actor.Require("EMPLOYEE","MANAGER");if(quantity<=0)throw new ArgumentException("Số lượng phải lớn hơn 0.");if(string.IsNullOrWhiteSpace(phone))throw new ArgumentException("Nhập số điện thoại khách đã có trong bảng Customers.");
        using var c=OracleConnectionFactory.Create();c.Open();using var tx=c.BeginTransaction();
        try{
            int customerId;using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT CustomerID FROM Customers WHERE Phone=:phone";q.Parameters.Add("phone",OracleDbType.Varchar2,15).Value=phone.Trim();var v=q.ExecuteScalar();if(v is null or DBNull)throw new InvalidOperationException("Không tìm thấy khách hàng theo số điện thoại. Tạo khách bằng tài khoản quản lý trước.");customerId=Convert.ToInt32(v);}
            decimal price;int stock;using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT p.Price,v.StockQuantity FROM ProductVariants v JOIN Products p ON p.ProductID=v.ProductID WHERE v.VariantID=:id AND p.Status='Active'";q.Parameters.Add("id",OracleDbType.Int32).Value=variantId;using var r=q.ExecuteReader();if(!r.Read())throw new InvalidOperationException("Không tìm thấy biến thể đang bán.");price=Convert.ToDecimal(r.GetValue(0));stock=Convert.ToInt32(r.GetValue(1));}if(quantity>stock)throw new InvalidOperationException($"Không đủ tồn để tiếp nhận đơn; hiện còn {stock}. Nhân viên không có quyền giữ/trừ kho.");
            // Use the same lock order as POS so MAX(ID)+1 stays unique across both app flows.
            SalesService.Lock(c,tx,"Orders");SalesService.Lock(c,tx,"OrderDetails");SalesService.Lock(c,tx,"Payments");
            var total=price*quantity;int oid,did,pid;using(var q=c.CreateCommand()){q.Transaction=tx;q.CommandText="SELECT NVL(MAX(OrderID),0)+1 FROM Orders";oid=Convert.ToInt32(q.ExecuteScalar());q.CommandText="SELECT NVL(MAX(OrderDetailID),0)+1 FROM OrderDetails";did=Convert.ToInt32(q.ExecuteScalar());q.CommandText="SELECT NVL(MAX(PaymentID),0)+1 FROM Payments";pid=Convert.ToInt32(q.ExecuteScalar());}
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO Orders(OrderID,CustomerID,EmployeeID,OrderDate,Status,TotalAmount) VALUES(:id,:cust,NULL,SYSDATE,'Pending',:total)";q.Parameters.Add("id",OracleDbType.Int32).Value=oid;q.Parameters.Add("cust",OracleDbType.Int32).Value=customerId;q.Parameters.Add("total",OracleDbType.Decimal).Value=total;q.ExecuteNonQuery();}
            InsertHistory(c,tx,oid,"CREATED",null,"Pending","Tạo đơn trực tuyến.",actor.Username);
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO OrderDetails(OrderDetailID,OrderID,VariantID,Quantity,UnitPrice) VALUES(:id,:ord,:var,:qty,:price)";q.Parameters.Add("id",OracleDbType.Int32).Value=did;q.Parameters.Add("ord",OracleDbType.Int32).Value=oid;q.Parameters.Add("var",OracleDbType.Int32).Value=variantId;q.Parameters.Add("qty",OracleDbType.Int32).Value=quantity;q.Parameters.Add("price",OracleDbType.Decimal).Value=price;q.ExecuteNonQuery();}
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO Payments(PaymentID,OrderID,PaymentMethod,Amount,PaymentDate,Status) VALUES(:id,:ord,'Cash',:amount,NULL,'Unpaid')";q.Parameters.Add("id",OracleDbType.Int32).Value=pid;q.Parameters.Add("ord",OracleDbType.Int32).Value=oid;q.Parameters.Add("amount",OracleDbType.Decimal).Value=total;q.ExecuteNonQuery();}
            tx.Commit();return oid.ToString();
        }catch{tx.Rollback();throw;}
    }
    public void UpdateStatus(UserSession actor,int orderId,string newStatus,string note)
    {
        actor.Require("MANAGER");
        if(!new[]{"Pending","Shipping","Completed","Cancelled"}.Contains(newStatus))throw new ArgumentException("Trạng thái không hợp lệ.");
        using var c=OracleConnectionFactory.Create();c.Open();using var tx=c.BeginTransaction();
        try
        {
            string oldStatus;decimal total;
            using(var q=c.CreateCommand())
            {
                q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT Status,TotalAmount FROM Orders WHERE OrderID=:id FOR UPDATE";q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;
                using var r=q.ExecuteReader();if(!r.Read())throw new InvalidOperationException("Không tìm thấy đơn.");oldStatus=r.GetString(0);total=Convert.ToDecimal(r.GetValue(1));
            }
            if(oldStatus==newStatus)throw new InvalidOperationException("Đơn đã ở trạng thái này.");
            var allowed=oldStatus switch{"Pending"=>new[]{"Shipping","Completed","Cancelled"},"Shipping"=>new[]{"Completed","Cancelled"},_=>Array.Empty<string>()};
            if(!allowed.Contains(newStatus))throw new InvalidOperationException($"Không thể chuyển đơn từ {oldStatus} sang {newStatus}; đơn Hoàn tất/Đã hủy là trạng thái kết thúc.");
            if(newStatus=="Completed")
            {
                var lines=new List<(int Variant,int Quantity)>();
                using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT VariantID,Quantity FROM OrderDetails WHERE OrderID=:id ORDER BY VariantID";q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;using var r=q.ExecuteReader();while(r.Read())lines.Add((Convert.ToInt32(r.GetValue(0)),Convert.ToInt32(r.GetValue(1))));}
                if(lines.Count==0)throw new InvalidOperationException("Không thể hoàn tất đơn không có chi tiết hàng hóa.");
                foreach(var line in lines)
                {
                    using var q=c.CreateCommand();q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT StockQuantity FROM ProductVariants WHERE VariantID=:id FOR UPDATE";q.Parameters.Add("id",OracleDbType.Int32).Value=line.Variant;
                    var stock=Convert.ToInt32(q.ExecuteScalar());if(stock<line.Quantity)throw new InvalidOperationException($"Biến thể {line.Variant} không đủ tồn: còn {stock}, cần {line.Quantity}.");
                    q.CommandText="UPDATE ProductVariants SET StockQuantity=StockQuantity-:qty WHERE VariantID=:id";q.Parameters.Clear();q.Parameters.Add("qty",OracleDbType.Int32).Value=line.Quantity;q.Parameters.Add("id",OracleDbType.Int32).Value=line.Variant;q.ExecuteNonQuery();
                }
                using(var q=c.CreateCommand())
                {
                    q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT PaymentID FROM Payments WHERE OrderID=:id FOR UPDATE";q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;var payment=q.ExecuteScalar();
                    if(payment is null or DBNull)
                    {
                        SalesService.Lock(c,tx,"Payments");q.CommandText="SELECT NVL(MAX(PaymentID),0)+1 FROM Payments";q.Parameters.Clear();var paymentId=Convert.ToInt32(q.ExecuteScalar());
                        q.CommandText="INSERT INTO Payments(PaymentID,OrderID,PaymentMethod,Amount,PaymentDate,Status) VALUES(:pid,:oid,'Cash',:amount,SYSDATE,'Paid')";q.Parameters.Clear();q.Parameters.Add("pid",OracleDbType.Int32).Value=paymentId;q.Parameters.Add("oid",OracleDbType.Int32).Value=orderId;q.Parameters.Add("amount",OracleDbType.Decimal).Value=total;q.ExecuteNonQuery();
                    }
                    else
                    {
                        q.CommandText="UPDATE Payments SET Status='Paid',PaymentDate=NVL(PaymentDate,SYSDATE) WHERE OrderID=:id";q.Parameters.Clear();q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;q.ExecuteNonQuery();
                    }
                }
            }
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="UPDATE Orders SET Status=:status WHERE OrderID=:id";q.Parameters.Add("status",OracleDbType.Varchar2,15).Value=newStatus;q.Parameters.Add("id",OracleDbType.Int32).Value=orderId;q.ExecuteNonQuery();}
            InsertHistory(c,tx,orderId,"STATUS",oldStatus,newStatus,string.IsNullOrWhiteSpace(note)?null:note.Trim(),actor.Username);
            tx.Commit();
        }
        catch{tx.Rollback();throw;}
    }
    private static void InsertHistory(OracleConnection c,OracleTransaction tx,int orderId,string eventType,string? oldStatus,string? newStatus,string? note,string actor,int? oldEmployee=null,int? newEmployee=null)
    {
        SalesService.Lock(c,tx,"OrderHistory");int historyId;using(var next=c.CreateCommand()){next.Transaction=tx;next.CommandText="SELECT NVL(MAX(HistoryID),0)+1 FROM OrderHistory";historyId=Convert.ToInt32(next.ExecuteScalar());}
        using var q=c.CreateCommand();q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO OrderHistory(HistoryID,OrderID,ChangedAt,EventType,OldStatus,NewStatus,OldEmployeeID,NewEmployeeID,Note,ChangedBy) VALUES(:historyId,:orderId,SYSDATE,:eventType,:oldStatus,:newStatus,:oldEmployee,:newEmployee,:note,:actor)";
        q.Parameters.Add("historyId",OracleDbType.Int32).Value=historyId;q.Parameters.Add("orderId",OracleDbType.Int32).Value=orderId;q.Parameters.Add("eventType",OracleDbType.Varchar2,20).Value=eventType;q.Parameters.Add("oldStatus",OracleDbType.Varchar2,15).Value=(object?)oldStatus??DBNull.Value;q.Parameters.Add("newStatus",OracleDbType.Varchar2,15).Value=(object?)newStatus??DBNull.Value;q.Parameters.Add("oldEmployee",OracleDbType.Int32).Value=(object?)oldEmployee??DBNull.Value;q.Parameters.Add("newEmployee",OracleDbType.Int32).Value=(object?)newEmployee??DBNull.Value;q.Parameters.Add("note",OracleDbType.NVarchar2,500).Value=(object?)note??DBNull.Value;q.Parameters.Add("actor",OracleDbType.Varchar2,128).Value=actor;q.ExecuteNonQuery();
    }
}
