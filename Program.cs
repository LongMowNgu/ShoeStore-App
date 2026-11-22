using System.Runtime.Versioning;

namespace QuanLyBanGiay;

[SupportedOSPlatform("windows")]
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        while (true)
        {
            using var login = new QuanLyBanGiay.LoginForm();
            if (login.ShowDialog() != DialogResult.OK || login.Session is null)
            {
                QuanLyBanGiay.OracleConnectionFactory.ClearCredentials();
                return;
            }
            using var main = new QuanLyBanGiay.MainForm(login.Session);
            if (main.ShowDialog() != DialogResult.Retry)
            {
                QuanLyBanGiay.OracleConnectionFactory.ClearCredentials();
                return;
            }
            QuanLyBanGiay.OracleConnectionFactory.ClearCredentials();
        }
    }
}

