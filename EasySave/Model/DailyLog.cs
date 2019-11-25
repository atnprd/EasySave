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
    class DailyLog
    {
        private DirectoryInfo path_backup;
        private FileInfo file;
        private long file_size;
        private string file_name;
        private string file_date;
        private double transfer_time;

        Stopwatch stopwatch = new Stopwatch();

        private DateTime today ;

        public DailyLog(string path)
        {
            this.file = new FileInfo(path);
            
            file_size = 0;
            today = DateTime.Now;
            transfer_time = 0;
            path_backup = new DirectoryInfo(path);
        }




        public void millisecondEarly()
        {
            stopwatch.Restart();
        }

        public void millisecondFinal()
        {
            stopwatch.Stop();
            transfer_time = stopwatch.Elapsed.TotalMilliseconds;
        }

        public void fileSize()
        {
            FileInfo[] files = path_backup.GetFiles("*");
            
            foreach (FileInfo file in files)
            {
                file_size += file.Length;
            }
            
        }
        public void fileDate()
        {
            var culture = new CultureInfo("fr-FR");
            file_date = file.LastWriteTime.ToString();
        }

        public void fileName()
        {
            file_name = file.Name;
        }
       
        



        public void write(string path)
        {
            List<FormatDailylog> data = new List<FormatDailylog>();
            data.Add(new FormatDailylog()
            {
                size_file = file_size,
                name_file = file_name,
                last_date_file = file_date,
                time_transfer = transfer_time
            });
            using (StreamWriter file = File.CreateText(path + "\\Dailylog.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data);
            }
          

        }
        
}


    


}

