using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System.Diagnostics;

namespace SeleniumCheck
{
    public static class UIAHelper
    {
        public static void AutoLogin(string userName, string password, int timeout = 1000)
        {
            Console.WriteLine("Waiting for authorization. c(:");
            var MaxTimeout = TimeSpan.FromMilliseconds(timeout);
            var pid = Retry
                .WhileNull(
                    () => Process.GetProcessesByName("CredentialUIBroker").FirstOrDefault()?.Id,
                    timeout: MaxTimeout,
                    throwOnTimeout: true).Result;

            if (pid is null)
                return;

            Thread.Sleep(1000);

            var process = Process.GetProcessById((int)pid);
            var app = new Application(process, true);
            using (var automation = new UIA3Automation())
            {
                //var Desktop = automation.GetDesktop();
                //var window = Desktop.FindFirstDescendant(f => f.ByClassName("Credential Dialog Xaml Host")).AsWindow();
                var window = app.GetMainWindow(automation);
                window.FindFirstDescendant(cf => cf.ByName("Password"))?.AsTextBox().Enter(password);
                window.FindFirstDescendant(cf => cf.ByName("User name"))?.AsTextBox().Enter(userName);
                window.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton().Click();
            }
        }
    }
}
