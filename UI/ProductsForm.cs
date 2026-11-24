using System.Data;
using System.Runtime.Versioning;

using Oracle.ManagedDataAccess.Client;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
public partial class ProductsForm : Form
{
    private readonly ProductRepository _products = new();
    private readonly UserSession _session = null!;
    private int? _selectedVariantId;
    private int? _selectedStyleId;

    public ProductsForm() { InitializeComponent(); UiTheme.Apply(this); }
    internal ProductsForm(UserSession session)
    {
        _session = session;
        _session.Require("MANAGER", "EMPLOYEE");
        InitializeComponent(); UiTheme.Apply(this);
        var canEdit = session.IsManager;
        foreach (var control in new Control[] { styleName, brand, category, sku, sizeCode, color, price, reorder, addButton, addVariantButton, updateButton, deactivateButton })
            control.Visible = canEdit;
        sku.ReadOnly = false; sku.PlaceholderText = "Mã trong Suppliers"; labelSku.Text = "Mã nhà cung cấp *";
        reorder.Visible = false; labelReorder.Visible = false;
        if (!canEdit)
            foreach (var control in new Control[] { labelStyle, labelBrand, labelCategory, labelSku, labelSize, labelColor, labelPrice, labelReorder }) control.Visible = false;
        panel.Text = canEdit ? "Thông tin sản phẩm" : "Danh mục chỉ xem (quyền nhân viên)";
        Load += (_, _) => Reload();
    }

    private void searchButton_Click(object? sender, EventArgs e) => Reload(searchBox.Text);
    private void reloadButton_Click(object? sender, EventArgs e) { searchBox.Clear(); Reload(); }

