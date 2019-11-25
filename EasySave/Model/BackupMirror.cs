using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    class BackupMirror : IBackup
    {
        public BackupMirror(string _name, string _source_folder, string _target_folder)
        {
            DirectoryInfo diSource = new DirectoryInfo(@_source_folder);
            if (!diSource.Exists)
            {
                Console.WriteLine("Source file not found");
            }

            name = _name;
            source_folder = _source_folder;
            target_folder = _target_folder;
            m_realTimeMonitoring = new RealTimeMonitoring(source_folder, target_folder);
        }

        private int currentFile;
        private RealTimeMonitoring m_realTimeMonitoring;
        private string m_name;
        private string m_source_folder;
        private string m_target_folder;

        public string name { get => m_name; set => m_name = value; }
        public string source_folder { get => m_source_folder; set => m_source_folder = value; }
        public string target_folder { get => m_target_folder; set => m_target_folder = value; }

        public void LaunchSave()
        {
            currentFile = 0;
            DirectoryInfo di = new DirectoryInfo(m_source_folder);
            string path = target_folder + '/' + name;
            FullSave(di, path);
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
                m_realTimeMonitoring.GenerateLog(currentFile);
                currentFile++;
                string temp_path = target_path + '/' + fi.Name;
                Console.WriteLine("copy : " + fi.Name);
                fi.CopyTo(temp_path, true);
            }
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string temp_path = target_path +'/'+ subdir.Name;
                FullSave(subdir, temp_path);
            }
        }

        public void LaunchSave(bool state)
        {
            LaunchSave();
        }
    }
}
