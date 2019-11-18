using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    class Backup
    {
        public Backup(string _name, save_type_enum _save_type, string _source_folder, string _target_folder)
        {
            name = _name;
            save_type = _save_type;
            source_foler = _source_folder;
            target_folder = _target_folder;
        }

        private enum save_type_enum {Miror, Differential };

        private string m_name;
        private save_type_enum m_save_type;  
        private string m_source_folder;
        private string m_target_folder;

        public string name { get => m_name; set => m_name = value; }
        public save_type_enum save_type { get => m_save_type; set => m_save_type = value; }
        public string source_folder { get => source_folder; set => source_folder = value; }
        public string target_folder { get => target_folder; set => target_folder = value; }
    }
}
