using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    interface IBackup
    {
        string name { get; set; }
        string source_folder { get; set; }
        string target_folder { get; set; }

        void LaunchSave();

        void LaunchSave(bool full_save);

        void FullSave(DirectoryInfo di, string target_path);
    }
}

