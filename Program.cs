using System.Runtime.Versioning;

namespace ShoeStore.WinForms;

[SupportedOSPlatform("windows")]
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        while (true)
        {
            using var login = new ShoeStore.WinForms.UI.LoginForm();
            if (login.ShowDialog() != DialogResult.OK || login.Session is null) return;
            using var main = new ShoeStore.WinForms.UI.MainForm(login.Session);
            if (main.ShowDialog() != DialogResult.Retry) return;
        }
    }
}
