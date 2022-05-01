using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

namespace SeleniumCheck
{
    internal class DriverManager : IDisposable
    {
        IWebDriver driver;

        public DriverManager(bool Headless = true)
        {
            ChromeOptions options = new ChromeOptions();
            if (Headless) options.AddArgument("headless");
            string _ = new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
            driver = new ChromeDriver(options);
        }

        public void LoginAndCheck(string username, string password)
        {
            driver.Navigate().GoToUrl(Constants.LoginUrl);
            driver.FindElement(By.Name(Constants.Username)).SendKeys(username);
            driver.FindElement(By.Name(Constants.Password)).SendKeys(password);
            driver.FindElement(By.Name(Constants.LoginButton)).Submit();

            try
            {
                IWebElement loginLog = driver.FindElement(By.Id(Constants.LoginLog)).FindElement(By.TagName("td"));
                Console.Clear();
                Console.WriteLine(loginLog.Text);
                Console.ReadKey();
                Dispose();
            }
            catch
            {

            }
        }

        public void CheckSession()
        {
            try
            {
                ReadOnlyCollection<IWebElement> sessionContents = driver.FindElement(By.Id(Constants.AnotherSession)).FindElements(By.TagName("td"));

                if (sessionContents.Count > 0)
                {
                    Console.Clear();
                    Console.WriteLine("Warning!");
                    Console.WriteLine("There are already other user sessions in progress:");
                    Console.WriteLine();
                    Console.WriteLine($"{sessionContents.ElementAtOrDefault(0)?.Text}: {sessionContents.ElementAtOrDefault(2)?.Text}");
                    Console.WriteLine($"{sessionContents.ElementAtOrDefault(1)?.Text}: {sessionContents.ElementAtOrDefault(3)?.Text}");
                    Console.WriteLine();
                    Console.WriteLine("Continue will result in termination of the other session. Please any key for continue...");
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine("continuing...");
                    driver.FindElement(By.Name(Constants.SessionButton)).Submit();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void WaitSslLink()
        {
            UntilExists(By.Id(Constants.SslLink));
        }

        public ReadOnlyCollection<Cookie> GetCookies()
        {
            return driver.Manage().Cookies.AllCookies;
        }

        public void CheckPulseSecure()
        {
            if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.PulseSecurePath)))
            {
                Console.WriteLine("Pulse Secure not found! If you want to download and install press any key.");
                Console.ReadKey();
                Console.Clear();
                Helper.FileDownload(Constants.MsiUrl, Constants.MsiName);
                Console.WriteLine("installing...");
                Helper.ExecuteCommand("msiexec", $"/i {Constants.MsiName} /q");
                Helper.DeleteFile(Constants.MsiName);
            }
        }

        public void ConnectToRDP(string appLaunchToken)
        {
            string param = String.Format(Constants.SslParams, appLaunchToken, DateTimeOffset.Now.ToUnixTimeSeconds());
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.PulseSecurePath), String.Format(Constants.PulseSecure, param.ToBase64()));
        }

        public void Dispose()
        {
            driver.Dispose();
            Environment.Exit(0);
        }

        private void UntilExists(By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            IWebElement element = wait.Until(ExpectedConditions.ElementExists(locator));
        }
    }
}
