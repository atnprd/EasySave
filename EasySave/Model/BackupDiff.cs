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
        public BackupDiff(string _name, string _source_folder, string _target_folder, RealTimeMonitoring realTimeMonitoring)
        {
            DirectoryInfo diSource = new DirectoryInfo(@_source_folder);
            if (!diSource.Exists)
            {
                Console.WriteLine("Source file not found");
            }

            name = _name;
            source_folder = _source_folder;
            target_folder = _target_folder;
            first_save = true;
            m_realTimeMonitoring = realTimeMonitoring;
        }

        private int current_file;
        private string m_name;
        private string m_source_folder;
        private string m_target_folder;
        private bool m_first_save;
        private RealTimeMonitoring m_realTimeMonitoring;

        public string name { get => m_name; set => m_name = value; }
        public string source_folder { get => m_source_folder; set => m_source_folder = value; }
        public string target_folder { get => m_target_folder; set => m_target_folder = value; }
        public bool first_save { get => m_first_save; set => m_first_save = value; }

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

        public void LaunchSave(bool full_save)
        {
            current_file = 0;
            DirectoryInfo di = new DirectoryInfo(m_source_folder);
            if (!full_save && !first_save)
            {
                string complete_path = target_folder + '/' + name + "/completeBackup/";
                string target_path = target_folder + '/' + name + "/tempBackup/";
                if (Directory.Exists(target_folder))
                {
                    Directory.Delete(target_folder, true);
                }
                IncrementSave(di, target_folder, complete_path);
            }
            else
            {
                string complete_path = target_folder + '/' + name + "/completeBackup/";
                first_save = false;
                FullSave(di, m_target_folder);
            }
        }

        public void FullSave(DirectoryInfo di, string target_path)
        {
            DirectoryInfo diTarget = new DirectoryInfo(target_path);
            if (!diTarget.Exists)
            {
                diTarget.Create();
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                m_realTimeMonitoring.GenerateLog(current_file);
                current_file++;
                string temp_path = target_path + '/' + fi.Name;
                Console.WriteLine("copy : " + fi.Name);
                fi.CopyTo(temp_path, true);
            }
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                target_path += '/' + subdir.Name;
                FullSave(subdir, target_path);
            }
        }

        public void IncrementSave(DirectoryInfo di, string target_path, string complete_path)
        {
            DirectoryInfo diTarget = new DirectoryInfo(target_path);
            if (!diTarget.Exists)
            {
                diTarget.Create();
            }
            DirectoryInfo dirComplete = new DirectoryInfo(complete_path);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (CheckNewFile(fi, dirComplete) || CheckModification(fi, dirComplete))
                {
                    m_realTimeMonitoring.GenerateLog(current_file);
                    current_file++;
                    string temp_path = target_path + '/' + fi.Name;
                    Console.WriteLine("copy : " + fi.Name);
                    fi.CopyTo(temp_path, true);
                }
            }
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                target_path += '/' + subdir.Name;
                FullSave(subdir, target_path);
            }
        }

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

        private bool CheckModification(FileInfo fiBase, DirectoryInfo dirTarget)
        {
            bool update_file = false;
            foreach (FileInfo fi in dirTarget.GetFiles())
            {
                if (fiBase.Name == fi.Name)
                {
                    if (fiBase.Length == fi.Length)
                    {
                        update_file = true;
                    }
                    else
                    {
                        update_file = false;
                    }
                }
                else
                {
                    update_file = false;
                }
            }
            return update_file;
        }
    }
}
