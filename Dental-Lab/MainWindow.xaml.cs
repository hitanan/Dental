using Dental_Lab.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dental_Lab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Toggle_Menu_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuAction();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonCloseMenu.Visibility = Visibility.Visible;
        //    ButtonOpenMenu.Visibility = Visibility.Collapsed;
        //}

        //private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonCloseMenu.Visibility = Visibility.Collapsed;
        //    ButtonOpenMenu.Visibility = Visibility.Visible;
        //}

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();

            ToggleMenu.IsChecked = !ToggleMenu.IsChecked;
            ToggleMenuAction();
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemHome":
                    usc = new Client();
                    GridMain.Children.Add(usc);
                    break;
                case "ItemCreate":
                    usc = new Client();
                    GridMain.Children.Add(usc);
                    break;
                default:
                    break;
            }
        }


        private void ToggleMenuAction()
        {
            var sb = (Storyboard)FindResource((bool)ToggleMenu.IsChecked ? "OpenMenu" : "CloseMenu");
            sb.Begin();
        }
    }
}
