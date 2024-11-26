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

        

        public DateDetailsWindow(DateTime selectedDate, string holiday, ObservableCollection<events> events)
        {
            InitializeComponent();
            SelectedDate = selectedDate;
            Holiday = holiday;
            

            // Display the selected date
            SelectedDateTextBlock.Text = SelectedDate.ToString("D");

            // Display holiday if available
            HolidayTextBlock.Text = string.IsNullOrEmpty(Holiday) ? "No holiday" : Holiday;

            

            // Populate the list of events
            if (events != null)
            {
                foreach (var evt in events)
                {
                    EventsListBox.Items.Add($"Title: {evt.Title}, Start at:{evt.start_time.ToShortTimeString()} .");
                }
            }
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Event functionality is not yet implemented.",
                            "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
