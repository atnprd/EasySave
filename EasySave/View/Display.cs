using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Contract;

namespace EasySave.View
{
    class Display : IDisplay
    {

        
        public Display()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Enter -help to view all command console");
            Console.WriteLine("Enter -add <name> <type of backup> <source folder> <target folder> to add file in queue");
            Console.WriteLine("Enter -remove <name> or <all> to remove file in queue");
            Console.WriteLine("Enter -save <name> or <all> to save your files");
            Console.ResetColor();

            string _capture = Console.ReadLine();
            string[] _capture_split = _capture.Split(' ');
           
            Commande(_capture_split[0],_capture_split[1],_capture_split[2],_capture_split[3],_capture_split[4]);
        
            
        }
        public void Commande(string _cmd, string _name, string _backuptype  ,string _source_folder, string _target_folder)
        {
            switch (_cmd)
            {
                case "-help": 
                        
                    break;
                case "-save":
                   if(_name == null){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("please enter <name> or <all>" );
                        Console.ResetColor();
                    }else{

                    }
                   break;
                case "-add":
                   if(_name == null | _backuptype == null | _source_folder == null | _target_folder == null){
                         Console.ForegroundColor = ConsoleColor.Red;
                         Console.WriteLine("please enter <name> or <all>" );
                         Console.ResetColor();
                    }
                    break;
                case "-remove":
                        
                    break;
                
            }
        }
    }
}
