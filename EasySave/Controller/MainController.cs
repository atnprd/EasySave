using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasySave.View;
using EasySave.Model;
using System.Threading;
using System.Windows;

namespace EasySave.Controller
{
    public class MainController : IMainController
    { 

        List<IBackup> backup = new List<IBackup>();
        IDisplay display = new Display();
       
        public delegate void DELEG();

        public MainController()
        {
            
            /*while (true) {
                string _capture = display.Readline();
                string[] _capture_split = _capture.Split(' ');


                Process_console(_capture_split);
            }*/
        }
        public void Run()
        {
            DELEG dele1 = StartWindow;
            Thread frameThread = new Thread(dele1.Invoke);
            frameThread.SetApartmentState(ApartmentState.STA);
            frameThread.Start();
        }
        public void StartWindow()
        {
            Frame app = new Frame();
            app.InitFrame();
        }
        //method that process data
        private void Process_console(string[]  _capture)
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
                        int count = 0;
                        
                        foreach (IBackup file in backup)
                        {
                            if (file.GetType() == typeof(BackupDiff))
                            {
                                count++; 
                            }   
                        }
                        IBackup[] backupdiff = new IBackup[count];
                        bool[] backupdifffull = new bool[count];
                        int y = 0;
                        for (int i = 0; i < backup.Count; i++)
                        {
                            if (backup[i].GetType() == typeof(BackupDiff)) {
                                backupdiff[y] = backup[i];

                                display.AskSave("diff full", backup[i].name);
                                string response = display.Readline();
                                if (response == "y")
                                {
                                    backupdifffull[y] = true;
                                }
                                else
                                {
                                    backupdifffull[y] = false;
                                }
                                y++;
                            }
                        }
                        foreach (IBackup file in backup)
                        {
                            if (file.GetType() == typeof(BackupDiff))
                            {
                                for(int i=0; i<count;i++)
                                if(backup.IndexOf(backupdiff[i]) == backup.IndexOf(file))
                                {
                                    file.LaunchSave(backupdifffull[i]);
                                    
                                }
                            }
                            else
                            {
                                file.LaunchSave();
                            }
                        }
                    }
                    else
                    {
                        foreach(IBackup file in backup)
                        {
                        string temp = file.source_folder.Substring(0, 2);
                        if (temp == @"\\")
                            {
                                Console.WriteLine("enter domain:");
                                string domaine = display.Readline();
                                Console.WriteLine("enter User name:");
                                string username = display.Readline();
                                Console.WriteLine("enter password:");
                                string password = display.Readline();

                                System.Diagnostics.Process.Start("net", @"use " + file.source_folder + @" /USER:" + domaine + @"\" + username + " " + password + " /p:no");
                                break;
                            }
                        }
                        
                        int id = Convert.ToInt32(_capture[1]);
                        foreach (IBackup file in backup)
                        {
                            if (backup.IndexOf(file)+1 == id)
                            {
                                if(file.GetType() == typeof(BackupDiff) )
                                {
                                    display.AskSave("diff full",file.name);
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
                                display.Success("-save",file.name);
                            }
                        }
                    }
                    break;
                case "-add":
                    if (_capture.Length < 5)
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
                            display.Success("-add", _capture[1]);
                        }
                        else if (_capture[2] == "mir")
                        {
                            backup.Add(new BackupMirror(_capture[1], _capture[3], _capture[4]) { name = _capture[1], source_folder = _capture[3], target_folder = _capture[4] });
                            display.Success("-add", _capture[1]);
                        }
                        
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
                        display.Success("-remove all",null);
                    }
                    else
                    {
                        int id = Convert.ToInt32(_capture[1]);
                        for (int i = 0; i < backup.Count; i++)
                        {
                            if (backup.IndexOf(backup[i])+1 == id)
                            {
                                backup.Remove(backup[i]);
                                display.Success("-remove",null);
                            }
                        }

                    }
                    break;
                case "-show":
                    foreach (IBackup file in backup) { 
                        string show = backup.IndexOf(file)+1 +"." + file.name;
                        display.Writeline(show);
                    } 
                    break;
                case "-help":
                    display.Help();
                    break;
                case "-exit":
                    break;
            }
        }
        public string Add_save(string name,string source_folder, string target_folder, string backuptype)
        {
            if (source_folder == "")
            {
                return "error_source";
            } else if (target_folder == "")
            {
                return "error_target";
            }
            else if (backuptype == "")
            {
                return "error_backuptype";
            }
            else if (backuptype == "diff")
            {
                backup.Add(new BackupDiff(name, source_folder, target_folder) { name = name, source_folder = source_folder, target_folder = target_folder });
                
                return "success_backupdiff";
            }
            else if (backuptype == "mirr")
            {
                backup.Add(new BackupMirror(name, source_folder, target_folder) { name = name, source_folder = source_folder, target_folder = target_folder });
                return "success_backupmirr";
            }
            else
            {
                return null;
            }
            
        }
        public string Remove_task(int indextask)
        {
            for (int i = 0; i < backup.Count; i++)
            {
                if (backup.IndexOf(backup[i]) == indextask)
                {
                    backup.Remove(backup[i]);
                }
            }
            return "success_delete";
        }
        public string Remove_alltasks()
        {
            backup.Clear();
            return "success_deleteall";
        }
        public void Save_alltasks()
        {
            int count = 0;

            foreach (IBackup file in backup)
            {
                if (file.GetType() == typeof(BackupDiff))
                {
                    count++;
                }
            }
            IBackup[] backupdiff = new IBackup[count];
            bool[] backupdifffull = new bool[count];
            int y = 0;
            for (int i = 0; i < backup.Count; i++)
            {
                if (backup[i].GetType() == typeof(BackupDiff))
                {
                    backupdiff[y] = backup[i];
                    View.View view = new View.View();
                    MessageBoxResult response = view.Messbx(backup[i].name);
                   
                    if (response == MessageBoxResult.No)
                    {
                        backupdifffull[y] = false;
                    }
                    else if(response == MessageBoxResult.Yes)
                    {
                        backupdifffull[y] = true;
                    }
                    y++;
                }
            }
            foreach (IBackup file in backup)
            {
                if (file.GetType() == typeof(BackupDiff))
                {
                    for (int i = 0; i < count; i++)
                        if (backup.IndexOf(backupdiff[i]) == backup.IndexOf(file))
                        {
                            file.LaunchSave(backupdifffull[i]);
                        }
                }
                else
                {
                    file.LaunchSave();
                }
            }
        }
        public string Save_task(int indextask)
        {
            foreach (IBackup task in backup)
            {
                if (backup.IndexOf(task) == indextask)
                {
                    if (task.GetType() == typeof(BackupDiff))
                    {
                        return "backupdiff";
                    }
                    else if(task.GetType() == typeof(BackupMirror))
                    {
                        task.LaunchSave();
                        return "success_mirr";
                    }

                }
            }
            return null;
            
        }
        public string Save_diff(bool fulldiff, int indextask)
        {
            foreach (IBackup task in backup)
            {
                if (backup.IndexOf(task) == indextask)
                {
                    task.LaunchSave(fulldiff);
                    return "success_diff";
                }
            }
            return null;
        }
        public IBackup Last_backup()
        {
            return backup.Last();
        }
    }
}
