using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ShoeStore.WinForms;

internal sealed class ProductRepository
{
    public DataTable Search(string keyword = "")
    {
        using var connection = OracleConnectionFactory.Create();
        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = """
            SELECT v.VARIANT_ID, v.STYLE_ID, v.SKU, s.STYLE_NAME, s.BRAND, s.CATEGORY,
                   v.SIZE_CODE, v.COLOR, v.SELL_PRICE, v.STOCK_QTY, v.REORDER_LEVEL
            FROM SHOE_VARIANT v JOIN SHOE_STYLE s ON s.STYLE_ID=v.STYLE_ID
            WHERE v.IS_ACTIVE=1 AND s.IS_ACTIVE=1
              AND (:keyword IS NULL OR UPPER(s.STYLE_NAME) LIKE '%' || UPPER(:keyword) || '%'
                   OR UPPER(s.BRAND) LIKE '%' || UPPER(:keyword) || '%'
                   OR UPPER(v.SKU) LIKE '%' || UPPER(:keyword) || '%')
            ORDER BY s.STYLE_NAME, v.SIZE_CODE, v.COLOR
            """;
        command.Parameters.Add("keyword", OracleDbType.NVarchar2, 150).Value =
            string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim();
        var result = new DataTable();
        using var adapter = new OracleDataAdapter(command);
        adapter.Fill(result);
        return result;
    }

    public void Add(string styleName, string brand, string category, string sku, string size, string color, decimal price, int reorderLevel)
    {
        using var connection = OracleConnectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            var styleId = NextId(connection, transaction, "SEQ_SHOE_STYLE");
            using (var styleCommand = connection.CreateCommand())
            {
                styleCommand.Transaction = transaction; styleCommand.BindByName = true;
                styleCommand.CommandText = "INSERT INTO SHOE_STYLE (STYLE_ID, STYLE_NAME, BRAND, CATEGORY) VALUES (:id,:name,:brand,:category)";
                styleCommand.Parameters.Add("id", OracleDbType.Int32).Value = styleId;
                styleCommand.Parameters.Add("name", OracleDbType.NVarchar2, 150).Value = styleName.Trim();
                styleCommand.Parameters.Add("brand", OracleDbType.NVarchar2, 80).Value = brand.Trim();
                styleCommand.Parameters.Add("category", OracleDbType.NVarchar2, 60).Value = NullIfWhiteSpace(category);
                styleCommand.ExecuteNonQuery();
            }
            var variantId = NextId(connection, transaction, "SEQ_SHOE_VARIANT");
            using (var variantCommand = connection.CreateCommand())
            {
                variantCommand.Transaction = transaction; variantCommand.BindByName = true;
                variantCommand.CommandText = "INSERT INTO SHOE_VARIANT (VARIANT_ID,STYLE_ID,SKU,SIZE_CODE,COLOR,SELL_PRICE,STOCK_QTY,REORDER_LEVEL) VALUES (:id,:styleId,:sku,:size,:color,:price,0,:reorder)";
                AddVariant(variantCommand, variantId, styleId, sku, size, color, price, reorderLevel);
                variantCommand.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch { transaction.Rollback(); throw; }
    }

    public void AddVariant(int styleId, string sku, string size, string color, decimal price, int reorderLevel)
    {
        using var connection = OracleConnectionFactory.Create();
        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = "INSERT INTO SHOE_VARIANT (VARIANT_ID,STYLE_ID,SKU,SIZE_CODE,COLOR,SELL_PRICE,STOCK_QTY,REORDER_LEVEL) VALUES (SEQ_SHOE_VARIANT.NEXTVAL,:styleId,:sku,:size,:color,:price,0,:reorder)";
        command.Parameters.Add("styleId", OracleDbType.Int32).Value = styleId;
        command.Parameters.Add("sku", OracleDbType.Varchar2, 40).Value = sku.Trim();
        command.Parameters.Add("size", OracleDbType.Varchar2, 10).Value = size.Trim();
        command.Parameters.Add("color", OracleDbType.NVarchar2, 40).Value = color.Trim();
        command.Parameters.Add("price", OracleDbType.Decimal).Value = price;
        command.Parameters.Add("reorder", OracleDbType.Int32).Value = reorderLevel;
        connection.Open(); command.ExecuteNonQuery();
    }

    public void Update(int variantId, int styleId, string styleName, string brand, string category, string sku, string size, string color, decimal price, int reorderLevel)
    {
        using var connection = OracleConnectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using (var styleCommand = connection.CreateCommand())
            {
                styleCommand.Transaction = transaction; styleCommand.BindByName = true;
                styleCommand.CommandText = "UPDATE SHOE_STYLE SET STYLE_NAME=:name,BRAND=:brand,CATEGORY=:category WHERE STYLE_ID=:id AND IS_ACTIVE=1";
                styleCommand.Parameters.Add("name", OracleDbType.NVarchar2, 150).Value = styleName.Trim();
                styleCommand.Parameters.Add("brand", OracleDbType.NVarchar2, 80).Value = brand.Trim();
                styleCommand.Parameters.Add("category", OracleDbType.NVarchar2, 60).Value = NullIfWhiteSpace(category);
                styleCommand.Parameters.Add("id", OracleDbType.Int32).Value = styleId;
                if (styleCommand.ExecuteNonQuery() != 1) throw new InvalidOperationException("Không tìm thấy mẫu giày để sửa.");
            }
            using (var variantCommand = connection.CreateCommand())
            {
                variantCommand.Transaction = transaction; variantCommand.BindByName = true;
                variantCommand.CommandText = "UPDATE SHOE_VARIANT SET SKU=:sku,SIZE_CODE=:size,COLOR=:color,SELL_PRICE=:price,REORDER_LEVEL=:reorder WHERE VARIANT_ID=:id AND STYLE_ID=:styleId AND IS_ACTIVE=1";
                AddVariant(variantCommand, variantId, styleId, sku, size, color, price, reorderLevel);
                if (variantCommand.ExecuteNonQuery() != 1) throw new InvalidOperationException("Không tìm thấy biến thể giày để sửa.");
            }
            transaction.Commit();
        }
        catch { transaction.Rollback(); throw; }
    }

