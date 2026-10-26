using Oracle.ManagedDataAccess.Client;

namespace ShoeStore.WinForms;

internal static class OracleConnectionFactory
{
    // Update appsettings.json with the group's Oracle service and demo account.
    public static OracleConnection Create()
    {
        var connectionString = Environment.GetEnvironmentVariable("SHOESTORE_ORACLE");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Chưa cấu hình biến môi trường SHOESTORE_ORACLE cho kết nối Oracle.");
        return new OracleConnection(connectionString);
    }
}
