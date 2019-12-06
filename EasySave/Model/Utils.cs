using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    static class Utils
    {
        public static int Crypt(string source_file, string target_file)
        {
            try
            {
                using (Process p = new Process())
                {
                    string cmd = "dotnet run --project D:/git/CryptoSoftMini/CryptoSoftMini " + source_file + " " + target_file;
                    p.StartInfo = new ProcessStartInfo("cmd.exe")
                    {
                        RedirectStandardOutput = false,
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                    };
                   
                    

                    p.Start();
                    p.StandardInput.Write(cmd + p.StandardInput.NewLine);
                    p.WaitForExit(1500);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }

        private static void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("ui");
        }
    }
}

           

        
