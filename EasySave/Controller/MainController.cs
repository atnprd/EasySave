using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasySave.View;
using EasySave.Model;
using System.Threading;
using System.Windows;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace EasySave.Controller
{
    public class MainController : IMainController
    {
        List<IBackup> m_backup = new List<IBackup>();
        List<Thread> m_threads_list = new List<Thread>();
        IDisplay display = new Display();
        Thread frameThread;

        public delegate void DELEG();

        string[] m_blacklisted_apps = Utils.getBlacklist();

        public List<IBackup> backup { get => m_backup; set => m_backup = value; }
        public View.View View { get; set; }
        public string[] blacklisted_apps { get => m_blacklisted_apps; set => m_blacklisted_apps = value; }
        public List<Thread> threads_list { get => m_threads_list; set => m_threads_list = value; }

        public MainController()
        {

            /*while (true) {
                string _capture = display.Readline();
                string[] _capture_split = _capture.Split(' ');

                
                Process_console(_capture_split);
                
            }
            */
        }

        private static Mutex _mutex = null;
        public void Run()
        // Program Entry
        {

            const string appName = "EasySave";
            bool createdNew;
            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                Application.Current.Shutdown();
            }

            CheckExistingFiles();

            DistantConsoleServer server = new DistantConsoleServer(this);
            Thread ServerThread = new Thread(server.RunServer);
            ServerThread.Start();
            
            DELEG dele1 = StartWindow;
            frameThread = new Thread(dele1.Invoke);
            frameThread.SetApartmentState(ApartmentState.STA);
            frameThread.Start();

        }
        public void StartWindow()
        // Method starting the window
        {
            Frame app = new Frame();
            app.InitFrame(this);
        }

        public void Close()
        {
            Process.GetCurrentProcess().Kill();
        }

        //method that process data in consoleMode
        private void Process_console(string[] _capture)
        {
            switch (_capture[0])
            {
                case "-save":
                    if (_capture.Length < 2)
                    {
                        display.Error("-save");
                    }
                    else if (Utils.checkBusinessSoft(blacklisted_apps))
                    {
                        display.Error("-businesswarerunning");
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
                            if (Utils.checkBusinessSoft(blacklisted_apps))
                            {
                                display.Error("-businesswarerunning");
                                break;
                            }
                            else if (file.GetType() == typeof(BackupDiff))
                            {
                                for (int i = 0; i < count; i++)
                                    if (backup.IndexOf(backupdiff[i]) == backup.IndexOf(file))
                                    {
                                        file.LaunchSaveInc(backupdifffull[i]);
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
                        foreach (IBackup file in backup)
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
                            if (backup.IndexOf(file) + 1 == id)
                            {
                                if (file.GetType() == typeof(BackupDiff))
                                {
                                    display.AskSave("diff full", file.name);
                                    string response = display.Readline();
                                    if (response == "y")
                                    {
                                        file.LaunchSaveInc(true);
                                    }
                                    else
                                    {
                                        file.LaunchSaveInc(false);
                                    }
                                }
                                else
                                {
                                    file.LaunchSave();
                                }
                                display.Success("-save", file.name);
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
                            backup.Add(new BackupDiff(_capture[1], _capture[3], _capture[4], this) { name = _capture[1], source_folder = _capture[3], target_folder = _capture[4] });
                            display.Success("-add", _capture[1]);
                        }
                        else if (_capture[2] == "mir")
                        {
                            backup.Add(new BackupMirror(_capture[1], _capture[3], _capture[4], this) { name = _capture[1], source_folder = _capture[3], target_folder = _capture[4] });
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
                        display.Success("-remove all", null);
                    }
                    else
                    {
                        int id = Convert.ToInt32(_capture[1]);
                        for (int i = 0; i < backup.Count; i++)
                        {
                            if (backup.IndexOf(backup[i]) + 1 == id)
                            {
                                backup.Remove(backup[i]);
                                display.Success("-remove", null);
                            }
                        }

                    }
                    break;
                case "-show":
                    foreach (IBackup file in backup) {
                        string show = backup.IndexOf(file) + 1 + "." + file.name;
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
        public string Add_save(string name, string source_folder, string target_folder, string backuptype)
        // method that adds a save to the back up list; this method also checks every exceptions
        {
            if (name == "")
            {
                return "error_name";
            } else if (source_folder == "")
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
                backup.Add(new BackupDiff(name, source_folder, target_folder, this) { name = name, source_folder = source_folder, target_folder = target_folder });

                return "success_backupdiff";
            }
            else if (backuptype == "mirr")
            {
                backup.Add(new BackupMirror(name, source_folder, target_folder, this) { name = name, source_folder = source_folder, target_folder = target_folder });
                return "success_backupmirr";
            }
            else
            {
                return null;
            }

        }
        public string Remove_task(int indextask)
        // Method that removes 1 task from the backup list
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
        // Method that removes all tasks from the backup list
        {
            if (backup.Count != 0)
            {
                string current_thread = "";
                foreach (var th in this.threads_list)
                {
                    if (th.IsAlive)
                    {
                        current_thread = current_thread + " " + th.Name;
                    }
                }
                if (current_thread != "")
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("these task(s) are still in progress:" + current_thread + " ,do you want to stop them?", " close", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (var th in this.threads_list)
                        {
                            th.Abort();
                        }
                        return "success_deleteall";
                    }
                }
            }
            return null;
        }
        public string Save_alltasks()
        // Method that saves all tasks in the backup list
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

            List<Thread> taskThreads = new List<Thread>();

            int y = 0;
            for (int i = 0; i < backup.Count; i++)
            {
                if (backup[i].GetType() == typeof(BackupDiff))
                {
                    backupdiff[y] = backup[i];
                    MessageBoxResult response = new View.View(this).Messbx(backup[i].name);

                    if (response == MessageBoxResult.No)
                    {
                        backupdifffull[y] = false;
                    }
                    else if (response == MessageBoxResult.Yes)
                    {
                        backupdifffull[y] = true;
                    }
                    y++;
                }

                y = 0;
            }
            foreach (IBackup file in backup)
            {
                if (file.GetType() == typeof(BackupDiff))
                {
                    Thread th = new Thread(new ParameterizedThreadStart(file.LaunchSaveInc));
                    th.Start(backupdifffull[y]);
                    th.Name = file.name;
                    threads_list.Add(th);
                    y++;
                }
                else
                {
                    Thread th = new Thread(file.LaunchSave);
                    th.Start();
                    th.Name = file.name;
                    threads_list.Add(th);
                }
            }
            return null;
        }
        public string Save_task(int indextask)
        // Method that saves 1 task in the backup list
        {
            foreach (IBackup task in backup)
            {
                if (backup.IndexOf(task) == indextask)
                {
                    if (Utils.checkBusinessSoft(blacklisted_apps))
                    {
                        return "businesswarerunning";
                    }
                    else if (task.GetType() == typeof(BackupDiff))
                    {
                        return "backupdiff";
                    }
                    else if (task.GetType() == typeof(BackupMirror))
                    {
                        Thread threadsave = new Thread(task.LaunchSave);
                        threadsave.Start();
                        threadsave.Name = task.name;
                        threads_list.Add(threadsave);
                        return "success_mirr";
                    }
                }
            }
            return null;

        }
        public string Save_diff(bool fulldiff, int indextask)
        // Method that saves as a differencial backup when pressing yes on the popup
        {
            foreach (IBackup task in backup)
            {
                if (backup.IndexOf(task) == indextask)
                {
                    Thread threadsave = new Thread(new ParameterizedThreadStart(task.LaunchSaveInc));
                    threadsave.Start(fulldiff);
                    threadsave.Name = task.name;
                    threads_list.Add(threadsave);
                    return "success_diff";
                }
            }
            return null;
        }
        public IBackup Last_backup()
        // return last backup added
        {
            return backup.Last();
        }

        public Dictionary<string, Dictionary<string,string>> getLanguageDict()
        //language swap method
        {
            return Utils.loadLanguage();
        }
        public string Informations_items(int index)
        //method associating saves with save informations
        {
            foreach (IBackup item in backup)
            {
                if (backup.IndexOf(item) == index)
                {
                    return item.name + "*" + item.source_folder + "*" + item.target_folder;
                }
            }
            return null;
        }
        public bool IsAPriorityTaskRunning()
        {
            bool ret = false;
            foreach (IBackup backup in backup)
            {
                if (backup.priority_work_in_progress)
                {
                    ret = true;
                }
            }
            return ret;
        }
        public void Update_progressbar()
        // Progress bar method
        {
            if (View.current_name == null && View.current_targetpath == null)
            {
            }
            else
            {
                string val =Utils.JsonReader(View.current_targetpath + "/realtime_log_" + View.current_name + ".json", "backup_progress");
                if (val != "")
                {
                    View.Dispatcher.BeginInvoke(new Action(() => { View.progressbartask.Value = Convert.ToInt16(val); }));
                }
                View.Refresh();
            }
        }
        public void Play_Pause(string name)
        // Play\Pause method
        {
            for (int i = 0; i < threads_list.Count; i++)
            {
                if (threads_list[i].Name == name)
                {
                    if (threads_list[i].ThreadState == System.Threading.ThreadState.Suspended)
                    {
                        threads_list[i].Resume();
                    }
                    else
                    {
                        threads_list[i].Suspend();
                    }

                }
            }
        }

        public void Stop(string name)
        // Stop method
        {
            for (int i = 0; i < threads_list.Count; i++)
            {
                try
                {
                    if (threads_list[i].Name == name)
                    {
                        threads_list[i].Abort();
                        threads_list.Remove(threads_list[i]);
                        View.progressbartask.Value = 0;
                    }
                }
                catch
                {
                } 
            }
        }

        public void KillThread(string name)
        // Method stopping all threads
        {
            for (int i = 0; i < threads_list.Count; i++)
            {
                try
                {
                    if (threads_list[i].Name == name)
                    {
                        threads_list[i].Abort();
                        threads_list.Remove(threads_list[i]);
                        
                    }
                }
                catch
                {
                }
            }
        }

        public void CheckExistingFiles()
        {
            if (!File.Exists(ConfigurationSettings.AppSettings["CryptoSoftPath"]))
            {
                MessageBox.Show("CryptoSoft not found");
                Process.GetCurrentProcess().Kill();
            }
            if (!File.Exists(ConfigurationSettings.AppSettings["softwareBlacklist"]))
            {
                MessageBox.Show("Software blacklist not found");
                Process.GetCurrentProcess().Kill();
            }
            if (!File.Exists(ConfigurationSettings.AppSettings["ExtensionList"]))
            {
                MessageBox.Show("List of extensions to crypt not found");
                Process.GetCurrentProcess().Kill();
            }
            if (!File.Exists(ConfigurationSettings.AppSettings["PriorityList"]))
            {
                MessageBox.Show("List of extensions to prioritarize not found");
                Process.GetCurrentProcess().Kill();
            }
            if (!File.Exists(ConfigurationSettings.AppSettings["LanguageDict"]))
            {
                MessageBox.Show("Language dictionnary not found");
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
