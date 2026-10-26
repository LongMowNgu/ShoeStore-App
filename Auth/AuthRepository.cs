using Oracle.ManagedDataAccess.Client;

namespace ShoeStore.WinForms.Auth;

internal sealed class AuthRepository
{
    public bool HasAnyAccount()
    {
        using var connection = OracleConnectionFactory.Create();
        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = "SELECT COUNT(*) FROM APP_USER";
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public UserSession? SignIn(string username, string password)
    {
        username = NormalizeUsername(username);
        using var connection = OracleConnectionFactory.Create();
        using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = """
            SELECT USER_ID, USERNAME, ROLE_CODE, PASSWORD_HASH, PASSWORD_SALT, EMPLOYEE_ID
            FROM APP_USER
            WHERE UPPER(USERNAME) = UPPER(:username) AND IS_ACTIVE = 1
            """;
        command.Parameters.Add("username", OracleDbType.Varchar2, 40).Value = username.Trim();
        connection.Open();
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        var hash = reader.GetString(3);
        var salt = reader.GetString(4);
        if (!PasswordHasher.Verify(password, hash, salt)) return null;
        int? employeeId = reader.IsDBNull(5) ? null : Convert.ToInt32(reader.GetValue(5));
        return new UserSession(Convert.ToInt32(reader.GetValue(0)), reader.GetString(1), reader.GetString(2), employeeId);
    }

    public void CreateFirstManager(string username, string password)
    {
        ValidateCredentials(username, password);
        username = NormalizeUsername(username);
        var (hash, salt) = PasswordHasher.Hash(password);
        using var connection = OracleConnectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using (var lockCommand = connection.CreateCommand())
            {
                lockCommand.Transaction = transaction;
                lockCommand.CommandText = "LOCK TABLE APP_USER IN EXCLUSIVE MODE";
                lockCommand.ExecuteNonQuery();
            }

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.BindByName = true;
            command.CommandText = """
                INSERT INTO APP_USER (USER_ID, USERNAME, PASSWORD_HASH, PASSWORD_SALT, ROLE_CODE, IS_ACTIVE)
                SELECT SEQ_APP_USER.NEXTVAL, :username, :hash, :salt, 'MANAGER', 1
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM APP_USER)
                """;
            AddCredentials(command, username, hash, salt);
            if (command.ExecuteNonQuery() != 1)
                throw new InvalidOperationException("Tài khoản đã được khởi tạo. Hãy đăng nhập bằng tài khoản quản lý.");
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void RegisterEmployeeAccount(UserSession actor, string username, string password, string roleCode, string employeeName, string employeePhone)
    {
        actor.Require("MANAGER");
        ValidateCredentials(username, password);
        username = NormalizeUsername(username);
        if (roleCode is not ("MANAGER" or "CASHIER" or "WAREHOUSE"))
            throw new ArgumentException("Vai trò tài khoản không hợp lệ.");
        if (roleCode != "MANAGER" && string.IsNullOrWhiteSpace(employeeName))
            throw new ArgumentException("Nhập họ tên nhân viên để tạo hồ sơ gắn với tài khoản.");

        var (hash, salt) = PasswordHasher.Hash(password);
        using var connection = OracleConnectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            int? employeeId = null;
            if (roleCode != "MANAGER")
            {
                using var employeeCommand = connection.CreateCommand();
                employeeCommand.Transaction = transaction;
                employeeCommand.BindByName = true;
                employeeCommand.CommandText = "INSERT INTO EMPLOYEE (EMPLOYEE_ID, FULL_NAME, PHONE) VALUES (SEQ_EMPLOYEE.NEXTVAL, :fullName, :phone) RETURNING EMPLOYEE_ID INTO :employeeId";
                employeeCommand.Parameters.Add("fullName", OracleDbType.NVarchar2, 100).Value = employeeName.Trim();
                employeeCommand.Parameters.Add("phone", OracleDbType.Varchar2, 20).Value = string.IsNullOrWhiteSpace(employeePhone) ? DBNull.Value : employeePhone.Trim();
                var output = employeeCommand.Parameters.Add("employeeId", OracleDbType.Int32);
                output.Direction = System.Data.ParameterDirection.Output;
                employeeCommand.ExecuteNonQuery();
                employeeId = Convert.ToInt32(output.Value.ToString());
            }

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.BindByName = true;
            command.CommandText = """
                INSERT INTO APP_USER (USER_ID, USERNAME, PASSWORD_HASH, PASSWORD_SALT, ROLE_CODE, EMPLOYEE_ID, IS_ACTIVE)
                VALUES (SEQ_APP_USER.NEXTVAL, :username, :hash, :salt, :roleCode, :employeeId, 1)
                """;
            command.Parameters.Add("username", OracleDbType.Varchar2, 40).Value = username.Trim();
            command.Parameters.Add("hash", OracleDbType.Varchar2, 100).Value = hash;
            command.Parameters.Add("salt", OracleDbType.Varchar2, 100).Value = salt;
            command.Parameters.Add("roleCode", OracleDbType.Varchar2, 20).Value = roleCode;
            command.Parameters.Add("employeeId", OracleDbType.Int32).Value = employeeId is null ? DBNull.Value : employeeId.Value;
            command.ExecuteNonQuery();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void AddCredentials(OracleCommand command, string username, string hash, string salt)
    {
        command.Parameters.Add("username", OracleDbType.Varchar2, 40).Value = username.Trim();
        command.Parameters.Add("hash", OracleDbType.Varchar2, 100).Value = hash;
        command.Parameters.Add("salt", OracleDbType.Varchar2, 100).Value = salt;
    }

    private static void ValidateCredentials(string username, string password)
    {
        username = username.Trim();
        if (username.Length is < 3 or > 40 || username.Any(c => !(char.IsLetterOrDigit(c) || c is '_' or '.' or '-')))
            throw new ArgumentException("Tên đăng nhập cần 3–40 ký tự, gồm chữ, số, dấu chấm, gạch dưới hoặc gạch ngang.");
        if (password.Length is < 8 or > 128)
            throw new ArgumentException("Mật khẩu cần từ 8 đến 128 ký tự.");
    }

    private static string NormalizeUsername(string username) => username.Trim().ToUpperInvariant();
}
