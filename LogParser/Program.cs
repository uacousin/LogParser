using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogParser
{
    
    
    
    class Program
    {
        static void Main(string[] args)
        {
            LogList parser = new LogList();           
            var temp = from e in parser.init where (!e.IsBot() && e.IsReal())  select e;
            parser.init = temp.ToList();
            
            parser.Filter(parser.init);
            //parser.FindAllCountries();
            var temp2 = from e in parser.filtered group e by e.OS into g select new { Name = g.Key, Count = g.Count() };
            temp2.ToList();
            foreach (var e in temp2)
            {
                Console.WriteLine("{0} : {1}", e.Name, e.Count);
            }
            //parser.PrintFiltered();

            //parser.init = logswithoutbots.ToList();
            
            
            Console.ReadKey();            
        }
    }



    public class LogEntity
    {
        public readonly static string [] OSList  = 
            { "Windows NT 6.1", "Windows NT 6.2", "Windows NT 6.3", "Windows NT 10.0", "Android 6", "Android 4", "Android 5", "Windows NT 5.1", "IPhone", "Mac OS X"  } ; 
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
            return res.Substring(0, res.Length-1) ;
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
        public bool IsBot(string ua) => ua.ToLower().Contains("bot") || ua.ToLower().Contains("mediapartners-google");

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
    public class InitLogEntity
    {
        public string IP;
        public string Time;
        public string status;
        public string Req;
        public string Ref;
        public string UA;
        public void Print() => Console.WriteLine($"{IP}\t{Time}\t{status}\t{Req}\t{Ref}\t{UA}");
        public bool IsBot() => UA.ToLower().Contains("bot") || UA.ToLower().Contains("mediapartners-google");
        public bool IsReal() => Req.StartsWith("/book");
    }
    public class LogList
    {
         
        public List<InitLogEntity> init;
        public List<LogEntity> filtered;
        public readonly string path = @"access.log";
        public LogList ()
        {           
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            string line;
            init = new List<InitLogEntity>();
            InitLogEntity init_temp; ;
            while ((line = file.ReadLine()) != null)
            {
                init_temp = new InitLogEntity();
                string[] lineArray = line.Split('"');                                
                string[] line0array = lineArray[0].Split(' ');

                init_temp.IP = line0array[0];
                init_temp.Time = line0array[3].Substring(1);
                line0array = lineArray[1].Split(' ');
                init_temp.Req = line0array[1];
                init_temp.Ref = lineArray[3];
                init_temp.UA = lineArray[5];
                init_temp.status = lineArray[2].Split(' ')[0];
                init.Add(init_temp);
            }
            file.Close();
        }
        public void Filter(List<InitLogEntity> p_init)
        {
            
            filtered = new List<LogEntity>();            
            foreach (var e in p_init)
            {
                filtered.Add(new LogEntity(e));
            }
        }
       
        public  void FindAllCountries (  )
        {
            filtered.ForEach(x =>  x.FindCountryByIP());

        }
        public static void Print(List<InitLogEntity> inp) => inp.ForEach(x => x.Print());
        public static void Print(List<LogEntity> inp ) => inp.ForEach(x => x.Print());
        public  void PrintFiltered() => filtered.ForEach(x => x.Print());
        public void PrintInit() => init.ForEach(x => x.Print());
    }
}
