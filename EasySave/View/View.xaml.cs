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
        public View()
        {
            InitializeComponent();
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
            bool mirr_check = Mirror_button.IsChecked ?? false;
            bool diff_check = Diff_button.IsChecked ?? false;

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
                    Message.Content = "Please add a Name !";
                    Message.Visibility = Visibility;
                    break;
                case "success_mirr":
                    Message.Foreground = Brushes.Green;
                    Message.Content = "Mirror Save Successfully Saved !";
                    Message.Visibility = Visibility;
                    break;
                case "success_diff":
                    Message.Foreground = Brushes.Green;
                    Message.Content = "Differential Save Successfully Saved !";
                    Message.Visibility = Visibility;
                    break;
                case "error_source":
                    Message.Foreground = Brushes.Red;
                    Message.Content = "Please add a source path !";
                    Message.Visibility = Visibility;
                    break;
                case "error_target":
                    Message.Foreground = Brushes.Red;
                    Message.Content = "Please add a target path !";
                    Message.Visibility = Visibility;
                    break;
                case "error_backuptype":
                    Message.Foreground = Brushes.Red;
                    Message.Content = "Please select a backup type !";
                    Message.Visibility = Visibility;
                    break;
                case "success_backupdiff":
                    Message.Foreground = Brushes.Green;
                    Message.Content = "Differential Save Successfully Added !";
                    Message.Visibility = Visibility;
                    break;
                case "success_backupmirr":
                    Message.Foreground = Brushes.Green;
                    Message.Content = "Mirror Save Successfully Added !";
                    Message.Visibility = Visibility;
                    break;
                case "success_addedall":
                    Message.Foreground = Brushes.Green;
                    Message.Content = "All Saves Successfully Added !";
                    Message.Visibility = Visibility;
                    break;
                case "success_delete":
                    Message.Foreground = Brushes.Green;
                    Message.Content = "Save Successfully Removed !";
                    Message.Visibility = Visibility;
                    break;
                case "success_deleteall":
                    Message.Foreground = Brushes.Green;
                    Message.Content = "Saves Successfully Removed !";
                    Message.Visibility = Visibility;
                    break;
                case "businesswarerunning":
                    Message.Foreground = Brushes.Red;
                    Message.Content = " business software running, check the blacklist file for more infos !";
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
    }
}
