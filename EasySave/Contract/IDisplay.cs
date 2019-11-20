using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Contract
{
    interface IDisplay
    {
        void Commande(string _cmd, string _name, string _backuptype, string _source_folder, string _target_folder);
    }
}