    private void addButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        if (!int.TryParse(sku.Text, out var supplierId) || supplierId <= 0) { Warn("Nhập mã nhà cung cấp có trong bảng Suppliers."); return; }
        Run(() => _products.Add(styleName.Text, brand.Text, category.Text, supplierId, sizeCode.Text, color.Text, price.Value, (int)reorder.Value), "Đã thêm mẫu giày và biến thể. Tồn kho ban đầu bằng 0.");
    }

    private void addVariantButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateVariantInput()) return;
        if (_selectedStyleId is null) { Warn("Chọn một biến thể thuộc mẫu giày cần thêm cỡ/màu."); return; }
        var selectedStyleId = _selectedStyleId.Value;
        Run(() => _products.AddVariant(selectedStyleId, sku.Text, sizeCode.Text, color.Text, price.Value, (int)reorder.Value), "Đã thêm biến thể. Tồn kho ban đầu bằng 0.");
    }

    private void updateButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;
        if (_selectedVariantId is null || _selectedStyleId is null) { Warn("Chọn một biến thể trong danh sách trước khi cập nhật."); return; }
        if (!int.TryParse(sku.Text, out var supplierId) || supplierId <= 0) { Warn("Nhập mã nhà cung cấp có trong bảng Suppliers."); return; }
        Run(() => _products.Update(_selectedVariantId.Value, _selectedStyleId.Value, styleName.Text, brand.Text, category.Text, supplierId, sizeCode.Text, color.Text, price.Value, (int)reorder.Value), "Đã cập nhật sản phẩm.");
    }

    private void deactivateButton_Click(object? sender, EventArgs e)
    {
        if (_selectedVariantId is null) { Warn("Chọn biến thể cần ngừng bán."); return; }
        if (MessageBox.Show("Ngừng bán toàn bộ cỡ và màu thuộc mẫu sản phẩm đang chọn? Dữ liệu đơn cũ được giữ lại.", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        Run(() => _products.Deactivate(_selectedVariantId.Value), "Đã ngừng bán sản phẩm.");
    }

    private void grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (grid.CurrentRow?.DataBoundItem is not DataRowView view) return;
        var row = view.Row;
        _selectedVariantId = Convert.ToInt32(row["VARIANT_ID"]);
        _selectedStyleId = Convert.ToInt32(row["STYLE_ID"]);
        styleName.Text = Convert.ToString(row["STYLE_NAME"]); brand.Text = Convert.ToString(row["BRAND"]); category.Text = Convert.ToString(row["CATEGORY"]);
        sku.Text = Convert.ToString(row["SUPPLIER_ID"]); sizeCode.Text = Convert.ToString(row["SIZE_CODE"]); color.Text = Convert.ToString(row["COLOR"]);
        price.Value = Math.Clamp(Convert.ToDecimal(row["SELL_PRICE"]), price.Minimum, price.Maximum);
        reorder.Value = Math.Clamp(Convert.ToDecimal(row["REORDER_LEVEL"]), reorder.Minimum, reorder.Maximum);
        statusLabel.Text = $"Tồn hiện tại: {row["STOCK_QTY"]}. Mã biến thể là mã sinh từ VariantID; tồn kho được quản lý ở màn hình kho.";
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(styleName.Text) || string.IsNullOrWhiteSpace(brand.Text) || string.IsNullOrWhiteSpace(category.Text) || string.IsNullOrWhiteSpace(sku.Text) || string.IsNullOrWhiteSpace(sizeCode.Text) || string.IsNullOrWhiteSpace(color.Text))
        { Warn("Nhập tên mẫu, thương hiệu, loại, mã nhà cung cấp, cỡ số và màu sắc."); return false; }
        if (price.Value <= 0) { Warn("Giá bán phải lớn hơn 0."); return false; }
        return true;
    }

    private bool ValidateVariantInput()
    {
        if (string.IsNullOrWhiteSpace(sizeCode.Text) || string.IsNullOrWhiteSpace(color.Text))
        { Warn("Nhập kích cỡ dạng số và màu sắc cho biến thể."); return false; }
        if (price.Value <= 0) { Warn("Giá bán phải lớn hơn 0."); return false; }
        return true;
    }

    private void Run(Action action, string success)
    {
        try { action(); Reload(searchBox.Text); ClearFields(); statusLabel.Text = success; }
        catch (OracleException ex) when (ex.Number == 1)
        {
            var detail = ex.Message.Contains("UQ_PRODUCTS_NAME", StringComparison.OrdinalIgnoreCase)
                ? "Tên mẫu giày này đã tồn tại. Hãy chọn tên khác."
                : ex.Message.Contains("UQ_VARIANTS_COMBO", StringComparison.OrdinalIgnoreCase)
                    ? "Mẫu giày đã có biến thể với cùng kích cỡ và màu sắc."
                    : "Dữ liệu bị trùng với một bản ghi đã có. Hãy kiểm tra lại tên mẫu và tổ hợp cỡ/màu.";
            MessageBox.Show(detail, "Dữ liệu đã tồn tại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể lưu dữ liệu Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void Reload(string keyword = "")
    {
        try
        {
            grid.DataSource = _products.Search(keyword);
            foreach (var name in new[] { "VARIANT_ID", "STYLE_ID", "SUPPLIER_ID" }) if (grid.Columns.Contains(name)) grid.Columns[name].Visible = false;
            var captions = new Dictionary<string, string> { ["SKU"] = "Mã biến thể", ["STYLE_NAME"] = "Mẫu giày", ["BRAND"] = "Thương hiệu", ["CATEGORY"] = "Loại", ["SIZE_CODE"] = "Cỡ", ["COLOR"] = "Màu", ["SELL_PRICE"] = "Giá bán", ["STOCK_QTY"] = "Tồn", ["REORDER_LEVEL"] = "Ngưỡng" };
            foreach (var item in captions) if (grid.Columns.Contains(item.Key)) grid.Columns[item.Key].HeaderText = item.Value;
            statusLabel.Text = $"Có {grid.Rows.Count} biến thể đang kinh doanh.";
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Không tải được sản phẩm từ Oracle", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void ClearFields()
    { _selectedStyleId = null; _selectedVariantId = null; styleName.Clear(); brand.Clear(); category.Clear(); sku.Clear(); sizeCode.Clear(); color.Clear(); price.Value = 0; reorder.Value = 3; grid.ClearSelection(); }

    private void Warn(string message) => MessageBox.Show(message, "Kiểm tra thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void panel_Enter(object sender, EventArgs e)
    {

    }
}

internal sealed class ProductRepository
{
    public DataTable Search(string keyword="")
    {
        using var c=OracleConnectionFactory.Create();using var q=c.CreateCommand();q.BindByName=true;
        q.CommandText="SELECT v.VariantID AS VARIANT_ID,p.ProductID AS STYLE_ID,p.SupplierID AS SUPPLIER_ID,TO_CHAR(v.VariantID) AS SKU,p.ProductName AS STYLE_NAME,b.BrandName AS BRAND,ca.CategoryName AS CATEGORY,TO_CHAR(sz.SizeValue) AS SIZE_CODE,cl.ColorName AS COLOR,p.Price AS SELL_PRICE,v.StockQuantity AS STOCK_QTY,0 AS REORDER_LEVEL FROM ProductVariants v JOIN Products p ON p.ProductID=v.ProductID JOIN Brands b ON b.BrandID=p.BrandID JOIN Categories ca ON ca.CategoryID=p.CategoryID JOIN Colors cl ON cl.ColorID=v.ColorID JOIN Sizes sz ON sz.SizeID=v.SizeID WHERE p.Status='Active' AND (:q IS NULL OR UPPER(p.ProductName) LIKE '%'||UPPER(:q)||'%' OR UPPER(b.BrandName) LIKE '%'||UPPER(:q)||'%' OR TO_CHAR(v.VariantID) LIKE '%'||:q||'%') ORDER BY p.ProductName,sz.SizeValue,cl.ColorName";
        q.Parameters.Add("q",OracleDbType.NVarchar2,150).Value=string.IsNullOrWhiteSpace(keyword)?DBNull.Value:keyword.Trim();var d=new DataTable();using var a=new OracleDataAdapter(q);a.Fill(d);return d;
    }
    public void Add(string name,string brand,string category,int supplier,string size,string color,decimal price,int reorder)
    {
        using var c=OracleConnectionFactory.Create();c.Open();using var tx=c.BeginTransaction();try{
            EnsureProductNameAvailable(c,tx,name,null);
            var b=FindOrCreate(c,tx,"Brands","BrandID","BrandName",brand,"Country","Unknown");var ca=FindOrCreate(c,tx,"Categories","CategoryID","CategoryName",category,"Description",null);var cl=FindOrCreate(c,tx,"Colors","ColorID","ColorName",color,null,null);var sz=FindOrCreateSize(c,tx,size);
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT COUNT(*) FROM Suppliers WHERE SupplierID=:id";q.Parameters.Add("id",OracleDbType.Int32).Value=supplier;if(Convert.ToInt32(q.ExecuteScalar())==0)throw new InvalidOperationException("Mã nhà cung cấp không tồn tại trong Suppliers.");}
            var pid=Next(c,tx,"Products","ProductID");var vid=Next(c,tx,"ProductVariants","VariantID");
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO Products(ProductID,ProductName,BrandID,CategoryID,SupplierID,Gender,Price,Status) VALUES(:id,:name,:brand,:cat,:supplier,'Unisex',:price,'Active')";P(q,"id",pid);P(q,"name",name.Trim());P(q,"brand",b);P(q,"cat",ca);P(q,"supplier",supplier);P(q,"price",price);q.ExecuteNonQuery();}
            using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO ProductVariants(VariantID,ProductID,ColorID,SizeID,StockQuantity) VALUES(:id,:product,:color,:p_size,0)";P(q,"id",vid);P(q,"product",pid);P(q,"color",cl);P(q,"p_size",sz);q.ExecuteNonQuery();}tx.Commit();
        }catch{tx.Rollback();throw;}
    }
    public void AddVariant(int productId,string sku,string size,string color,decimal price,int reorder)
    {using var c=OracleConnectionFactory.Create();c.Open();using var tx=c.BeginTransaction();try{var cl=FindOrCreate(c,tx,"Colors","ColorID","ColorName",color,null,null);var sz=FindOrCreateSize(c,tx,size);EnsureVariantAvailable(c,tx,productId,cl,sz,null);using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="UPDATE Products SET Price=:price WHERE ProductID=:id";P(q,"price",price);P(q,"id",productId);if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Không tìm thấy sản phẩm.");}var id=Next(c,tx,"ProductVariants","VariantID");using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO ProductVariants(VariantID,ProductID,ColorID,SizeID,StockQuantity) VALUES(:id,:product,:color,:p_size,0)";P(q,"id",id);P(q,"product",productId);P(q,"color",cl);P(q,"p_size",sz);q.ExecuteNonQuery();}tx.Commit();}catch{tx.Rollback();throw;}}
    public void Update(int variantId,int productId,string name,string brand,string category,int supplier,string size,string color,decimal price,int reorder)
    {using var c=OracleConnectionFactory.Create();c.Open();using var tx=c.BeginTransaction();try{EnsureProductNameAvailable(c,tx,name,productId);var b=FindOrCreate(c,tx,"Brands","BrandID","BrandName",brand,"Country","Unknown");var ca=FindOrCreate(c,tx,"Categories","CategoryID","CategoryName",category,"Description",null);var cl=FindOrCreate(c,tx,"Colors","ColorID","ColorName",color,null,null);var sz=FindOrCreateSize(c,tx,size);EnsureVariantAvailable(c,tx,productId,cl,sz,variantId);using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="UPDATE Products SET ProductName=:name,BrandID=:brand,CategoryID=:cat,SupplierID=:supplier,Price=:price WHERE ProductID=:id";P(q,"name",name.Trim());P(q,"brand",b);P(q,"cat",ca);P(q,"supplier",supplier);P(q,"price",price);P(q,"id",productId);if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Không tìm thấy sản phẩm.");}using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="UPDATE ProductVariants SET ColorID=:color,SizeID=:p_size WHERE VariantID=:id AND ProductID=:product";P(q,"color",cl);P(q,"p_size",sz);P(q,"id",variantId);P(q,"product",productId);if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Không tìm thấy biến thể.");}tx.Commit();}catch{tx.Rollback();throw;}}
    public void Deactivate(int variantId){using var c=OracleConnectionFactory.Create();using var q=c.CreateCommand();q.BindByName=true;q.CommandText="UPDATE Products SET Status='Inactive' WHERE ProductID=(SELECT ProductID FROM ProductVariants WHERE VariantID=:id)";P(q,"id",variantId);c.Open();if(q.ExecuteNonQuery()!=1)throw new InvalidOperationException("Không tìm thấy biến thể.");}
    private static int FindOrCreate(OracleConnection c,OracleTransaction tx,string table,string idCol,string nameCol,string text,string? extraCol,string? extraValue){if(string.IsNullOrWhiteSpace(text))throw new ArgumentException($"Nhập {nameCol}.");using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText=$"SELECT {idCol} FROM {table} WHERE UPPER({nameCol})=UPPER(:name)";q.Parameters.Add("name",OracleDbType.NVarchar2,150).Value=text.Trim();var x=q.ExecuteScalar();if(x is not null and not DBNull)return Convert.ToInt32(x);}var id=Next(c,tx,table,idCol);using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText=extraCol is null?$"INSERT INTO {table}({idCol},{nameCol}) VALUES(:id,:name)":$"INSERT INTO {table}({idCol},{nameCol},{extraCol}) VALUES(:id,:name,:extra)";P(q,"id",id);P(q,"name",text.Trim());if(extraCol is not null)q.Parameters.Add("extra",OracleDbType.NVarchar2,200).Value=(object?)extraValue??DBNull.Value;q.ExecuteNonQuery();}return id;}
    private static void EnsureProductNameAvailable(OracleConnection c,OracleTransaction tx,string name,int? excludeId){using var q=c.CreateCommand();q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT COUNT(*) FROM Products WHERE UPPER(TRIM(ProductName))=UPPER(TRIM(:name)) AND (:excludeId IS NULL OR ProductID<>:excludeId)";q.Parameters.Add("name",OracleDbType.Varchar2,100).Value=name.Trim();q.Parameters.Add("excludeId",OracleDbType.Int32).Value=(object?)excludeId??DBNull.Value;if(Convert.ToInt32(q.ExecuteScalar())>0)throw new InvalidOperationException("Tên mẫu giày này đã tồn tại. Hãy chọn tên khác.");}
    private static void EnsureVariantAvailable(OracleConnection c,OracleTransaction tx,int productId,int colorId,int sizeId,int? excludeVariantId){using var q=c.CreateCommand();q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT COUNT(*) FROM ProductVariants WHERE ProductID=:productId AND ColorID=:colorId AND SizeID=:sizeId AND (:excludeId IS NULL OR VariantID<>:excludeId)";q.Parameters.Add("productId",OracleDbType.Int32).Value=productId;q.Parameters.Add("colorId",OracleDbType.Int32).Value=colorId;q.Parameters.Add("sizeId",OracleDbType.Int32).Value=sizeId;q.Parameters.Add("excludeId",OracleDbType.Int32).Value=(object?)excludeVariantId??DBNull.Value;if(Convert.ToInt32(q.ExecuteScalar())>0)throw new InvalidOperationException("Mẫu giày đã có biến thể với cùng kích cỡ và màu sắc.");}
    private static int FindOrCreateSize(OracleConnection c,OracleTransaction tx,string text){if(!decimal.TryParse(text,out var value)||value<30||value>50)throw new ArgumentException("Kích cỡ phải là số từ 30 đến 50.");using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="SELECT SizeID FROM Sizes WHERE SizeValue=:p_size";q.Parameters.Add("p_size",OracleDbType.Decimal).Value=value;var x=q.ExecuteScalar();if(x is not null and not DBNull)return Convert.ToInt32(x);}var id=Next(c,tx,"Sizes","SizeID");using(var q=c.CreateCommand()){q.Transaction=tx;q.BindByName=true;q.CommandText="INSERT INTO Sizes(SizeID,SizeValue) VALUES(:id,:p_size)";P(q,"id",id);P(q,"p_size",value);q.ExecuteNonQuery();}return id;}
    private static int Next(OracleConnection c,OracleTransaction tx,string t,string col){SalesService.Lock(c,tx,t);using var q=c.CreateCommand();q.Transaction=tx;q.CommandText=$"SELECT NVL(MAX({col}),0)+1 FROM {t}";return Convert.ToInt32(q.ExecuteScalar());}
    private static void P(OracleCommand q,string name,object value)=>q.Parameters.Add(name, value switch { decimal => OracleDbType.Decimal, string => OracleDbType.NVarchar2, _ => OracleDbType.Int32 }).Value=value;
}
