namespace ShoeStore.WinForms.Auth;

internal sealed record UserSession(int UserId, string Username, string RoleCode, int? EmployeeId)
{
    public bool IsManager => RoleCode == "MANAGER";

    public bool HasAnyRole(params string[] roles) => IsManager || roles.Contains(RoleCode, StringComparer.OrdinalIgnoreCase);

    public void Require(params string[] roles)
    {
        if (!HasAnyRole(roles))
            throw new UnauthorizedAccessException("Tài khoản hiện tại không có quyền thực hiện chức năng này.");
    }
}
