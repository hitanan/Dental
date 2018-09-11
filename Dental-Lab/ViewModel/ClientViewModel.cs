using Dental_Lab.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dental_Lab.ViewModel
{
    class ClientViewModel: BaseViewModel
    {

        private ObservableCollection<Client> _List;
        public ObservableCollection<Client> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private Client _SelectedItem;
        public Client SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                //if (SelectedItem != null)
                //{
                //    Name = SelectedItem.Name;
                //    Phone = SelectedItem.Phone;
                //    Email = SelectedItem.Email;
                //    Address = SelectedItem.Address;
                //    Birthday = SelectedItem.Birthday;
                //} else
                //{
                //    Reset();
                //}
            }
        }

        //private string _Name;
        //public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }


        //private string _Phone;
        //public string Phone { get => _Phone; set { _Phone = value; OnPropertyChanged(); } }


        //private string _Address;
        //public string Address { get => _Address; set { _Address = value; OnPropertyChanged(); } }


        //private string _Email;
        //public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }



        //private DateTime? _Birthday;
        //public DateTime? Birthday { get => _Birthday; set { _Birthday = value; OnPropertyChanged(); } }


        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddAppointmentCommand { get; set; }

        public ClientViewModel()
        {
            List = new ObservableCollection<Client>(DataProvider.Ins.DB.Clients);
            Reset();
            AddCommand = new RelayCommand<object>((p) =>
            {
                //var Client = new Client() { Name = Name, Phone = Phone, Address = Address, Email = Email, Birthday = Birthday };
                //DataProvider.Ins.DB.Clients.Add(Client);
                //DataProvider.Ins.DB.SaveChanges();
                //List.Add(Client);
                Reset();
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem.Id == 0)
                {
                    DataProvider.Ins.DB.Clients.Add(SelectedItem);
                    DataProvider.Ins.DB.SaveChanges();
                    List.Add(SelectedItem);
                } 
                else
                {
                    //var Client = DataProvider.Ins.DB.Clients.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                    //Client.Name = SelectedItem.Name;
                    //Client.Phone = SelectedItem.Phone;
                    //Client.Address = SelectedItem.Address;
                    //Client.Email = SelectedItem.Email;
                    //Client.Birthday = SelectedItem.Birthday;
                    DataProvider.Ins.DB.SaveChanges();
                }
                //Reset();
            }, (p) =>
            {
                // If add
                if (SelectedItem.Id == 0)
                {
                    var emails = DataProvider.Ins.DB.Clients.Where(x => x.Email.Equals(SelectedItem.Email));
                    if (emails.Count() > 0 )
                        return false;
                } // If edit
                else  if (String.IsNullOrEmpty(SelectedItem.Name))
                     return false;

                return true;

            });
            DeleteCommand = new RelayCommand<object>((p) => Reset(), (p) => SelectedItem.Id > 0);

            AddAppointmentCommand = new RelayCommand<object>((p) =>
            {
                // sent Id to window
                Window parentWindow = Application.Current.MainWindow;
                var mainViewModel = parentWindow.DataContext as MainViewModel;
                //mainViewModel.SetMainControl("ItemSchedule", true);
                mainViewModel.SetMainControl(Menu.ItemSchedule, SelectedItem, false);

            }, (p) => SelectedItem.Id > 0);
        }

        private void Reset()
        {
            SelectedItem = new Client();
        }

    }
}
