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
        List<SaveFiles> saveFiles = new List<SaveFiles>();
        
        
        public Display()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Enter -help to view all command console");
            Console.WriteLine("Enter -add <name> <type of backup> <source folder> <target folder> to add file in queue");
            Console.WriteLine("Enter -remove <id> or <all> to remove file in queue");
            Console.WriteLine("Enter -save <id> or <all> to save your files");
            Console.WriteLine("Enter -show to show all files");
            Console.ResetColor();

            while(true){
                string _capture = Console.ReadLine();
                string[] _capture_split = _capture.Split(' ');
                Commande(_capture_split);
            }
            
        }
        
        public void Commande(string[] _capture)
        {
            switch (_capture[0])
            {
                case "-help": 
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter -help to view all command console");
                    Console.WriteLine("Enter -add <name> <type of backup> <source folder> <target folder> to add file in queue");
                    Console.WriteLine("Enter -remove <id> or <all> to remove file in queue");
                    Console.WriteLine("Enter -save <id> or <all> to save your files");
                    Console.WriteLine("Enter -show to show all files");
                    Console.ResetColor();
                    break;
                case "-save":
                   if(_capture.Length < 2){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("please enter <id> or <all>" );
                        Console.ResetColor();
                    }else if(_capture[1] == "all"){
                        //execute backup  
                    }else{
                        int id = Convert.ToInt32(_capture[1]);
                        foreach(SaveFiles file in saveFiles){
                            if(file.id == id){
                             //execute backup   
                            }
                        }
                    }
                   break;
                case "-add":
                   if(_capture.Length < 4){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("please enter <name> <type of backup> <source folder> <target folder>" );
                        Console.ResetColor();
                    }else if(saveFiles.Count == 5){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("please remove one file in queue to add one" );
                        Console.ResetColor();
                    }else{
                        saveFiles.Add(new SaveFiles(){id=saveFiles.Count+1,name=_capture[1],backuptype=_capture[2],sourcefolder=_capture[3],targetfolder=_capture[4]});
                    }    
                    break;
                case "-remove":
                    if(_capture.Length < 2){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("please enter <id> or <all>" );
                        Console.ResetColor();
                    }else if(_capture[1] == "all"){
                        saveFiles.Clear();
                    }else{
                        for(int i=0; i < saveFiles.Count; i++){
                            if(Convert.ToString(saveFiles[i].id) == _capture[1]){
                                saveFiles.Remove(saveFiles[i]);
                                foreach(SaveFiles file in saveFiles){
                                    if(Convert.ToInt32(file.id)>i+1){
                                        file.id = file.id -1;
                                    }
                                }  
                            }
                        }
                    } 
                    break;
                case "-show":
                    foreach(SaveFiles file in saveFiles){
                        Console.WriteLine(file.id +"." +file.name);
                    }
                    break;
            }
        }
    }class SaveFiles{  
        public int id;
        public string name;
        public string backuptype;
        public string sourcefolder;
        public string targetfolder;
    }
}
