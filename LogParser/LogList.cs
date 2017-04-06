using System;
using System.Collections.Generic;


namespace LogParser
{
    public class LogList
    {

        public List<InitLogEntity> init;
        public List<LogEntity> filtered;
        public readonly string path = @"access.log";
        public LogList()
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

        public void FindAllCountries()
        {
            filtered.ForEach(x => x.FindCountryByIP());

        }
        public static void Print(List<InitLogEntity> inp) => inp.ForEach(x => x.Print());
        public static void Print(List<LogEntity> inp) => inp.ForEach(x => x.Print());
        public void PrintFiltered()
        {
            filtered.ForEach(x => x.Print());
            Console.WriteLine("Count: {0}", filtered.Count);
        }
        public void PrintInit() => init.ForEach(x => x.Print());
    }
}


