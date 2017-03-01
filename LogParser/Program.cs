using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string reg_split =@"( )|("")";
            string FileNamer = DateTime.Now.Hour.ToString() + DateTime.Now.Millisecond.ToString() + DateTime.Now.Day.ToString();
            //ViewBag.Output = "No Test Data Available...........";
            //ViewBag.Details = "No Details Avalable...........";

            //// Output the start time.
            //DateTime startTime = DateTime.Now;
            //ViewBag.Output = "Program Start Time: " + startTime.ToString("T");

            // Read all log file lines into an array.       
            DataTable dt = new DataTable();    
            string[] lines = System.IO.File.ReadAllLines(@"access.log");
            dt.Columns.Add("IP_Address", typeof(string));
            dt.Columns.Add("DateTime", typeof(string));
            dt.Columns.Add("UserAgent", typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns["Day"] };

            for (int i = 0; i < lines.Length; i++)
            {

                // Create array for that line, splitting fields by sspaces.  From this point, much of our conditional logic will be specific array indexes.
                // This assumes that this program is only for schema used in the logs/access.log file.
                string[] lineArray = Regex.Split(lines[i], reg_split);

                // We don't want to use comment lines or data within the comment lines.  To avoid this, we'll assume a length of 21 items for lines[i].
                if (lines[i].Substring(0, 1) != "#" && lineArray.Length == 21)
                {

                    // Isolate lines where the request was a GET protocol on port 80. Also eliminate IPs starting with 207.114 .
                                           // Create datarow to add to data table container.
                        DataRow dr = dt.NewRow();
                        dr["IP_Address"] = lineArray[0];
                        dr["DateTime"] = lineArray[3];
                        dr["UserAgent"] = lineArray[5];
                    dt.Rows.Add(dr);

                    // Create duplicate search expression and check for duplicates.
                    // string searchExpression = "IP_Address = '" + lineArray[2].ToString() + "'";
                    //DataRow[] duplicateRow = dt.Select(searchExpression);


                    // Have the data table accept all changes.
                    dt.AcceptChanges();

                    
                }
            }

            foreach (DataRow dataRow in dt.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.Write(item.ToString());
                    Console.Write(" | ");
                }
                Console.WriteLine();
            }
            Console.WriteLine(dt);
            Console.ReadKey();




        }
    }
}
