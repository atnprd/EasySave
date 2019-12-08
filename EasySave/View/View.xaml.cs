﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using EasySave.Controller;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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
                if(response == "error_source" | response == "error_target" | response == "error_backuptype")
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
                if (response == "error_source" | response == "error_target" | response == "error_backuptype")
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
                default:
                    break;
            }
        }
    }
}
