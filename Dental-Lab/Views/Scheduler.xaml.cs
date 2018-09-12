using Dental_Lab.Model;
using Dental_Lab.ViewModel;
using Syncfusion.UI.Xaml.Schedule;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Controls.Navigation;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dental_Lab.Views
{ 
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>

    #region Scheduler Class

    public partial class Scheduler : UserControl, INotifyPropertyChanged
    {
        #region Members
        public static string RESOURCE = "Doctors";

        internal Appointment SelectedAppointment;
        //private Client _selectedClient;
        internal Client SelectedClient;// { get => _selectedClient; set { _selectedClient = value; OnPropertyChanged(); } }
        internal BindingClass AddDataContext = null;
        Appointment copiedAppointment;
        DateTime CurrentSelectedDate;
        //Reminder reminder;

         private string _scheduleType;
         public string ScheduleType { get => _scheduleType; set { _scheduleType = value; OnPropertyChanged(); } }


        private string _scheduleResource;
        public string ScheduleResource { get => _scheduleResource; set { _scheduleResource = value; OnPropertyChanged(); } }

        public ObservableCollection<ResourceType> ResourceCollection { get; set; }

        private ObservableCollection<Client> _Clients;
        public ObservableCollection<Client> Clients { get => _Clients; set { _Clients = value; OnPropertyChanged(); } }

        private ObservableCollection<User> _doctors;
        public ObservableCollection<User> Doctors { get => _doctors; set { _doctors = value; OnPropertyChanged(); } }

        #endregion

        #region Properties

        private ScheduleAppointmentCollection _appCollection;
        public ScheduleAppointmentCollection AppCollection { get => _appCollection; set { _appCollection = value; OnPropertyChanged(); } }
        //public static List<MyClient> Clients = new List<MyClient> {
        //    new MyClient { Id=0, Code="NguyenThiThu", Name = "Nguyễn Thị Thu" },
        //    new MyClient { Id=1, Code="TruongVanDong", Name = "Trương Văn Đông" },
        //    new MyClient { Id=2, Code="HaVanTham", Name = "Hà Văn Thắm" }
        //};

        #endregion

        #region Constructor

        public Scheduler()
        {
            InitializeComponent();
            customeEditor.DataContext = this;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            CultureInfo culture = CultureInfo.CreateSpecificCulture("vi");
            Thread.CurrentThread.CurrentCulture = culture;

            ScheduleType = "Week";
            ScheduleResource = RESOURCE;

            AppCollection = new ScheduleAppointmentCollection();

            Clients = new ObservableCollection<Client>(DataProvider.Ins.DB.Clients);
            Doctors = new ObservableCollection<User>(DataProvider.Ins.DB.Users.Where(u => u.RoleId == 2));

            DateTime currentdate = DateTime.Now.Date;
            if (currentdate.DayOfWeek == System.DayOfWeek.Friday || currentdate.DayOfWeek == System.DayOfWeek.Saturday ||currentdate.DayOfWeek == System.DayOfWeek.Sunday )
                currentdate = currentdate.SubtractDays(3);
            AppCollection.Add(new Appointment()
            {
                AppointmentType = Appointment.AppointmentTypes.Health,
                Status = Schedule.AppointmentStatusCollection[0],
                StartTime = currentdate.AddHours(7),
                EndTime = currentdate.AddHours(10),
                Subject = "Checkup",
                AppointmentBackground = new SolidColorBrush(Color.FromArgb(255, 236, 12, 12)),
                Doctor = Doctors[0],
                Client = Clients[1],
                ResourceCollection = new ObservableCollection<object> { new Resource() { TypeName = RESOURCE, ResourceName = Doctors[0].UserName } }
            });

            Schedule.Appointments = AppCollection;

            // Doctor Resources
            ResourceCollection = new ObservableCollection<ResourceType>();
            ResourceType resourceType = new ResourceType { TypeName = RESOURCE };
            foreach (var item in Doctors)
            {
                resourceType.ResourceCollection.Add(new Resource { DisplayName = item.Name, ResourceName = item.UserName });
            }
            ResourceCollection.Add(resourceType);
            Schedule.ScheduleResourceTypeCollection = ResourceCollection;

            //Schedule.Resource  = null;
            DataContext = this;

            PreviewMouseLeftButtonUp += Schedule_PreviewMouseLeftButtonDown;
            Schedule.ContextMenuOpening += Schedule_PopupMenuOpening;
            Schedule.MouseLeave += new MouseEventHandler(Schedule_MouseLeave);
            Schedule.AppointmentEditorOpening += Schedule_AppointmentEditorOpening;
            Schedule.Loaded += Schedule_Loaded;
            //Schedule.ReminderOpening += Schedule_ReminderOpening;


        }

        public void SelectClient(Client client)
        {
            SelectedClient = client;
            if (customeEditor != null && customeEditor.ClientText != null && AddDataContext != null && AddDataContext.Appointment == null) {
                customeEditor.ClientText.SelectedItem = SelectedClient;
            }
        }
        #endregion

        #region Events
        void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            customeEditor.AppType.ItemsSource = Enum.GetValues(typeof(Appointment.AppointmentTypes));
            customeEditor.AppType.SelectedIndex = 0;
            customeEditor.Doctor.ItemsSource = Doctors;

            customeEditor.ClientText.AutoCompleteSource = Clients;
            //customeEditor.Client2.CustomSource = Clients;

            Schedule.PreviewMouseLeftButtonDown += Schedule_PreviewMouseLeftButtonDown;
            Schedule.PreviewMouseWheel += Schedule_PreviewMouseWheel;
        }


        #region Reminder And Radical Menu
        
        void Schedule_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (RadialPopup != null)
                RadialPopup.IsOpen = false;
        }

        void Schedule_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RadialPopup != null)
                RadialPopup.IsOpen = false;
        }

        void Schedule_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!RadialPopup.IsMouseOver && RadialPopup.IsOpen)
                RadialPopup.IsOpen = false;
        }

        void Schedule_PopupMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            RadialPopup.IsOpen = false;
            e.Cancel = true;
            if (e.CurrentSelectedDate != null)
            {
                CurrentSelectedDate = (DateTime)e.CurrentSelectedDate;
            }

            AddDataContext = new BindingClass() { CurrentSelectedDate = e.CurrentSelectedDate, Appointment = e.Appointment, SelectedResource = e.SelectedResource };
            RadialPopup.IsOpen = true;
            radialMenu.IsOpen = true;

            
            if (e.Appointment != null)
            {
                for (int i = 0; i < radialMenu.Items.Count; i++)
                {
                    if (i == 3 && copiedAppointment == null)
                    {
                        (radialMenu.Items[i] as SfRadialMenuItem).IsEnabled = false;
                        (radialMenu.Items[i] as SfRadialMenuItem).Opacity = 0.5;
                    }
                    else
                    {
                        (radialMenu.Items[i] as SfRadialMenuItem).IsEnabled = true;
                        (radialMenu.Items[i] as SfRadialMenuItem).Opacity = 1;
                    }
                }

            }
            else
            {
                (radialMenu.Items[1] as SfRadialMenuItem).IsEnabled = false;
                (radialMenu.Items[1] as SfRadialMenuItem).Opacity = 0.5;
                (radialMenu.Items[2] as SfRadialMenuItem).IsEnabled = false;
                (radialMenu.Items[2] as SfRadialMenuItem).Opacity = 0.5;
                (radialMenu.Items[5] as SfRadialMenuItem).IsEnabled = false;
                (radialMenu.Items[5] as SfRadialMenuItem).Opacity = 0.5;
                (radialMenu.Items[0] as SfRadialMenuItem).IsEnabled = true;
                if (copiedAppointment != null)
                {
                    (radialMenu.Items[3] as SfRadialMenuItem).IsEnabled = true;
                    (radialMenu.Items[3] as SfRadialMenuItem).Opacity = 1;
                }
                else
                {
                    (radialMenu.Items[3] as SfRadialMenuItem).IsEnabled = false;
                    (radialMenu.Items[3] as SfRadialMenuItem).Opacity = 0.5;
                }

            }
        }

        /*
        private void Schedule_ReminderOpening(object sender, ReminderControlOpeningEventArgs e)
        {
            e.Cancel = true;
            this.IsHitTestVisible = false;
            if (reminder == null)
            {
                reminder = new Reminder();
                reminder.Closed += reminder_Closed;
                reminder.ReminderAppCollection = e.RemindAppCollection as ScheduleAppointmentCollection;
                reminder.Show();
            }
        }

        void reminder_Closed(object sender, EventArgs e)
        {
            this.IsHitTestVisible = true;
        }

       
        */
        #endregion


        void Schedule_AppointmentEditorOpening(object sender, AppointmentEditorOpeningEventArgs e)
        {
            e.Cancel = true;
            AddDataContext = new BindingClass() { CurrentSelectedDate = e.StartTime, Appointment = e.Appointment, SelectedResource = e.SelectedResource };
            if (e.Appointment != null)
                EditAppointment();
            else
            {
                customeEditor.AppType.SelectedIndex = 0;
                AddDataContext.Appointment = null;
                AddAppointment();
            }
        }

        #region Popup Menu Click Events

        void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            //RadialPopup.IsOpen = false;
            if (this.copiedAppointment != null)
            {
                Appointment app = this.copiedAppointment;
                TimeSpan appTimeDiff = app.EndTime - app.StartTime;
                Appointment appointment = new Appointment
                {
                    Subject = app.Subject,
                    Notes = app.Notes,
                    Location = app.Location,
                    ReadOnly = app.ReadOnly,
                    AppointmentBackground = app.AppointmentBackground,
                    AppointmentType = app.AppointmentType,
                    StartTime = (DateTime)this.CurrentSelectedDate,
                    EndTime = ((DateTime)this.CurrentSelectedDate).Add(appTimeDiff)
                };

                if (AddDataContext.SelectedResource.Count > 0)
                {
                    appointment.ResourceCollection = new ObservableCollection<object> {
                        new Resource() { TypeName = Scheduler.RESOURCE, ResourceName = (AddDataContext.SelectedResource[0] as Resource).ResourceName }
                    };
                }
                Schedule.Appointments.Add(appointment);
            }
        }

        void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            copiedAppointment = (Appointment)Schedule.SelectedAppointment;
        }

        void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Schedule.SelectedAppointment != null)
                Schedule.Appointments.Remove(Schedule.SelectedAppointment);
        }

        void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditAppointment();
        }

        void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddAppointment();
        }

        #endregion

        #endregion

        #region Editor Opening Methods
        private void EditAppointment()
        {
            customeEditor.Visibility = Visibility.Visible;
            Schedule.IsHitTestVisible = false;
            customeEditor.DataContext = AddDataContext;
            SelectedAppointment = AddDataContext.Appointment as Appointment;
            //customeEditor.AddReminder.Visibility = Visibility.Collapsed;
            //customeEditor.Reminder.Visibility = Visibility.Visible;
            customeEditor.AddEndTimeMonth.Visibility = Visibility.Collapsed;
            customeEditor.AddEndTime.Visibility = Visibility.Collapsed;
            customeEditor.AddStartTimeMonth.Visibility = Visibility.Collapsed;
            customeEditor.AddStartTime.Visibility = Visibility.Collapsed;
            customeEditor.EditStartTimeMonth.Visibility = Visibility.Visible;
            customeEditor.EditStartTime.Visibility = Visibility.Visible;
            //customeEditor.EditEndTimeMonth.Visibility = Visibility.Visible;
            customeEditor.EditEndTime.Visibility = Visibility.Visible;
            customeEditor.Delete.Visibility = Visibility.Visible;
            if (AddDataContext.Appointment != null)
            {
                customeEditor.EditStartTime.DateTime = SelectedAppointment.StartTime;
                customeEditor.EditStartTimeMonth.SelectedDate = SelectedAppointment.StartTime;
                customeEditor.EditEndTime.DateTime = SelectedAppointment.EndTime;
                customeEditor.EditEndTimeMonth.SelectedDate = SelectedAppointment.EndTime;
            }
            customeEditor.Subject.Text = SelectedAppointment.Subject;
            customeEditor.Notes.Text = SelectedAppointment.Notes;

            // Populate the Doctor name by resource
            //if (AddDataContext.SelectedResource.Count > 0)
            if (SelectedAppointment.ResourceCollection != null && SelectedAppointment.ResourceCollection.Count > 0)
            {
                //customeEditor.Doctor.SelectedItem = Clients.Find(e => e.UserName.Equals((AddDataContext.SelectedResource[0] as Resource).ResourceName));
                customeEditor.Doctor.SelectedItem = Doctors.FirstOrDefault(e => e.UserName.Equals((SelectedAppointment.ResourceCollection[0] as Resource).ResourceName));
                //customeEditor.Doctor.IsEnabled = false;
            }

            customeEditor.ClientText.SelectedItem = SelectedAppointment.Client;
            customeEditor.ClearClient.Visibility = customeEditor.ViewClient.Visibility = customeEditor.ClientText.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddAppointment()
        {
            customeEditor.Doctor.IsEnabled = true;
            customeEditor.Visibility = Visibility.Visible;
            Schedule.IsHitTestVisible = false;
            SelectedAppointment = null;
            //customeEditor.AddReminder.SelectedIndex = 0;
            //customeEditor.AddEndTimeMonth.Visibility = Visibility.Visible;
            customeEditor.AddEndTime.Visibility = Visibility.Visible;
            customeEditor.AddStartTimeMonth.Visibility = Visibility.Visible;
            customeEditor.AddStartTime.Visibility = Visibility.Visible;

            if (AddDataContext.CurrentSelectedDate != null)
            {
                customeEditor.AddStartTime.DateTime = AddDataContext.CurrentSelectedDate.Value;
                customeEditor.AddStartTimeMonth.SelectedDate = AddDataContext.CurrentSelectedDate.Value;
                customeEditor.AddEndTimeMonth.SelectedDate = AddDataContext.CurrentSelectedDate.Value.AddMinutes(20);
                customeEditor.AddEndTime.DateTime = AddDataContext.CurrentSelectedDate.Value.AddMinutes(20);
            }
            else if (AddDataContext.Appointment != null)
            {
                customeEditor.AddStartTimeMonth.SelectedDate = customeEditor.AddEndTimeMonth.SelectedDate = (AddDataContext.Appointment as Appointment).StartTime;
                customeEditor.AddStartTime.DateTime = customeEditor.AddEndTime.DateTime = (AddDataContext.Appointment as Appointment).StartTime.AddMinutes(20);
            }

            //if (AddDataContext.Appointment == null)
            //{
            //    customeEditor.Client.SelectedItem = null;
            //}


            //customeEditor.AddReminder.Visibility = Visibility.Visible;
            //customeEditor.Reminder.Visibility = Visibility.Collapsed;
            customeEditor.EditStartTimeMonth.Visibility = Visibility.Collapsed;
            customeEditor.EditStartTime.Visibility = Visibility.Collapsed;
            customeEditor.EditEndTimeMonth.Visibility = Visibility.Collapsed;
            customeEditor.EditEndTime.Visibility = Visibility.Collapsed;
            customeEditor.Delete.Visibility = Visibility.Collapsed;
            customeEditor.Subject.Text = string.Empty;
            customeEditor.Notes.Text = string.Empty;
            customeEditor.DataContext = AddDataContext;


            // Populate the Doctor name by resource
            if (AddDataContext.SelectedResource.Count > 0)
            {
                customeEditor.Doctor.SelectedItem = Doctors.FirstOrDefault(e => e.UserName.Equals((AddDataContext.SelectedResource[0] as Resource).ResourceName));
            }

            customeEditor.ClientText.SelectedItem = SelectedClient;
            customeEditor.ClearClient.Visibility = customeEditor.ViewClient.Visibility = customeEditor.ClientText.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region OnPropertyChanged
        //public void OnPropertyChanged(String prop)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;

        //    if (handler != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(prop));
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddClients(Client client)
        {
            Clients.Add(client);
        }
        #endregion

    }

    #endregion

    #region Appointment Class

    public class Appointment : ScheduleAppointment, INotifyPropertyChanged
    {
        #region public properties

        private AppointmentTypes _appointmentType;
        public AppointmentTypes AppointmentType
        {
            get => _appointmentType;
            set
            {
                if (_appointmentType != value)
                {
                    _appointmentType = value;
                    OnPropertyChanged("AppointmentImageURI");
                    OnPropertyChanged("AppointmentType");
                }
            }
        }
        //public AppointmentTypes AppointmentType { get; set; }


        //private ImageSource _imageuri;
        //public ImageSource AppointmentImageURI
        //{
        //    get { return _imageuri; }
        //    set
        //    {
        //        _imageuri = value;
        //        OnPropertyChanged("AppointmentImageURI");
        //    }
        //}
        //[DependsOn("AppointmentType")]
        public ImageSource AppointmentImageURI
        {
            get {
                switch (AppointmentType)
                {
                    case Appointment.AppointmentTypes.Family:
                        {
                            return new BitmapImage(new Uri("pack://application:,,,/Assets/Cake.png"));
                        }
                    case Appointment.AppointmentTypes.Health:
                        {
                            return new BitmapImage(new Uri("pack://application:,,,/Assets/Hospital.png"));
                        }
                    case Appointment.AppointmentTypes.Office:
                        {
                            return new BitmapImage(new Uri("pack://application:,,,/Assets/Team.png"));
                        }
                    default:
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/Team.png"));
                }

            }
        }

        public string DoctorCollection
        {
            get {
                return ResourceCollection.Count > 0 ? (ResourceCollection[0] as Resource).ResourceName : null;
            }
        }
        public string AppointmentTime
        {
            get
            {
                return StartTime.ToString("hh:mm tt");
            }
        }


        public enum AppointmentTypes
        {
            Office,
            Health,
            Family
        }

        private Client _client;
        public Client Client { get => _client; set  { _client = value; OnPropertyChanged(); } }

        private User _doctor;
        public User Doctor {
            get => _doctor;
            set {

                if (_doctor != value)
                {
                    _doctor = value;
                    OnPropertyChanged();

                    ResourceCollection = new ObservableCollection<object> { new Resource() { TypeName = Scheduler.RESOURCE, ResourceName = Doctor.UserName } };
                    OnPropertyChanged("ResourceCollection");
                    OnPropertyChanged("DoctorCollection");
                }
            }
        }

        #endregion

        //private void OnPropertyChanged(string propertyName)
        //{
        //    var eventHandler = PropertyChanged;
        //    if (eventHandler != null)
        //    {
        //        eventHandler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region CustomEditor Class

    public class CustomEditor : Control
    {
        #region Constructor
        public CustomEditor()
        {
            DefaultStyleKey = typeof(CustomEditor);
            UpdateLayout();

        }
        #endregion

        #region Private Members
        public Scheduler SchedulerControl;

        public DatePicker EditStartTimeMonth;
        public DateTimeEdit EditStartTime;
        public DatePicker EditEndTimeMonth;
        public DateTimeEdit EditEndTime;
        public DatePicker AddStartTimeMonth;
        public DateTimeEdit AddStartTime;
        public DatePicker AddEndTimeMonth;
        public DateTimeEdit AddEndTime;
        public TextBox Subject;
        public TextBox Notes;
        //public TextBox Location;
        public Button Close;
        public Button Save;
        public StackPanel ShowMorePanel;
        public ComboBox Status;
        public ScrollViewer Scroll;
        public ComboBox AppType;
        public ComboBox Doctor;
        public SfTextBoxExt ClientText;
        //public AutoComplete Client2;
        //public ComboBox Reminder;
        public Button Delete;
        public Button ClearClient;
        public Button ViewClient;
        //public ComboBox AddReminder;
        public ComboBox AddStatus;

        #endregion

        #region Override Methods
        public override void OnApplyTemplate()
        {
            SchedulerControl = (Scheduler)DataContext;
            AddStartTimeMonth = GetTemplateChild("addstartmonth") as DatePicker;
            AddStartTime = GetTemplateChild("addstarttime") as DateTimeEdit;
            AddEndTimeMonth = GetTemplateChild("addendmonth") as DatePicker;
            AddEndTime = GetTemplateChild("addendtime") as DateTimeEdit;
            EditStartTimeMonth = GetTemplateChild("editstartmonth") as DatePicker;
            EditStartTime = GetTemplateChild("editstarttime") as DateTimeEdit;
            EditEndTimeMonth = GetTemplateChild("editendmonth") as DatePicker;
            EditEndTime = GetTemplateChild("editendtime") as DateTimeEdit;
            Subject = GetTemplateChild("sub") as TextBox;
            Notes = GetTemplateChild("notes") as TextBox;
            //Location = GetTemplateChild("where") as TextBox;
            Close = GetTemplateChild("close") as Button;
            Save = GetTemplateChild("save") as Button;
            //Reminder = GetTemplateChild("reminder") as ComboBox;
            Delete = GetTemplateChild("delete") as Button;
            //ShowMorePanel = GetTemplateChild("showmorepanel") as StackPanel;
            Scroll = GetTemplateChild("scroll") as ScrollViewer;
            Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            Visibility = Visibility.Collapsed;
            //AddReminder = GetTemplateChild("addreminder") as ComboBox;
            AppType = GetTemplateChild("apptype") as ComboBox;
            Doctor = GetTemplateChild("doctor") as ComboBox;
            ClientText = GetTemplateChild("client") as SfTextBoxExt;
            ClearClient = GetTemplateChild("clearClient") as Button;
            ViewClient = GetTemplateChild("viewClient") as Button;
            //Client2 = GetTemplateChild("client2") as AutoComplete;
            Close.Click += Close_Click;
            Save.Click += Save_Click;
            Delete.Click += Delete_Click;
            ClearClient.Click += ClearClient_Click;
            ViewClient.Click += ViewClient_Click;
            //AddReminder.ItemsSource = Reminder.ItemsSource = Enum.GetValues(typeof(ReminderTimeType));
            //AddReminder.SelectedIndex = Reminder.SelectedIndex = 0;
            //SchedulerControl.AddDataContext.Appointment = new Appointment();
            DataContext = SchedulerControl.AddDataContext;
            Visibility = Visibility.Collapsed;

            ClientText.SelectedItemChanged += Client_SelectedItemChanged;

            base.OnApplyTemplate();
        }

        private void ClearClient_Click(object sender, RoutedEventArgs e)
        {
            ClientText.SelectedItem = null;
            SchedulerControl.SelectedClient = null;
            ClientText.Focus();
        }
        private void ViewClient_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Application.Current.MainWindow;
            var mainViewModel = parentWindow.DataContext as MainViewModel;
            mainViewModel.SetMainControl(ViewModel.Menu.ItemClient, ClientText.SelectedItem, true);
        }

        private void Client_SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClientText.IsEnabled = e.NewValue == null;
            ClearClient.Visibility = ViewClient.Visibility = e.NewValue == null ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion

        #region Events

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            SchedulerControl.Schedule.IsHitTestVisible = true;
            if (SchedulerControl.SelectedAppointment != null)
            {
                SchedulerControl.Schedule.Appointments.Remove(SchedulerControl.SelectedAppointment);
            }

        }

        void Save_Click(object sender, RoutedEventArgs e)
        {
            SchedulerControl.SelectedClient = null;

            Doctor.IsEnabled = true;
            
            Visibility = Visibility.Collapsed;
            SchedulerControl.Schedule.IsHitTestVisible = true;
            Appointment appointment;
            if (SchedulerControl.SelectedAppointment == null)
            {
                appointment = new Appointment();
                DateTime date = (DateTime)AddStartTime.DateTime;
                appointment.StartTime = ((DateTime)AddStartTimeMonth.SelectedDate).Date.Add(new TimeSpan(date.Hour, date.Minute, date.Second));
                DateTime date1 = (DateTime)AddEndTime.DateTime;
                //appointment.ReminderTime = (ReminderTimeType)AddReminder.SelectedItem;
                appointment.EndTime = ((DateTime)AddEndTimeMonth.SelectedDate).Date.Add(new TimeSpan(date1.Hour, date1.Minute, date1.Second));
            }
            else
            {
                appointment = SchedulerControl.SelectedAppointment;
                DateTime date = (DateTime)EditStartTime.DateTime;
                //appointment.ReminderTime = (ReminderTimeType)Reminder.SelectedItem;
                appointment.StartTime = ((DateTime)EditStartTimeMonth.SelectedDate).Date.Add(new TimeSpan(date.Hour, date.Minute, date.Second));
                DateTime date1 = (DateTime)EditEndTime.DateTime;
                appointment.EndTime = ((DateTime)EditEndTimeMonth.SelectedDate).Date.Add(new TimeSpan(date1.Hour, date1.Minute, date1.Second));
            }
            if (Doctor.SelectedItem != null)
            {
                //appointment.ResourceCollection = new ObservableCollection<object>();
                appointment.ResourceCollection = new ObservableCollection<object> { new Resource() { TypeName = Scheduler.RESOURCE, ResourceName = (Doctor.SelectedItem as User).UserName} };
            }
            Client client;
            if (ClientText.SelectedItem != null)
            {
                client = ClientText.SelectedItem as Client;
            } else
            {
                // TODO: Create Client for later
                client = new Client { Name = ClientText.Text, Code = Name.Replace(" ", "")};
                //Scheduler.AddClients(client);
            }
            appointment.Client = client;
            appointment.Subject = Subject.Text;
            appointment.Notes = Notes.Text;
            //appointment.Location = Location.Text;
            if (AppType.SelectedItem != null)
            {
                appointment.AppointmentType = (Appointment.AppointmentTypes)AppType.SelectedItem;
            }
            else
            {
                appointment.AppointmentType = Appointment.AppointmentTypes.Office;
            }

            //SetImageForAppointment(appointment);
            if (SchedulerControl.SelectedAppointment == null)
            {
                SchedulerControl.Schedule.Appointments.Add(appointment);
            }

        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            SchedulerControl.Schedule.IsHitTestVisible = true;
        }

        #endregion
    }

    #endregion

    #region Binding Class

    public class BindingClass
    {
        public DateTime? CurrentSelectedDate { get; set; }

        public object Appointment { get; set; }
        public List<object> SelectedResource { get; set; }
    }

    #endregion
}
