using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Contract;
using EasySave.View;

using EasySave.Model;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< HEAD
            
=======
            IBackup save = new BackupDiff("test", "D:/projet/copy", "D:/projet/Test");
            save.LaunchSave();

            Console.WriteLine("");
            Console.WriteLine("Enter continuer");
            Console.Read();

            save.LaunchSave();

            Console.WriteLine("");
            Console.WriteLine("Enter pour terminer le programe");
            Console.Read();
>>>>>>> feature/saving
        }
    }
}
