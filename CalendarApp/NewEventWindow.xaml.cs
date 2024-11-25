using PdfSharp.Drawing.BarCodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;

namespace CalendarApp
{
    /// <summary>
    /// NewEventWindow.xaml 
    /// </summary>
    public partial class NewEventWindow : Window, INotifyPropertyChanged
    {
        public events NewEvent { get; set; }
        public DateTime? EventDate
        {
            get
            {
                // check if start_time is not default value
                return NewEvent.start_time != DateTime.MinValue
                    ? NewEvent.start_time.Date
                    : (DateTime?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    NewEvent.start_time = value.Value.Date;
                }
                else
                {
                    NewEvent.start_time = DateTime.MinValue;
                }
                RaisePropertyChanged();
            }
        }
        public NewEventWindow()
        {
            InitializeComponent();

            NewEvent = new events
            {
                user_id = 1, // Example user ID, replace with actual user
                created_at = DateTime.Now,
                start_time = DateTime.MinValue
            };
            DataContext = NewEvent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Properties bound to XAML
        public new string Title
        {
            get => NewEvent.title;
            set => NewEvent.title = value;
        }

        public string Description
        {
            get => NewEvent.description;
            set => NewEvent.description = value;
        }

        

        public bool IsWholeDay { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool AddNotification { get; set; }

        public string SelectedNotificationTime { get; set; }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NewEvent.title))
            {
                MessageBox.Show("Event title cannot be empty.");
                return;
            }

            if (EventDate == null)
            {
                MessageBox.Show("Please select a date for the event.");
                return;
            }

            // Time assignment
            if (IsWholeDay)
            {
                NewEvent.start_time = EventDate.Value.Date;
                NewEvent.end_time = EventDate.Value.Date.AddDays(1).AddTicks(-1);
            }
            else
            {
                if (StartTime == null || EndTime == null)
                {
                    MessageBox.Show("Please specify start and end times.");
                    return;
                }

                NewEvent.start_time = EventDate.Value.Date + StartTime.Value;
                NewEvent.end_time = EventDate.Value.Date + EndTime.Value;

                if (NewEvent.end_time <= NewEvent.start_time)
                {
                    MessageBox.Show("End time must be later than start time.");
                    return;
                }
            }

            // Notification
            if (AddNotification)
            {
                int minutesBefore = 0;
                switch (SelectedNotificationTime)
                {
                    case "5 minutes":
                        minutesBefore = 5;
                        break;
                    case "10 minutes":
                        minutesBefore = 10;
                        break;
                    case "15 minutes":
                        minutesBefore = 15;
                        break;
                    case "30 minutes":
                        minutesBefore = 30;
                        break;
                    case "1 hour":
                        minutesBefore = 60;
                        break;
                    default:
                        minutesBefore = 0;
                        break;
                }
                NewEvent.notify_time_before = minutesBefore;
                NewEvent.notify_status = "Pending";
            }
            else
            {
                NewEvent.notify_time_before = null;
                NewEvent.notify_status = null;
            }

            // TODO: Save to database
            MessageBox.Show("Event added successfully!");
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ResetEventForm();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow == null)
            {
                mainWindow = new MainWindow();
            }
            mainWindow.Show();
            ResetEventForm();
            this.Close();
        }

        private void ResetEventForm()
        {
            Title = string.Empty;
            Description = string.Empty;
            EventDate = null;
            IsWholeDay = false;
            StartTime = null;
            EndTime = null;
            AddNotification = false;
            SelectedNotificationTime = null;
        }
    }
}