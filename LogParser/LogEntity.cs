using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace LogParser
{
    public class LogEntity
    {
        public readonly static string[] OSList =
            { "Windows NT 6.1", "Windows NT 6.2", "Windows NT 6.3", "Windows NT 10.0", "Android 6", "Android 4", "Android 5", "Windows NT 5.1", "IPhone", "Mac OS X"  };
        public LogEntity()
        { }

        public string IP;
        public DateTime datetime;
        public bool Bot;
        public string OS;
        public string Browser;
        public string Country;
        public string UA;
        public string GetBrowser(string ua)
        {
            var array_ua = ua.Split(' ');
            string res = Regex.Match(array_ua.Last(), @"\D+").Groups[0].Value;
            return res.Substring(0, res.Length - 1);
        }
        public string GetOS(string ua)
        {
            var st = Regex.Matches(ua, @"\(([^)]*)\)");
            for (int i = 0; i < st.Count; i++)
            {
                foreach (var os in OSList)
                {
                    if (st[i].Groups[1].Value.ToLower().Contains(os.ToLower()))
                        return os;
                }
            }
            return "No OS";

        }
        public bool IsBot(string ua) => (ua.ToLower().Contains("bot") || ua.ToLower().Contains("mediapartners-google") || ua.ToLower().Contains("slurp"));

        public LogEntity(InitLogEntity init)
        {
            IP = init.IP;
            datetime = ParseDateTime(init.Time);
            Browser = GetBrowser(init.UA);
            OS = GetOS(init.UA);
            Country = String.Empty;
            Bot = IsBot(init.UA);
            //UA = init.UA;
        }
        public static DateTime ParseDateTime(string sditetime) => DateTime.ParseExact(sditetime.ToString(), "dd/MMM/yyyy:H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        public void FindCountryByIP() => Country = GetCountryByIP(IP);
        public void Print() => Console.WriteLine($"{IP}\t{Bot}\t{datetime}\t{OS}\t{Browser}\t{Country}\t{UA}");

        static public string GetCountryByIP(string IP)
        {

            using (var objClient = new System.Net.WebClient())
            {
                var strFile = objClient.DownloadString("http://freegeoip.net/xml/" + IP);
                var xml = new XmlDocument();
                xml.LoadXml(strFile);
                XmlNode node = xml.DocumentElement.SelectSingleNode("//Response/CountryName");
                string attr = node.InnerText;
                return attr;
            }
        }

    }
}
