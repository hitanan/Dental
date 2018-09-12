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
using Dental_Lab.Model;
using Dental_Lab.Utility;

namespace Dental_Lab.ViewModel
{

    public enum Menu { ItemSchedule, ItemClient };
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


        private int _ListViewMenuIndex;
        public int ListViewMenuIndex
        {
            get => this._ListViewMenuIndex;
            set => SetProperty(ref this._ListViewMenuIndex, value);
        }
        private object _maincontrol = new Scheduler();
        //private object _maincontrol = new Client();
        public object MainControl { get => _maincontrol; set => SetProperty(ref _maincontrol, value); }

        public Dictionary<string, object> OpenedControls = new Dictionary<string, object>();
        public MainViewModel()
        {
            ListViewMenuIndex = 0;
            CloseCommand = new RelayCommand<Window>(w => w.Close());
            ToggleCommand = new RelayCommand<Window>(w => ToggleMenuAction(w));
            ToggleMenuItemCommand = new RelayCommand<object>(obj =>
            {

                var values = (object[])obj;
                var window = (Window)values[0];
                var SelectedItem = (ListViewItem)values[1];

                //if (OpenedControls.ContainsKey(SelectedItem.Name))
                //{
                //    MainControl = OpenedControls[SelectedItem.Name];
                //}
                //else
                //{
                //    SetMainControl(SelectedItem.Name);
                //    OpenedControls.Add(SelectedItem.Name, MainControl);
                //}
                SetMainControl(SelectedItem.Name);

            });
        }
        private void ToggleMenuAction(Window w)
        {
            var sb = (Storyboard)w.FindResource(ToggleIsCheched ? "OpenMenu" : "CloseMenu");
            sb.Begin();
        }


        public void SetMainControl(Menu menu, object item = null, bool updateMenu = false)
        {
            SetMainControl(Enum.GetName(typeof(Menu), menu), item, updateMenu);
            
        }
        public void SetMainControl(string controlName, object item = null,bool updateMenu = false)
        {
            if (controlName.IsMenu(Menu.ItemSchedule))
            {
                if (updateMenu)
                {
                    ListViewMenuIndex = (int)Menu.ItemSchedule;
                }
                Scheduler scheduler;
                if (OpenedControls.ContainsKey(controlName))
                {
                    scheduler = OpenedControls[controlName] as Scheduler;
                }
                else
                {
                    OpenedControls[controlName] = scheduler = new Scheduler();
                }

                scheduler.SelectClient(item as Client);
                MainControl = scheduler;
 
            }
            else if (controlName.IsMenu(Menu.ItemClient))
            {
                if (updateMenu)
                {
                    ListViewMenuIndex = (int)Menu.ItemClient;
                }
                ClientView clientView;
                if (OpenedControls.ContainsKey(controlName))
                {
                    clientView = OpenedControls[controlName] as ClientView;

                } else
                {
                    OpenedControls[controlName] = clientView = new ClientView();
                }
                var clientViewModel = clientView.DataContext as ClientViewModel;
                if (updateMenu)
                {
                    clientViewModel.SelectedItem = item as Client ?? new Client();
                }
                clientView.DataContext = clientViewModel;
                MainControl = clientView;
 
            }
        }

    }

}