    public void Deactivate(int variantId)
    {
        using var connection = OracleConnectionFactory.Create();
        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = "UPDATE SHOE_VARIANT SET IS_ACTIVE=0 WHERE VARIANT_ID=:id AND IS_ACTIVE=1";
        command.Parameters.Add("id", OracleDbType.Int32).Value = variantId;
        connection.Open();
        if (command.ExecuteNonQuery() != 1) throw new InvalidOperationException("Biến thể không tồn tại hoặc đã ngừng kinh doanh.");
    }

    private static int NextId(OracleConnection connection, OracleTransaction transaction, string sequence)
    {
        if (sequence is not ("SEQ_SHOE_STYLE" or "SEQ_SHOE_VARIANT")) throw new ArgumentOutOfRangeException(nameof(sequence));
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"SELECT {sequence}.NEXTVAL FROM DUAL";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static object NullIfWhiteSpace(string value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();

    private static void AddVariant(OracleCommand command, int variantId, int styleId, string sku, string size, string color, decimal price, int reorderLevel)
    {
        command.Parameters.Add("id", OracleDbType.Int32).Value = variantId;
        command.Parameters.Add("styleId", OracleDbType.Int32).Value = styleId;
        command.Parameters.Add("sku", OracleDbType.Varchar2, 40).Value = sku.Trim();
        command.Parameters.Add("size", OracleDbType.Varchar2, 10).Value = size.Trim();
        command.Parameters.Add("color", OracleDbType.NVarchar2, 40).Value = color.Trim();
        command.Parameters.Add("price", OracleDbType.Decimal).Value = price;
        command.Parameters.Add("reorder", OracleDbType.Int32).Value = reorderLevel;
    }
}
