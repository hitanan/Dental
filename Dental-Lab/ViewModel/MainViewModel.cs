using Dental_Lab.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Globalization;

namespace Dental_Lab.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        public ICommand CloseCommand { get; set; }
        public ICommand ToggleCommand { get; set; }
        public ICommand ToggleMenuItemCommand { get; set; }
        private bool _ToggleIsCheched;
        public bool ToggleIsCheched
        {
            get => _ToggleIsCheched;
            set => SetProperty(ref _ToggleIsCheched, value);
        }

        private string _MenuSelected;
        private string MenuSelected
        {
            get =>_MenuSelected;
            set => SetProperty(ref _MenuSelected, value);
        }

        public MainViewModel()
        {
            CloseCommand = new RelayCommand<Window>(null, (w) => { w.Close(); });
            ToggleCommand = new RelayCommand<Window>(null, (w) => ToggleMenuAction(w));
            ToggleMenuItemCommand = new RelayCommand<object>(null, (obj) =>
            {
                ToggleIsCheched = !ToggleIsCheched;

                var values = (object[])obj;
                var window = (Window)values[0];
                var SelectedItem = (ListViewItem)values[1];
                var GridMain = (Grid)values[2];

                ToggleMenuAction(window);

                UserControl usc = null;
                GridMain.Children.Clear();

                switch (SelectedItem.Name)
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

            });
        }
        private void ToggleMenuAction(Window w)
        {
            var sb = (Storyboard)w.FindResource(ToggleIsCheched ? "OpenMenu" : "CloseMenu");
            sb.Begin();
        }
    }

    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
