using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using EasySave.Controller;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace EasySave.View
{
    /// <summary>
    /// Logique d'interaction pour View.xaml
    /// </summary>
    public partial class View : UserControl
    {
        IMainController controller = new MainController();
        string language;
        Dictionary<string, Dictionary<string, string>> language_dict;
        Dictionary<string, string> dict;

        public View()
        {
            InitializeComponent();
            language_dict = controller.getLanguageDict();
            dict = language_dict["english"];
            language = "en";
        }

        private void Add_sourcefolder(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                sourcefolder.Text = openFileDialog.FileName;
            }
        }

        private void Add_targetfolder(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                targetfolder.Text = openFileDialog.FileName;
            }
        }

        private void Add_save(object sender, RoutedEventArgs e)
        {
            bool mirr_check = mirror_button.IsChecked ?? false;
            bool diff_check = diff_button.IsChecked ?? false;

            if (mirr_check)
            {
                string response = controller.Add_save(Name.Text, sourcefolder.Text, targetfolder.Text, "mirr");
                if (response == "error_name" | response == "error_source" | response == "error_target" | response == "error_backuptype")
                {

                }
                else
                {
                    Save_task.Items.Add(controller.Last_backup().name);
                }
                Display_error_success(response);
            }
            else if (diff_check)
            {
                string response = controller.Add_save(Name.Text, sourcefolder.Text, targetfolder.Text, "diff");
                if (response == "error_name" | response == "error_source" | response == "error_target" | response == "error_backuptype")
                {

                }
                else
                {
                    Save_task.Items.Add(controller.Last_backup().name);
                }
                Display_error_success(response);
            }
            else
            {
                Display_error_success("error_backuptype");
            }
        }

        private void Delete_item(object sender, RoutedEventArgs e)
        {
            if (Save_task.SelectedItem != null)
            {
                string response = controller.Remove_task(Save_task.SelectedIndex);
                Save_task.Items.RemoveAt(Save_task.Items.IndexOf(Save_task.SelectedItem));
                Display_error_success(response);
            }
        }
        private void Delete_allitems(object sender, RoutedEventArgs e)
        {
            string response = controller.Remove_alltasks();
            while(Save_task.Items.Count > 0)
            {
                Save_task.Items.RemoveAt(0);
            }
            Display_error_success(response);
        }
        
        private void Save_item(object sender, RoutedEventArgs e)
        {
            if (Save_task.SelectedItem != null)
            {
                string response = controller.Save_task(Save_task.SelectedIndex);
                Display_error_success(response);

                if (response == "backupdiff")
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to do a full save ?", "Differential Backup", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if(result == MessageBoxResult.No)
                    {
                        string response_diff = controller.Save_diff(false, Save_task.SelectedIndex);
                        Display_error_success(response_diff);
                    }
                    else if(result == MessageBoxResult.Yes)
                    {
                        string response_diff = controller.Save_diff(true, Save_task.SelectedIndex);
                        Display_error_success(response_diff);
                    }
                }
            }
        }
        private void Display_error_success(string mess)
        {
            switch (mess)
            {
                case "error_name":
                    Message.Foreground = Brushes.Red;
                    Message.Content = dict["error_name"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "success_mirr":
                    Message.Foreground = Brushes.Green;
                    Message.Content = dict["success_mirr"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "success_diff":
                    Message.Foreground = Brushes.Green;
                    Message.Content = dict["success_diff"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "error_source":
                    Message.Foreground = Brushes.Red;
                    Message.Content = dict["error_source"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "error_target":
                    Message.Foreground = Brushes.Red;
                    Message.Content = dict["error_target"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "error_backuptype":
                    Message.Foreground = Brushes.Red;
                    Message.Content = dict["error_backuptype"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "success_backupdiff":
                    Message.Foreground = Brushes.Green;
                    Message.Content = dict["success_backupdiff"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "success_backupmirr":
                    Message.Foreground = Brushes.Green;
                    Message.Content = dict["success_backupmirr"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "success_addedall":
                    Message.Foreground = Brushes.Green;
                    Message.Content = dict["success_addedall"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "success_delete":
                    Message.Foreground = Brushes.Green;
                    Message.Content = dict["success_delete"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "success_deleteall":
                    Message.Foreground = Brushes.Green;
                    Message.Content = dict["success_deleteall"]; ;
                    Message.Visibility = Visibility;
                    break;
                case "businesswarerunning":
                    Message.Foreground = Brushes.Red;
                    Message.Content = dict["businesswarerunning"]; ;
                    Message.Visibility = Visibility;
                    break;
                default:
                    break;
            }
        }
        public MessageBoxResult Messbx(string name)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to do a full save for"+" "+ name + " ?", "Differential Backup", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            return result;
        }

        private void Saves_items(object sender, RoutedEventArgs e)
        {
            controller.Save_alltasks();

        }

        private void Save_task_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(Save_task.SelectedIndex != -1)
            { 
                string response = controller.Informations_items(Save_task.SelectedIndex);
                string [] item = response.Split('*');
                name_item.Content = item[0];
                source_item.Content = item[1];
                target_item.Content = item[2];
            }
        }

        private void Open_blacklist(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", ConfigurationSettings.AppSettings["softwareBlacklist"]);
            /*string json = File.ReadAllText("..\\..\\Model\\software_blacklist.json");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj[0]["blacklisted_items"] = Blacklist_name.Text;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("..\\..\\Model\\software_blacklist.json", output);*/
        }
        private void Open_cryptextension(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", ConfigurationSettings.AppSettings["ExtensionList"]);
            /*string json = File.ReadAllText("..\\..\\Model\\software_blacklist.json");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj[0]["blacklisted_items"] = Blacklist_name.Text;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("..\\..\\Model\\software_blacklist.json", output);*/
        }
        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            controller.Close();
        }
        private void Switch_language(object sender, RoutedEventArgs e)
        {
            dict = Switch_dict();
            save_name.Content = dict["save_name"];
            source_path.Content = dict["source_path"];
            target_path.Content = dict["target_path"];
            mirror_button.Content = dict["mirror_button"];
            diff_button.Content = dict["diff_button"];
            add_save.Content = dict["add_save"];
            blacklist_software.Content = dict["blacklist_software"];
            encrypt_extension.Content = dict["encrypt_extension"];
            save.Content = dict["save"];
            save_all.Content = dict["save_all"];
            delete.Content = dict["delete"];
            delete_all.Content = dict["delete_all"];
            task_infos.Content = dict["task_infos"];
            task_name.Content = dict["task_name"];
            task_source.Content = dict["task_source"];
            task_target.Content = dict["task_target"];
            sw_language.Content = dict["sw_language"];

        }
        
        private Dictionary<string, string> Switch_dict()
        {
            Dictionary<string, string> dict;
            switch (language)
            {
                case "fr":
                    dict = language_dict["english"];
                    language = "en";
                    return dict;
                case "en":
                    dict = language_dict["french"];
                    language = "fr";
                    return dict;
                default:
                    return language_dict["english"];
            }

            
        }
    }
}
