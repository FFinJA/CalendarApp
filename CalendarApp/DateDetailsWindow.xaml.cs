using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CalendarApp
{
    /// <summary>
    /// DateDetailsWindow.xaml 
    /// </summary>
    public partial class DateDetailsWindow : Window
    {
        public DateTime SelectedDate { get; }
        public string Holiday { get; }

        private users _loggedInUser;


        public DateDetailsWindow(DateTime selectedDate, string holiday, ObservableCollection<events> events, users loggedInUser=null)
        {
            InitializeComponent();
            SelectedDate = selectedDate;
            Holiday = holiday;
            _loggedInUser = loggedInUser;

            // Display the selected date
            SelectedDateTextBlock.Text = SelectedDate.ToString("D");

            // Display holiday if available
            HolidayTextBlock.Text = string.IsNullOrEmpty(Holiday) ? "No holiday" : Holiday;

            

            // Populate the list of events
            if (events != null)
            {
                //foreach (var evt in events)
                //{
                //    EventsListBox.Items.Add($"Title: {evt.Title}, Start at:{evt.start_time.ToShortTimeString()} .");
                //}
                EventsListBox.ItemsSource = events;
            }

            if (_loggedInUser == null)
            {
                AddEventButton.Visibility = Visibility.Collapsed; //hide the button
            }
            else
            {
                AddEventButton.Visibility = Visibility.Visible; 
            }
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            var newEventWindow = new NewEventWindow(_loggedInUser, SelectedDate);
            if (newEventWindow.ShowDialog() == true)
            {
                //refresh the events list
                LoadEventsForSelectedDate();
            }
            }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExportToIcsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is events selectedEvent)
            {
                try
                {
                    // Generate .ics file content
                    StringBuilder icsContent = new StringBuilder();
                    icsContent.AppendLine("BEGIN:VCALENDAR");
                    icsContent.AppendLine("VERSION:2.0");
                    icsContent.AppendLine("PRODID:-//CalendarApp//EN");
                    icsContent.AppendLine("BEGIN:VEVENT");
                    icsContent.AppendLine($"UID:{Guid.NewGuid()}");
                    icsContent.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
                    icsContent.AppendLine($"DTSTART:{selectedEvent.start_time:yyyyMMddTHHmmssZ}");
                    icsContent.AppendLine($"DTEND:{selectedEvent.end_time:yyyyMMddTHHmmssZ}");
                    icsContent.AppendLine($"SUMMARY:{selectedEvent.Title}");
                    icsContent.AppendLine($"DESCRIPTION:{selectedEvent.Description}");
                    icsContent.AppendLine("END:VEVENT");
                    icsContent.AppendLine("END:VCALENDAR");

                    // Set the file path and save the .ics file
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string fileName = $"{selectedEvent.Title.Replace(" ", "_")}_{selectedEvent.start_time:yyyyMMdd}.ics";
                    string filePath = System.IO.Path.Combine(desktopPath, fileName);

                    System.IO.File.WriteAllText(filePath, icsContent.ToString());
                    MessageBox.Show($"Event exported successfully to {filePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export event: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadEventsForSelectedDate()
        {
            // clear current events
            EventsListBox.Items.Clear();

            try
            {
                using (var context = new calendadbEntities())
                {
                    // select events for the selected date
                    var eventList = context.events
                        .Where(e => e.EventDate == SelectedDate.Date && e.UserId == _loggedInUser.id)
                        .ToList();

                    // add events to the list box
                    foreach (var evt in eventList)
                    {
                        EventsListBox.Items.Add($"Title: {evt.Title}, Start at: {evt.StartTime.ToShortTimeString()}.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load events for selected date: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
