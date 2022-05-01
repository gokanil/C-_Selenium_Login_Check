using Konsole;
using Newtonsoft.Json;
using RestSharp;
using SeleniumCheck.Enum;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Xml.Serialization;
using Cookie = OpenQA.Selenium.Cookie;

namespace SeleniumCheck
{
    internal class Helper
    {
        #region members
        private static ProgressBar _pb;
        #endregion

        #region methods
        public static void ConsoleHideLog()
        {
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void ConsoleResetAndClear()
        {
            Console.Clear();
            Console.ResetColor();
        }

        public static T? PostRequest<T>(string Url, ICollection<KeyValuePair<string, string>> headers, ICollection<KeyValuePair<string, string>> parameters, ReadOnlyCollection<Cookie> cookies, string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome", DeserializationType deserializationType = DeserializationType.Json) where T : class
        {
            var client = new RestClient(Url);
            var request = new RestRequest(Method.POST);
            client.UserAgent = UserAgent;
            request.AddOrUpdateHeaders(headers);
            foreach (Cookie cookie in cookies) request.AddCookie(cookie.Name, cookie.Value);
            foreach (var parameter in parameters) request.AddOrUpdateParameter(parameter.Key, parameter.Value);
            IRestResponse response = client.Execute(request);
            if (deserializationType == DeserializationType.Json)
                return JsonDeserialize<T>(response.Content);
            return XmlDeserialize<T>(response.Content);
        }

        public static T? JsonDeserialize<T>(string content) where T : class
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static T? XmlDeserialize<T>(string content) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(content))
            {
                return (T?)serializer.Deserialize(reader);
            }
        }

        public static void FileDownload(string fileUrl, string fileName)
        {
            _pb = new ProgressBar(PbStyle.SingleLine, 100, fileName.Length + 14);
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    string fileIdentifier = ((WebClient)(sender)).QueryString["FileName"];
                    _pb.Refresh(e.ProgressPercentage, $"{fileIdentifier} {e.BytesReceived.BytesToMegabytes()}/{e.TotalBytesToReceive.BytesToMegabytes()} MB");
                };

                client.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                {
                    lock (e.UserState)
                    {
                        Monitor.Pulse(e.UserState);
                    }
                };

                client.QueryString.Add("FileName", fileName);

                Object _lock = new Object();
                lock (_lock)
                {
                    client.DownloadFileAsync(new Uri(fileUrl), fileName, _lock);
                    Monitor.Wait(_lock);
                }

                _pb = null;
                Console.WriteLine("Download finished!");
            }
        }

        public static void ExecuteCommand(string fileName, string arguments = "")
        {
            Process installerProcess = new Process();
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Arguments = arguments;
            processInfo.FileName = fileName;
            installerProcess.StartInfo = processInfo;
            installerProcess.Start();
            installerProcess.WaitForExit();
        }

        public static void DeleteFile(string path, string filename = "")
        {
            File.Delete(Path.Combine(path, filename));
        }

        public static void CheckArgsAndExit(int count, params string[] args)
        {
            if (args.Length == 0 || args.Any(x => string.IsNullOrEmpty(x)))
            {
                Console.WriteLine("args not valid!");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        #endregion
    }
}
