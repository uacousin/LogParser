using System;

using System.Collections.Generic;
using System.Linq;



namespace LogParser
{



    class Program
    {
        static void Main(string[] args)
        {
            LogList parser = new LogList();
            var temp0 = from e in parser.init where (e.IsReal()) select e;
            parser.init = temp0.ToList();
            Console.WriteLine("All:" + temp0.Count());
            var temp = from e in parser.init where (!e.IsBot() && e.IsReal()) select e;
            parser.init = temp.ToList();
            Console.WriteLine("NonBot: " + temp.Count());

            Console.WriteLine("------------------------");

            parser.Filter(parser.init);
            //parser.FindAllCountries();
            int t = 0;
            for (t = 0; t < 8; t++)
            {
                Console.WriteLine("Hour: " + t);
                var temp7 = from e in parser.filtered
                            where e.datetime.TimeOfDay.Hours == t
                            select e;
                Console.WriteLine("Count: " + temp7.Count());
                Console.WriteLine("---------------------------");

            }


            var temp5 = from e in parser.filtered
                        select e.Browser;
            // temp5.Sum(x=>int.Parse(x));
            //foreach (var e in temp5.Distinct())
            //Console.WriteLine(e);

            //Console.WriteLine("-----------------");

            //--------------------------------------------------
            var temp2 = from e in parser.filtered
                        group e by e.OS into g
                        select new { Name = g.Key, Count = g.Count() };
            foreach (var e in temp2)
                Console.WriteLine("{0} : {1}", e.Name, e.Count);

            Console.WriteLine("-----------------");

            var temp3 = from e in parser.filtered
                        group e by e.Browser into g
                        select new { Name = g.Key, Count = g.Count() };
            foreach (var e in temp3)
                Console.WriteLine("{0} : {1}", e.Name, e.Count);

            Console.WriteLine("-----------------");
            //-------------------------------------------------
            var temp4 = from e in parser.filtered
                        group e by e.Country into g
                        select new { Name = g.Key, Count = g.Count() };
            foreach (var e in temp4)
                Console.WriteLine("{0} : {1}", e.Name, e.Count);

            Console.WriteLine("-----------------");

            parser.PrintFiltered();

            Console.ReadKey();
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
        public bool IsBot() => UA.ToLower().Contains("bot") || UA.ToLower().Contains("mediapartners-google") || UA.ToLower().Contains("slurp") || UA.ToLower().Contains("crawler");
        public bool IsReal() => Req.StartsWith("/book");
    }
}
   