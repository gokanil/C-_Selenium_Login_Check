using SeleniumCheck;
using SeleniumCheck.Enum;
using SeleniumCheck.Model;

Helper.CheckArgsAndExit(2, args);

Helper.ConsoleHideLog();
DriverManager driver = new DriverManager();
Helper.ConsoleResetAndClear();

Console.WriteLine("logging in...");
driver.LoginAndCheck(args[0], args[1]);
Console.Clear();
driver.CheckSession();
Console.WriteLine("getting token...");
driver.WaitSslLink();

IVEResponse? data = Helper.PostRequest<IVEResponse>(Constants.AppLaunchToken, Constants.Headers, Constants.Parameters, driver.GetCookies(), deserializationType: DeserializationType.Xml);
if (data == null)
{
    Console.WriteLine("An error occurred while retrieving the token!");
    return;
}

Console.Clear();
driver.CheckPulseSecure();
driver.ConnectToRDP(data.APPLaunchToken);

driver.Dispose();