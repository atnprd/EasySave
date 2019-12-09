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
        //method to use CryptoSoft
        //CryptoSoft take two arguments, source file and target file and return crypt time in ms or -1 if an error happend
        //CryproSoft take the source file, crypt it and then copy it in the the taget file
        public static string Crypt(string source_file, string target_file)
        {
            string ret;
           
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "../../../CryptoSoftMini/CryptoSoftMini.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = source_file + " " + target_file;


            try 
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                   ret = exeProcess.StandardOutput.ReadToEnd() + " ms";
                    exeProcess.WaitForExit();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = null;
            }
            return ret;
        }

        //read the json file that list all extension to crypt and return an array of string with the extension to crypt
        private static string[] getCryptList(string path_to_crypt_list)
        {
            using (StreamReader r = new StreamReader(path_to_crypt_list))
            {
                CryptListFormat[] item_cryptlist;
                string[] cryptlist_extensions_array;
                string json = r.ReadToEnd();
                List<CryptListFormat> items = JsonConvert.DeserializeObject<List<CryptListFormat>>(json);
                item_cryptlist = items.ToArray();
                cryptlist_extensions_array = item_cryptlist[0].extension_to_crypt.Split(',');

                return cryptlist_extensions_array;
            }
        }

        //check if a given extension is the list to crypt
        public static bool IsToCrypt(string extension)
        {
                foreach (string crypt_ext in getCryptList("../../Model/crypt_extension.json"))
                {
                    if(crypt_ext == extension)
                    {
                    return true;
                    }
                }
                return false;
        }

        //read the json file of blacklisted apps and return the array list of blacklisted apps
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

        //check if process in blacklist apps are currently running
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

           

        
