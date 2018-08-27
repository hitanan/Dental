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

        private object _maincontrol = new Scheduler();
        public object MainControl { get => _maincontrol; set => SetProperty(ref _maincontrol, value); }

        public Dictionary<string, object> OpenedControls = new Dictionary<string, object>();
        public MainViewModel()
        {
            CloseCommand = new RelayCommand<Window>(w => w.Close());
            ToggleCommand = new RelayCommand<Window>(w => ToggleMenuAction(w));
            ToggleMenuItemCommand = new RelayCommand<object>(obj =>
            {

                var values = (object[])obj;
                var window = (Window)values[0];
                var SelectedItem = (ListViewItem)values[1];

                //ToggleIsCheched = !ToggleIsCheched;
                //ToggleMenuAction(window);
                if (OpenedControls.ContainsKey(SelectedItem.Name))
                {
                    MainControl = OpenedControls[SelectedItem.Name];
                }
                else
                {
                    switch (SelectedItem.Name)
                    {
                        case "ItemSchedule":
                            MainControl = new Scheduler();
                            break;
                        case "ItemClient":
                            MainControl = new Client();
                            break;
                        default:
                            break;
                    }
                    OpenedControls.Add(SelectedItem.Name, MainControl);
                }

            });
        }
        private void ToggleMenuAction(Window w)
        {
            var sb = (Storyboard)w.FindResource(ToggleIsCheched ? "OpenMenu" : "CloseMenu");
            sb.Begin();
        }
    }

}
