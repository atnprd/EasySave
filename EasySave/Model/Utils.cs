using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public static string[] getBlacklist(string path_to_blacklist)
        {
            using (StreamReader r = new StreamReader(path_to_blacklist))
            {
                BlacklistFormat[] item_blacklist;
                string[] blacklisted_apps_array;
                string json = r.ReadToEnd();
                List<BlacklistFormat> items = JsonConvert.DeserializeObject<List<BlacklistFormat>>(json);
                item_blacklist = items.ToArray();
                blacklisted_apps_array = item_blacklist[0].blacklisted_items.Split(',');

                return blacklisted_apps_array;
            }
        }
        public static bool checkBusinessSoft(string[] blacklisted_apps)
        {
            foreach (string process in blacklisted_apps)
            {
                if (Process.GetProcessesByName(process).Length > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

           

        
