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
    public class GridFrame
    {
        static Grid Grid = new Grid();
        public static void CreateWorkList()
        {
            Grid.Width = 600;
            Grid.Height = 750;
            Grid.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.VerticalAlignment = VerticalAlignment.Center;
            Grid.Background = Brushes.Transparent;
            View userControl = new View();
            Frame.RootGrid.Children.Add(Grid);
            Grid.Children.Add(userControl);
        }
    }
}
