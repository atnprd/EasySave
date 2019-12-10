using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasySave.View
{
    public class Frame : Window
    {
        public Window window = new Window();
        public static Grid RootGrid;

        public void InitFrame()
        {
            window.Width = 600;
            window.Height = 750;
            window.Title = "EasySave";
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            RootGrid = new Grid();
            RootGrid.Height = window.Width;
            RootGrid.Width = window.Height;
            RootGrid.Background = Brushes.White;
            GridFrame.CreateWorkList();
            window.Content = RootGrid;
            window.Show();
            System.Windows.Threading.Dispatcher.Run();
        }
    }
}
