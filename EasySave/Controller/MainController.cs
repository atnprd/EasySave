using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasySave.View;
using EasySave.Model;

namespace EasySave.Controller
{
    public class MainController
    {

        List<IBackup> backup = new List<IBackup>();
        IDisplay display = new Display();

        public MainController()
        {
            while (true) {
                string _capture = display.Readline();
                string[] _capture_split = _capture.Split(' ');
                Process(_capture_split);
            }
        }
        //method that process data
        private void Process(string[]  _capture)
        {
            switch (_capture[0])
            {
                case "-save":
                    if (_capture.Length < 2)
                    {
                        display.Error("-save");
                    }
                    else if (_capture[1] == "all")
                    {
                        foreach (IBackup file in backup)
                        {
                            file.LaunchSave();
                            display.Success("-save");
                        }
                    }
                    else
                    {
                        int id = Convert.ToInt32(_capture[1]);
                        foreach (IBackup file in backup)
                        {
                            if (backup.IndexOf(file)+1 == id)
                            {
                                if(file.GetType() == typeof(BackupDiff) )
                                {
                                    display.Success("diff full");
                                    string response = display.Readline();
                                    if (response == "y")
                                    {
                                        file.LaunchSave(true);
                                    }
                                    else
                                    {
                                        file.LaunchSave(false);
                                    }                                 
                                }
                                else { 
                                    file.LaunchSave();
                                }
                                display.Success("-save");
                            }
                        }
                    }
                    break;
                case "-add":
                    if (_capture.Length < 4)
                    {
                        display.Error("-add");
                    } else if (backup.Count == 5)
                    {
                        display.Error("-add fullqueue");
                    }
                    else {
                        if (_capture[2] == "diff")
                        {
                            backup.Add(new BackupDiff(_capture[1], _capture[3], _capture[4]) { name = _capture[1], source_folder = _capture[3], target_folder = _capture[4] });
                        }else if (_capture[2] == "mir")
                        {
                            backup.Add(new BackupMirror(_capture[1], _capture[3], _capture[4]) { name = _capture[1], source_folder = _capture[3], target_folder = _capture[4] });
                        }
                        display.Success("-add");
                    }
                    break;
                case "-remove":
                    if (_capture.Length < 2)
                    {
                        display.Error("-remove");
                    }
                    else if (_capture[1] == "all")
                    {
                        backup.Clear();
                        display.Success("-remove all");
                    }
                    else
                    {
                        int id = Convert.ToInt32(_capture[1]);
                        for (int i = 0; i < backup.Count; i++)
                        {
                            if (backup.IndexOf(backup[i])+1 == id)
                            {
                                backup.Remove(backup[i]);
                                display.Success("-remove");
                            }
                        }

                    }
                    break;
                case "-show":
                    foreach (IBackup file in backup) { 

                        Console.WriteLine(backup.IndexOf(file)+1 +"." + file.name);
                    } 
                    break;
                case "-help":
                    display.Help();
                    break;
            }
        }
    }
}
