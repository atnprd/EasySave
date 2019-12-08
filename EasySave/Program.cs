using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Model;

using EasySave.Controller;
using EasySave.View;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {

            MainController controller = new MainController();
            controller.Run();
            Console.Read();
        }
    }
}
