using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;




namespace LogParser
{
    
    public class LogEntity
    {
        public string IP;
        public DateTime datetime;
        public bool Bot;
        public string OS;
        public string Browser;
        public string Country;
        public LogEntity (DataRow logrow)
        {
            IP = logrow["IP_Address"].ToString();
            datetime = DateTime.ParseExact(logrow["DateTime"].ToString(), "dd/MMM/yyyy:H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Browser = Regex.Match(logrow["Browser"].ToString(), @"\D*").Groups[1].ToString();
            OS = logrow["OS"].ToString();
            Country = String.Empty;
            Bot = logrow["UserAgent"].ToString().ToLower().Contains("bot") || logrow["UserAgent"].ToString().ToLower().Contains("mediapartners-google");// || logrow["UserAgent"].ToString().ToLower().Contains("http://");
        }
        public void FindCountryByIP()
        {
            Country = GetCountryByIP(IP);
        }
        public void Print ()
        {
            Console.WriteLine($"{IP}\t{Bot}\t{datetime}\t{OS}\t{Browser}\t{Country}");
            
        }
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
    public class AccessLogParser
    {
         
        public static  readonly string path = @"access.log";
        private int records;
        public DataTable initdt;
         
        public int BotsCount { get; private set; }
        public int RealRequestCount { get; private set; }
        public IEnumerable<DataRow> RealRequests { get => from row in initdt.AsEnumerable() where row["Req"].ToString().StartsWith("/book") select row; }
        public IEnumerable<DataRow> Bots { get => from row in RealRequests where (row["UserAgent"].ToString().ToLower().Contains("bot")) select row; }
        public IEnumerable<DataRow> FilteredLog { get; private set; }
        public List<LogEntity> ToList(IEnumerable<DataRow> final)
        {
            List<LogEntity> res = new List<LogEntity>();
            foreach (var e in final)
            {

                res.Add(new LogEntity(e));
            }
            return res;
        }
        public AccessLogParser ()
        {
            initdt = new DataTable();            
            initdt.Columns.Add("IP_Address", typeof(string));
            initdt.Columns.Add("DateTime", typeof(string));
            initdt.Columns.Add("Req", typeof(string));
            initdt.Columns.Add("Ref", typeof(string));
            initdt.Columns.Add("UserAgent", typeof(string));
            initdt.Columns.Add("OS", typeof(string));
            initdt.Columns.Add("Browser", typeof(string));
            initdt.Columns.Add("Country", typeof(string));
            
            System.IO.StreamReader file =  new System.IO.StreamReader(path);
            string line;
            while ((line = file.ReadLine())!=null)
            {
                
                string[] lineArray = line.Split('"');

                DataRow dr = initdt.NewRow();
                string[] line0array = lineArray[0].Split(' ');
                //string country = GetCountryByIP(line0array[0]);
                //dr["Country"] = String.Format("{0, -40}", country);
                dr["IP_Address"] =  line0array[0];
                dr["DateTime"] =  line0array[3].Substring(1);
                line0array = lineArray[1].Split(' ');
                dr["Req"] =  line0array[1];
                dr["Ref"] =  lineArray[3];
                dr["UserAgent"] = lineArray[5];

                var st = Regex.Matches(lineArray[5], @"\(([^)]*)\)");
                if (st.Count >= 1)
                    dr["OS"] = st[0].Groups[1];
                var array_ua = lineArray[5].Split(' ');
                dr["Browser"] = array_ua.Last();
                initdt.Rows.Add(dr);
                initdt.AcceptChanges();
                records++;
            }
            file.Close();
            

        }
        private void CalcReal() =>  RealRequestCount = RealRequests.Count();
        private void CalcBots() => BotsCount = Bots.Count();
        public IEnumerable<DataRow> Filer()
        {
            FilteredLog = RealRequests.GetNonBot();
            return FilteredLog;
        }
        


    }
    static class EnumExtentions
    {
        static public IEnumerable<DataRow> GetNonBot(this IEnumerable<DataRow> dt) => from row in dt where !(row["UserAgent"].ToString().ToLower().Contains("bot")) select row;

        static public IEnumerable<DataRow> GetRealReq(this IEnumerable<DataRow> dt) => from row in dt where row["Req"].ToString().StartsWith("/book") select row;

        static public int  GetBotPer(this IEnumerable<DataRow> dt)
        {
            var bots = (from row in dt.GetRealReq() where (row["UserAgent"].ToString().ToLower().Contains("bot")) select row).Count();      
            
            return bots;
        }
        

    }
    class Program
    {
        static void Main(string[] args)
        {
            AccessLogParser parser = new AccessLogParser();
            LogList list = new LogList();
            LogList.init =  parser.ToList(parser.initdt.AsEnumerable());
            Console.WriteLine(LogList.init.Count());
            var logswithoutbots = from e in LogList.init where e.Bot select e;
            list.filtered = logswithoutbots.ToList();
            //list.FindAllCountries();
            list.Print();
            Console.WriteLine(list.filtered.Count());
            var res = from e in list.filtered group e by e.IP;
            
            Console.ReadKey();
            
        }

    }

    public class LogList
    {
        
        public static  List<LogEntity> init;
        public  List<LogEntity> filtered;
        public  void FindAllCountries (  )
        {
            filtered.ForEach(x => x.FindCountryByIP());

        }
        public static void Print(List<LogEntity> inp ) => inp.ForEach(x => x.Print());
        public  void Print() => filtered.ForEach(x => x.Print());
    }
}
