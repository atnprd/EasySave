using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.View
{
    public class Display : IDisplay
    {
       
        public Display()
        {
            Help("-help");

        }

        public void Help(string _capture)
        {
            switch (_capture)
            {
                case "-help":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter -help to view all commands console");
                    Console.WriteLine("Enter -add <name> <mir|diff> <source folder> <target folder> to add file in queue");
                    Console.WriteLine("Enter -remove <id> or <all> to remove file in queue");
                    Console.WriteLine("Enter -save <id> or <all> to save your files");
                    Console.WriteLine("Enter -show to show all files");
                    Console.ResetColor();
                    break;
                default:
                    break;

            }
        }
        public void Error(string error)
        {
            switch(error)
            {
                case "-save":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter <id> or <all>");
                    Console.ResetColor();
                    break;
                case "-add":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter <name> <mir|diff> <source folder> <target folder>");
                    Console.ResetColor();
                    break;
                case "-add fullqueue":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please remove one file in queue to add one");
                    Console.ResetColor();
                    break;
                case "-remove":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter <id> or <all>");
                    Console.ResetColor();
                    break;
            }
        }
    }
}
