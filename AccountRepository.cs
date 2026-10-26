using System.Data;
using Oracle.ManagedDataAccess.Client;
using ShoeStore.WinForms.Auth;

namespace ShoeStore.WinForms;

internal sealed class AccountRepository
{
    public DataTable Accounts(UserSession actor)
    {
        actor.Require("MANAGER");
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT u.USER_ID,u.USERNAME,u.ROLE_CODE,u.EMPLOYEE_ID,e.FULL_NAME,u.IS_ACTIVE,u.CREATED_AT FROM APP_USER u LEFT JOIN EMPLOYEE e ON e.EMPLOYEE_ID=u.EMPLOYEE_ID ORDER BY u.USERNAME";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public DataTable Employees(UserSession actor)
    {
        actor.Require("MANAGER");
        using var c = OracleConnectionFactory.Create(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT EMPLOYEE_ID,FULL_NAME FROM EMPLOYEE WHERE IS_ACTIVE=1 ORDER BY FULL_NAME";
        var dt = new DataTable(); using var a = new OracleDataAdapter(cmd); a.Fill(dt); return dt;
    }

    public void Update(UserSession actor, int userId, string role, int? employeeId, bool active)
    {
        actor.Require("MANAGER");
        if (role is not ("MANAGER" or "CASHIER" or "WAREHOUSE")) throw new ArgumentException("Vai trò không hợp lệ.");
        if (role != "MANAGER" && employeeId is null) throw new ArgumentException("Nhân viên bán hàng/kho phải gắn với hồ sơ nhân viên.");
        using var c = OracleConnectionFactory.Create(); c.Open(); using var tx = c.BeginTransaction();
        try
        {
            using (var lockTable = c.CreateCommand()) { lockTable.Transaction = tx; lockTable.CommandText = "LOCK TABLE APP_USER IN EXCLUSIVE MODE"; lockTable.ExecuteNonQuery(); }
            string username; string currentRole;
            using (var read = c.CreateCommand())
            {
                read.Transaction = tx; read.BindByName = true; read.CommandText = "SELECT USERNAME,ROLE_CODE FROM APP_USER WHERE USER_ID=:id FOR UPDATE"; read.Parameters.Add("id", OracleDbType.Int32).Value = userId;
                using var r = read.ExecuteReader(); if (!r.Read()) throw new InvalidOperationException("Tài khoản không tồn tại."); username = r.GetString(0); currentRole = r.GetString(1);
            }
            if (username.Equals(actor.Username, StringComparison.OrdinalIgnoreCase) && (!active || role != "MANAGER")) throw new InvalidOperationException("Không thể tự khóa hoặc hạ quyền tài khoản hiện đang đăng nhập.");
            if ((currentRole == "MANAGER" && role != "MANAGER") || !active)
            {
                using var count = c.CreateCommand(); count.Transaction = tx; count.BindByName = true;
                count.CommandText = "SELECT COUNT(*) FROM APP_USER WHERE ROLE_CODE='MANAGER' AND IS_ACTIVE=1 AND USER_ID<>:id"; count.Parameters.Add("id", OracleDbType.Int32).Value = userId;
                if (!active || role != "MANAGER") if (Convert.ToInt32(count.ExecuteScalar()) == 0) throw new InvalidOperationException("Hệ thống phải còn ít nhất một quản lý đang hoạt động.");
            }
            using (var update = c.CreateCommand())
            {
                update.Transaction = tx; update.BindByName = true; update.CommandText = "UPDATE APP_USER SET ROLE_CODE=:role,EMPLOYEE_ID=:employee,IS_ACTIVE=:active WHERE USER_ID=:id";
                update.Parameters.Add("role", OracleDbType.Varchar2, 20).Value = role;
                update.Parameters.Add("employee", OracleDbType.Int32).Value = employeeId is null ? DBNull.Value : employeeId.Value;
                update.Parameters.Add("active", OracleDbType.Int32).Value = active ? 1 : 0; update.Parameters.Add("id", OracleDbType.Int32).Value = userId; update.ExecuteNonQuery();
            }
            tx.Commit();
        }
        catch { tx.Rollback(); throw; }
    }
}
