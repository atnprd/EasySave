using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    class BackupDiff : IBackup

    {
        public BackupDiff(string _name, string _source_folder, string _target_folder)
        {
            //check if source directory exist
            DirectoryInfo diSource = new DirectoryInfo(@_source_folder);
            if (!diSource.Exists)
            {
                Console.WriteLine("Source file not found");
            }

            name = _name;
            source_folder = _source_folder;
            target_folder = _target_folder;
            first_save = true;
            m_realTimeMonitoring = RealTimeMonitoring.Instance;
            m_realTimeMonitoring.SetPaths(source_folder, target_folder);
        }

        private RealTimeMonitoring m_realTimeMonitoring;
        private DailyLog m_daily_log;

        private int current_file;
        private string m_name;
        private string m_source_folder;
        private string m_target_folder;
        private bool m_first_save;
       
        public string name { get => m_name; set => m_name = value; }
        public string source_folder { get => m_source_folder; set => m_source_folder = value; }
        public string target_folder { get => m_target_folder; set => m_target_folder = value; }
        public bool first_save { get => m_first_save; set => m_first_save = value; }


        //launch save, check if it is the first save, if it is do a full save, else do an incremental save
        public void LaunchSave()
        {
            current_file = 0;
            DirectoryInfo di = new DirectoryInfo(m_source_folder);
            if (first_save)
            {
                string target_path = target_folder + '/' + name + "/completeBackup/";
                first_save = false;
                FullSave(di, target_path);
            }
            else
            {
                string complete_path = target_folder + '/' + name + "/completeBackup/";
                string target_path = target_folder + '/' + name + "/tempBackup/";

                IncrementSave(di, target_path, complete_path);
            }
        }
        //launch save, user choose if it is a full or incremental save, if it is the first save he can't force incremental save
        public void LaunchSave(bool full_save)
        {
            current_file = 0;
            DirectoryInfo di = new DirectoryInfo(m_source_folder);
            if (!full_save && !first_save)
            {
                string complete_path = target_folder + '/' + name + "/completeBackup/";
                string target_path = target_folder + '/' + name + "/tempBackup/";
                IncrementSave(di, target_path, complete_path);
            }
            else
            {
                string complete_path = target_folder + '/' + name + "/completeBackup/";
                first_save = false;
                FullSave(di, complete_path);
            }
        }

        //Mirror save
        public void FullSave(DirectoryInfo di, string target_path)
        {
            DirectoryInfo diTarget = new DirectoryInfo(target_path);
            if (!diTarget.Exists)
            {
                diTarget.Create();
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                m_daily_log = new DailyLog(fi.FullName);
                m_daily_log.millisecondEarly();

                m_realTimeMonitoring.GenerateLog(current_file);
                current_file++;
                string temp_path = target_path + '/' + fi.Name;
                fi.CopyTo(temp_path, true);

                m_daily_log.millisecondFinal();
                m_daily_log.generateDailylog(target_folder, source_folder);
            }
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                target_path += '/' + subdir.Name;
                FullSave(subdir, target_path);
            }
        }

        //IncrementalSave
        public void IncrementSave(DirectoryInfo di, string target_path, string complete_path)
        {
            DirectoryInfo diTarget = new DirectoryInfo(target_path);
            if (!diTarget.Exists)
            {
                diTarget.Create();
            }
            // for each file in the directory, check if it was modified
            DirectoryInfo dirComplete = new DirectoryInfo(complete_path);
            foreach (FileInfo fi in di.GetFiles())
            {
                //check if it is a new file or if the file was modified based on the full save
                if (CheckNewFile(fi, dirComplete) || CheckModification(fi, dirComplete))
                {
                    m_daily_log = new DailyLog(fi.FullName);
                    m_daily_log.millisecondEarly();

                    m_realTimeMonitoring.GenerateLog(current_file);
                    current_file++;
                    string temp_path = target_path + '/' + fi.Name;
                    fi.CopyTo(temp_path, true);

                    m_daily_log.millisecondFinal();
                    m_daily_log.generateDailylog(target_folder, source_folder);
                }
            }
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                target_path += '/' + subdir.Name;
                complete_path += '/' + subdir.Name;
                IncrementSave(subdir, target_path,complete_path);
            }
            DeleteEmptyFolder(diTarget);
        }

        //recursive method that delete all empty folder
        public void DeleteEmptyFolder(DirectoryInfo dir)
        {
            DirectoryInfo[] subdirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in subdirs)
            {
                DeleteEmptyFolder(subdir);
            }
            if (dir.GetFiles("*", SearchOption.AllDirectories).Length == 0)
            {
                dir.Delete();
            } 
        }

        //check if a file don t exist in a given directory
        private bool CheckNewFile(FileInfo fiBase, DirectoryInfo dirTarget)
        {
            bool new_file = true;
            foreach (FileInfo fi in dirTarget.GetFiles())
            {
                if (fi.Name == fiBase.Name)
                {
                    new_file = false;
                }
            }
            return new_file;
        }
        //check if a file got a modified version on an other target
        private bool CheckModification(FileInfo fiBase, DirectoryInfo dirTarget)
        {
            bool update_file = false;
            foreach (FileInfo fi in dirTarget.GetFiles())
            {
                if (fiBase.Name == fi.Name)
                {
                    if (fiBase.Length != fi.Length)
                    {
                        update_file = true;
                    }
                }
            }
            return update_file;
        }
    }
}
