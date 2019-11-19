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

        private string path;
        private string backuptype;

        public Display()
        {
            Console.WriteLine("Enter -help to ask for help");
            string capture = Console.ReadLine();
            Commande(capture);
        }
        public void Commande(string cmd)
        {
            switch (cmd)
            {
                case "-help":   
                    break;
                case "-save":
                    break;
                case "-add":
                    break;
                
            }
        }
    }
}
