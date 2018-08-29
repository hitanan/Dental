﻿using Syncfusion.UI.Xaml.Schedule;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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

        internal Appointment SelectedAppointment;
        internal BindingClass AddDataContext = null;
        Appointment copiedAppointment;
        DateTime CurrentSelectedDate;
        //Reminder reminder;

         private string _scheduleType;
         public string ScheduleType { get => _scheduleType; set { _scheduleType = value; OnPropertyChanged("ScheduleType"); } }

        #endregion

        #region Properties

        public ScheduleAppointmentCollection AppCollection
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public Scheduler()
        {
            InitializeComponent();
            customeEditor.DataContext = this;
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
            CultureInfo culture = CultureInfo.CreateSpecificCulture("vi");
            Thread.CurrentThread.CurrentCulture = culture;

            ScheduleType = "Week";



            AppCollection = new ScheduleAppointmentCollection();
            DateTime currentdate = DateTime.Now.Date;
            if (currentdate.DayOfWeek == System.DayOfWeek.Friday || currentdate.DayOfWeek == System.DayOfWeek.Saturday)
                currentdate = currentdate.SubtractDays(3);
            else if (currentdate.DayOfWeek == System.DayOfWeek.Sunday || currentdate.DayOfWeek == System.DayOfWeek.Monday)
                currentdate = currentdate.AddDays(3);
            AppCollection.Add(new Appointment()
            {
                AppointmentImageURI = new BitmapImage(new Uri("pack://application:,,,/Assets/Hospital.png", UriKind.RelativeOrAbsolute)),
                AppointmentType = Appointment.AppointmentTypes.Health,
                Status = Schedule.AppointmentStatusCollection[0],
                StartTime = currentdate.AddHours(12),
                AppointmentTime = currentdate.AddHours(12).ToString("hh:mm tt"),
                EndTime = currentdate.AddHours(15),
                Subject = "Checkup",
                Location = "Chennai",
                AppointmentBackground = new SolidColorBrush(Color.FromArgb(255, 236, 12, 12))
            }
           );
            currentdate = currentdate.AddHours(4);
            AppCollection.Add(new Appointment()
            {
                AppointmentImageURI = new BitmapImage(new Uri("pack://application:,,,/Assets/Cake.png")),
                AppointmentType = Appointment.AppointmentTypes.Family,
                Status = Schedule.AppointmentStatusCollection[0],
                StartTime = currentdate.Date.SubtractDays(2).AddHours(1),
                AppointmentTime = currentdate.Date.SubtractDays(2).AddHours(1).ToString("hh:mm tt"),
                EndTime = currentdate.Date.SubtractDays(2).AddHours(4),
                Subject = "My B'day",
                AppointmentBackground = new SolidColorBrush(Color.FromArgb(255, 180, 31, 125))
            }
            );
            AppCollection.Add(new Appointment()
            {
                AppointmentImageURI = new BitmapImage(new Uri("pack://application:,,,/Assets/Team.png")),
                AppointmentType = Appointment.AppointmentTypes.Office,
                Status = Schedule.AppointmentStatusCollection[0],
                StartTime = currentdate.Date.AddDays(2).AddHours(9),
                AppointmentTime = currentdate.Date.AddDays(2).AddHours(9).ToString("hh:mm tt"),
                EndTime = currentdate.Date.AddDays(2).AddHours(12),
                Subject = "Meeting",
                AppointmentBackground = new SolidColorBrush(Color.FromArgb(255, 60, 181, 75))
            }
            );


            Schedule.Appointments = AppCollection;
            DataContext = this;

            //PreviewMouseLeftButtonUp += Schedule_PreviewMouseLeftButtonDown;
            Schedule.ContextMenuOpening += Schedule_PopupMenuOpening;
            //Schedule.MouseLeave += new MouseEventHandler(Schedule_MouseLeave);
            Schedule.AppointmentEditorOpening += Schedule_AppointmentEditorOpening;
            Schedule.Loaded += Schedule_Loaded;
            //Schedule.ReminderOpening += Schedule_ReminderOpening;
        }

        #endregion

        #region Events
        void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            customeEditor.AppType.ItemsSource = Enum.GetValues(typeof(Appointment.AppointmentTypes));
            customeEditor.Doctor.ItemsSource = Enum.GetValues(typeof(Appointment.AppointmentTypes));
            customeEditor.AppType.SelectedIndex = 0;
            //Schedule.PreviewMouseLeftButtonDown += Schedule_PreviewMouseLeftButtonDown;
            //Schedule.PreviewMouseWheel += Schedule_PreviewMouseWheel;
        }


        /*
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
            RadialPopup.IsOpen = true;
            radialMenu.IsOpen = true;
            if (e.CurrentSelectedDate != null)
            {
                CurrentSelectedDate = (DateTime)e.CurrentSelectedDate;
            }

            AddDataContext = new BindingClass() { CurrentSelectedDate = e.CurrentSelectedDate, Appointment = e.Appointment };

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
        */

        void Schedule_PopupMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            if (e.CurrentSelectedDate != null)
            {
                CurrentSelectedDate = (DateTime)e.CurrentSelectedDate;
            }

            AddDataContext = new BindingClass() { CurrentSelectedDate = e.CurrentSelectedDate, Appointment = e.Appointment };
        }

        void Schedule_AppointmentEditorOpening(object sender, AppointmentEditorOpeningEventArgs e)
        {
            e.Cancel = true;
            AddDataContext = new BindingClass() { CurrentSelectedDate = e.StartTime, Appointment = e.Appointment };
            if (e.Appointment != null)
                EditAppointment();
            else
            {
                customeEditor.AppType.SelectedIndex = 0;
                AddAppointment();
            }
        }

        #region Popup Menu Click Events

        void pasteButton_Click(object sender, RoutedEventArgs e)
        {
            //RadialPopup.IsOpen = false;
            if (this.copiedAppointment != null)
            {
                Appointment app = this.copiedAppointment;
                TimeSpan appTimeDiff = app.EndTime - app.StartTime;
                Appointment appointment = new Appointment();
                appointment.Subject = app.Subject;
                appointment.Notes = app.Notes;
                appointment.Location = app.Location;
                appointment.ReadOnly = app.ReadOnly;
                appointment.AppointmentBackground = app.AppointmentBackground;
                appointment.AppointmentTime = this.CurrentSelectedDate.ToString("hh:mm tt");
                appointment.AppointmentType = app.AppointmentType;
                CustomEditor.SetImageForAppointment(appointment);
                appointment.StartTime = (DateTime)this.CurrentSelectedDate;
                appointment.EndTime = ((DateTime)this.CurrentSelectedDate).Add(appTimeDiff);
                Schedule.Appointments.Add(appointment);
            }
        }

        void copyButton_Click(object sender, RoutedEventArgs e)
        {
            copiedAppointment = (Appointment)Schedule.SelectedAppointment;
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Schedule.SelectedAppointment != null)
                Schedule.Appointments.Remove(Schedule.SelectedAppointment);
        }

        void editButton_Click(object sender, RoutedEventArgs e)
        {
            EditAppointment();
        }

        void addButton_Click(object sender, RoutedEventArgs e)
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
            customeEditor.EditEndTimeMonth.Visibility = Visibility.Visible;
            customeEditor.EditEndTime.Visibility = Visibility.Visible;
            customeEditor.Delete.Visibility = Visibility.Visible;
            if (AddDataContext.Appointment != null)
            {
                customeEditor.EditStartTime.DateTime = (AddDataContext.Appointment as Appointment).StartTime;
                customeEditor.EditStartTimeMonth.SelectedDate = (AddDataContext.Appointment as Appointment).StartTime;
                customeEditor.EditEndTime.DateTime = (AddDataContext.Appointment as Appointment).EndTime;
                customeEditor.EditEndTimeMonth.SelectedDate = (AddDataContext.Appointment as Appointment).EndTime;
            }
            customeEditor.Subject.Text = SelectedAppointment.Subject;
            customeEditor.Notes.Text = SelectedAppointment.Notes;
            //customeEditor.Location.Text = SelectedAppointment.Location;
        }

        private void AddAppointment()
        {
            customeEditor.Visibility = Visibility.Visible;
            Schedule.IsHitTestVisible = false;
            SelectedAppointment = null;
            //customeEditor.AddReminder.SelectedIndex = 0;
            customeEditor.AddEndTimeMonth.Visibility = Visibility.Visible;
            customeEditor.AddEndTime.Visibility = Visibility.Visible;
            customeEditor.AddStartTimeMonth.Visibility = Visibility.Visible;
            customeEditor.AddStartTime.Visibility = Visibility.Visible;
            if (AddDataContext != null)
            {
                if (AddDataContext.CurrentSelectedDate != null)
                {
                    customeEditor.AddStartTime.DateTime = AddDataContext.CurrentSelectedDate.Value;
                    customeEditor.AddStartTimeMonth.SelectedDate = AddDataContext.CurrentSelectedDate.Value;
                    customeEditor.AddEndTimeMonth.SelectedDate = AddDataContext.CurrentSelectedDate.Value.AddHours(1);
                    customeEditor.AddEndTime.DateTime = AddDataContext.CurrentSelectedDate.Value.AddHours(1);
                }
                else if (AddDataContext.Appointment != null)
                {
                    customeEditor.AddStartTimeMonth.SelectedDate = customeEditor.AddEndTimeMonth.SelectedDate = (AddDataContext.Appointment as Appointment).StartTime;
                    customeEditor.AddStartTime.DateTime = customeEditor.AddEndTime.DateTime = (AddDataContext.Appointment as Appointment).StartTime.AddHours(1);
                }
            }
            //customeEditor.AddReminder.Visibility = Visibility.Visible;
            //customeEditor.Reminder.Visibility = Visibility.Collapsed;
            customeEditor.EditStartTimeMonth.Visibility = Visibility.Collapsed;
            customeEditor.EditStartTime.Visibility = Visibility.Collapsed;
            customeEditor.EditEndTimeMonth.Visibility = Visibility.Collapsed;
            customeEditor.EditEndTime.Visibility = Visibility.Collapsed;
            customeEditor.Delete.Visibility = Visibility.Collapsed;
            customeEditor.Subject.Text = string.Empty;
            customeEditor.Notes.Text = string.Empty;
            //customeEditor.Location.Text = string.Empty;
            customeEditor.DataContext = AddDataContext;
        }
        #endregion

        #region OnPropertyChanged
        void OnPropertyChanged(String prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }

    #endregion

    #region Appointment Class

    public class Appointment : ScheduleAppointment, INotifyPropertyChanged
    {
        #region public properties
        private ImageSource _imageuri;
        public ImageSource AppointmentImageURI
        {
            get { return _imageuri; }
            set
            {
                _imageuri = value;
                OnPropertyChanged("AppointmentImageURI");
            }
        }

        public string AppointmentTime { get; set; }

        public AppointmentTypes AppointmentType { get; set; }

        public enum AppointmentTypes
        {
            Office,
            Health,
            Family
        }

        private Clients _client;
        public Clients Client { get => _client; set  { _client = value; OnPropertyChanged("Client"); } }

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Clients
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Clients()
        {

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
        public Scheduler editorPage;
        public ComboBox Status;
        public ScrollViewer Scroll;
        public ComboBox AppType;
        public ComboBox Doctor;
        public TextBox Client;
        //public ComboBox Reminder;
        public Button Delete;
        //public ComboBox AddReminder;
        public ComboBox AddStatus;

        #endregion

        #region Override Methods
        public override void OnApplyTemplate()
        {
            editorPage = (Scheduler)DataContext;
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
            Client = GetTemplateChild("client") as TextBox;
            Close.Click += Close_Click;
            Save.Click += Save_Click;
            Delete.Click += Delete_Click;
            //AddReminder.ItemsSource = Reminder.ItemsSource = Enum.GetValues(typeof(ReminderTimeType));
            //AddReminder.SelectedIndex = Reminder.SelectedIndex = 0;
            DataContext = editorPage.AddDataContext;
            Visibility = Visibility.Collapsed;
            base.OnApplyTemplate();
        }
        #endregion

        #region Events

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            editorPage.Schedule.IsHitTestVisible = true;
            if (editorPage.SelectedAppointment != null)
            {
                editorPage.Schedule.Appointments.Remove(editorPage.SelectedAppointment);
            }
        }

        void Save_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            editorPage.Schedule.IsHitTestVisible = true;
            Appointment appointment;
            if (editorPage.SelectedAppointment == null)
            {
                appointment = new Appointment();
                DateTime date = (DateTime)AddStartTime.DateTime;
                appointment.StartTime = ((DateTime)AddStartTimeMonth.SelectedDate).Date.Add(new TimeSpan(date.Hour, date.Minute, date.Second));
                appointment.AppointmentTime = appointment.StartTime.ToString("hh:mm tt");
                DateTime date1 = (DateTime)AddEndTime.DateTime;
                //appointment.ReminderTime = (ReminderTimeType)AddReminder.SelectedItem;
                appointment.EndTime = ((DateTime)AddEndTimeMonth.SelectedDate).Date.Add(new TimeSpan(date1.Hour, date1.Minute, date1.Second));
                appointment.AppointmentTime = appointment.StartTime.ToString("hh:mm tt");
            }
            else
            {
                appointment = editorPage.SelectedAppointment;
                DateTime date = (DateTime)EditStartTime.DateTime;
                //appointment.ReminderTime = (ReminderTimeType)Reminder.SelectedItem;
                appointment.StartTime = ((DateTime)EditStartTimeMonth.SelectedDate).Date.Add(new TimeSpan(date.Hour, date.Minute, date.Second));
                appointment.AppointmentTime = appointment.StartTime.ToString("hh:mm tt");
                DateTime date1 = (DateTime)EditEndTime.DateTime;
                appointment.EndTime = ((DateTime)EditEndTimeMonth.SelectedDate).Date.Add(new TimeSpan(date1.Hour, date1.Minute, date1.Second));
                appointment.AppointmentTime = appointment.StartTime.ToString("hh:mm tt");
            }
            appointment.Subject = Subject.Text;
            appointment.Notes = Notes.Text;
            //appointment.Location = Location.Text;
            if (AppType.SelectedItem != null)
            {
                appointment.AppointmentType = (Appointment.AppointmentTypes)AppType.SelectedItem;
            }
            else
            {
                appointment.AppointmentType = Appointment.AppointmentTypes.Family;
            }
            SetImageForAppointment(appointment);
            if (editorPage.SelectedAppointment == null)
            {
                editorPage.Schedule.Appointments.Add(appointment);
            }
        }

        public static void SetImageForAppointment(Appointment appointment)
        {
            switch (appointment.AppointmentType)
            {
                case Appointment.AppointmentTypes.Family:
                    {
                        appointment.AppointmentImageURI = new BitmapImage(new Uri("pack://application:,,,/Assets/Cake.png"));
                        break;
                    }
                case Appointment.AppointmentTypes.Health:
                    {
                        appointment.AppointmentImageURI = new BitmapImage(new Uri("pack://application:,,,/Assets/Hospital.png"));
                        break;
                    }
                case Appointment.AppointmentTypes.Office:
                    {
                        appointment.AppointmentImageURI = new BitmapImage(new Uri("pack://application:,,,/Assets/Team.png"));
                        break;
                    }
            }
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            editorPage.Schedule.IsHitTestVisible = true;
        }

        #endregion
    }

    #endregion

    #region Binding Class

    public class BindingClass
    {
        public DateTime? CurrentSelectedDate { get; set; }

        public object Appointment { get; set; }
    }

    #endregion
}
