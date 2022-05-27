using SeleniumCheck;
using SeleniumCheck.Enum;
using SeleniumCheck.Model;

DriverManager driver = new DriverManager();
try
{
    string[] userContent = Helper.GetUserContent(args);
    Console.WriteLine("logging in...");
    driver.LoginAndCheck(userContent[0], userContent[1]);
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

    UIAHelper.AutoLogin(userContent[0], userContent[1], timeout: 30000);
    Environment.Exit(0);
}
catch (Exception ex)
{
    driver?.Dispose();
    Console.Clear();
    if (args.FirstOrDefault(x => x == "/log") != null)
        Console.WriteLine(ex.Message);
    else
        Console.WriteLine("An error has occurred. Sorry for that.");
    Console.ReadKey();
}