using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace EasySave.Model

{
    class RealTimeMonitoring
    {
        private DirectoryInfo path_dir_to_backup;
        private string timestamp_information_writing;
        private int nbr_eligible_files;
        private string total_size_files_to_backup;
        private int progress;
        private int nbr_files_left;
        private string size_files_left;
        private string backing_up_file;

        public RealTimeMonitoring(string path_file)
        {
            path_dir_to_backup = new DirectoryInfo(path_file);
            timestamp_information_writing = "N/A";
            nbr_eligible_files = 0;
            total_size_files_to_backup = "N/A";
            progress = 0;
            nbr_files_left = 0;
            size_files_left = "N/A";
            backing_up_file = "N/A";
        }

        public void updateInfos(int current_file_number)
        {
            updateTimestampInfos();
            updateNbrEligibleFiles();
            updateSizeFilesToBackup();
            updateProgress(current_file_number);
            updateLeftFilesNbr(current_file_number);
            updateLeftFileSize(current_file_number);
            updateCurrentFileName(current_file_number);
        }

        public void writeJSON(string path)
        {
            List<JsonFormat> _data = new List<JsonFormat>();
            _data.Add(new JsonFormat()
            {
                writing_time = timestamp_information_writing,
                eligible_files_nbr = nbr_eligible_files,
                total_size_backup = total_size_files_to_backup,
                backup_progress = progress,
                left_files_nbr = nbr_files_left,
                left_files_size = size_files_left,
                current_backup_file = backing_up_file
            }) ;
            using (StreamWriter file = File.CreateText(path + "\\realtime_log.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, _data);
            }
        }

        private void updateTimestampInfos()
        {
            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("fr-FR");
            timestamp_information_writing = localDate.ToString(culture);
        }

        private void updateNbrEligibleFiles()
        {
            nbr_eligible_files = path_dir_to_backup.GetFiles().Length-1;
        }

        private void updateSizeFilesToBackup()
        {   
            long files_size = 0;
            FileInfo[] files = path_dir_to_backup.GetFiles("*");

            foreach (FileInfo file in files)
            {   
                files_size += file.Length;
            }
            total_size_files_to_backup = adaptFileSize(files_size);
        }

        private void updateProgress(int current_file_number)
        {
            double tmp_progress = (double)current_file_number / (double)nbr_eligible_files;
            progress = (int)(Math.Round(tmp_progress, 2) * 100);
        }

        private void updateLeftFilesNbr(int current_file_number)
        {
            nbr_files_left = nbr_eligible_files - current_file_number;
        }

        private void updateLeftFileSize(int current_file_number)
        {
            long files_size = 0;
            int i = 0;
            FileInfo[] files = path_dir_to_backup.GetFiles("*");

            foreach (FileInfo file in files)
            {   
                if(i >= current_file_number)
                {
                    files_size += file.Length;
                }
                i++;
            }
            size_files_left = adaptFileSize(files_size);
        }

        private void updateCurrentFileName(int file_number)
        {
            FileInfo[] files = path_dir_to_backup.GetFiles("*");
            int i = 0;
            foreach (FileInfo file in files)
            {
                if (i == file_number)
                {
                    backing_up_file = file.Name;
                }
                i++;
            }
        }

        private string adaptFileSize(long files_size)
        {
            string size_files = "";
            if (files_size < 999)
            {
                size_files = files_size + " o";
            }
            else if (files_size < 999999)
            {
                double left_files_size_ko = files_size / 1000d;
                size_files = Math.Round(left_files_size_ko, 2).ToString() + " Ko";
            }
            else if (files_size < 999999999)
            {
                double left_files_size_mo = files_size / 1000000d;
                size_files = Math.Round(left_files_size_mo, 2).ToString() + " Mo";
            }
            else if (files_size < 999999999999)
            {
                double left_files_size_go = files_size / 1000000d;
                size_files = Math.Round(left_files_size_go, 2).ToString() + " Go";
            }
            else
            {
                double left_files_size_to = files_size / 1000000d;
                size_files = Math.Round(left_files_size_to, 2).ToString() + " To";
            }

            return size_files;
        }
    }   
}