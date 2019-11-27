using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;



namespace EasySave.Model
{
    public sealed class DailyLog
        //Class that generate the daily saves in a JSON file
    {
        private static readonly Lazy<DailyLog> lazy =
      new Lazy<DailyLog>(() => new DailyLog());

        public static DailyLog Instance { get { return lazy.Value; } }


        private FileInfo file;
        private string file_size;
        private string file_name;
        private string file_date;
        private double transfer_time;

        Stopwatch stopwatch = new Stopwatch();

        private DateTime today;
        private string todayString;

        private DailyLog()
            //DailyLog's constructor that instanciate some viariables
        {
            file_size = "0";
            today = DateTime.Today;
            todayString = today.ToString("yyyyMMdd");
            transfer_time = 0;
            ;
        }

        public void SetPaths( string _path)
        {
            this.file = new FileInfo(_path);
        }



        public void millisecondEarly()
            //method begining a stopwatch
        {
            stopwatch.Restart();
        }

        public void millisecondFinal()
            //method ending the stopwatch and then get the transfer time in milliseconds
        {
            stopwatch.Stop();
            transfer_time = stopwatch.Elapsed.TotalMilliseconds;
        }

        private void fileSize()
            //method getting the file's size 
        {
            file_size = file.Length + " ko";
            
        }
        private void fileDate()
            //method getting the file's date
        {
            var culture = new CultureInfo("fr-FR");
            file_date = file.LastWriteTime.ToString();
        }

        private void fileName()
            //method getting the file's name 
        {
            file_name = file.Name;
        }
       
        public void generateDailylog(string path, string source_folder)
        {
            dataFiles();
            write(path, source_folder);
        }


        private void dataFiles()
        //method getting all the attributes
        {
            fileDate();
            fileSize();
            fileName();

        }





        public void write(string path, string source_folder)
        // Method that writes a Json file containing all the attributes
        {
            List<FormatDailylog> data = new List<FormatDailylog>();
            data.Add(new FormatDailylog()
            {
                size_file = file_size,
                name_file = file_name,
                last_date_file = file_date,
                time_transfer = transfer_time,
                folder_source = source_folder,
                folder_target = path

            });
          

            TextWriter tsw = new StreamWriter(path + "\\"+todayString + ".json", true);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            tsw.WriteLine(json);
            tsw.Close();


        }
        
    }


    


}

