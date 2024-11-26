using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CalendarApp;

namespace CalendarApp
{
    /// <summary>
    /// NewEventWindow.xaml 
    /// </summary>
    public partial class NewEventWindow : Window
    {
        public events NewEvent { get; set; }

        public NewEventWindow()
        {
            InitializeComponent();
            var converter = new TimeSpanToDateTimeConverter();

            NewEvent = new events
            {
                UserId = 1, // Example user ID, replace with actual user
                CreatedAt = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(1)
            };

            DataContext = NewEvent;
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NewEvent.Title))
            {
                MessageBox.Show("Event title cannot be empty.");
                return;
            }

            if (NewEvent.EventDate == null)
            {
                MessageBox.Show("Please select a date for the event.");
                return;
            }

            // Time assignment
            if (NewEvent.IsWholeDay)
            {
                NewEvent.StartTime = NewEvent.EventDate.Value.Date;
                NewEvent.EndTime = NewEvent.EventDate.Value.Date.AddDays(1).AddTicks(-1);
            }
            else
            {
                if (NewEvent.StartTimeOnly == null || NewEvent.EndTimeOnly == null)
                {
                    MessageBox.Show("Please specify start and end times.");
                    return;
                }

                NewEvent.StartTime = NewEvent.EventDate.Value.Date + NewEvent.StartTimeOnly.Value;
                NewEvent.EndTime = NewEvent.EventDate.Value.Date + NewEvent.EndTimeOnly.Value;

                if (NewEvent.EndTime <= NewEvent.StartTime)
                {
                    MessageBox.Show("End time must be later than start time.");
                    return;
                }
            }

            //if isRecurring
            if (NewEvent.IsRecurring)
            {
                if (NewEvent.SelectedRecurrence == null)
                {
                    MessageBox.Show("Please select a recurrence pattern.");
                    return;
                }

                int recurrenceId;
                switch(NewEvent.SelectedRecurrence)
                {
                    case "Daily":
                        recurrenceId = 2;
                        break;
                    case "Weekly":
                        recurrenceId = 3;
                        break;
                    case "Monthly":
                        recurrenceId = 4;
                        break;
                    case "Yearly":
                        recurrenceId = 5;
                        break;
                    case "Working Day":
                        recurrenceId = 6;
                        break;
                    case "Just Once":
                        recurrenceId = 1;
                        break;
                    default:
                        recurrenceId = 1;
                        break;
                }

                NewEvent.RecurrenceId = recurrenceId;
            }
            else
            {
                NewEvent.RecurrenceId = null;
            }

            if (!string.IsNullOrWhiteSpace(NewEvent.CategoryName))
            {
                int categoryId; // Example category ID, replace with actual category
                switch (NewEvent.CategoryName)
                {
                    case "Work":
                        categoryId = 2;
                        break;
                    case "Home":
                        categoryId = 3;
                        break;
                    case "Education":
                        categoryId = 4;
                        break;
                    case "Health":
                        categoryId = 12;
                        break;
                    case "Personal":
                        categoryId = 13;
                        break;
                    case "Other":
                        categoryId = 1;
                        break;
                    default:
                        categoryId = 1;
                        break;
                }
                NewEvent.CategoryId = categoryId;
            }
            else
            {
                NewEvent.CategoryId = null;
            }


            // Notification
            if (NewEvent.AddNotification)
            {                
                int minutesBefore = 0;
                switch (NewEvent.SelectedNotificationTime)
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
                NewEvent.NotifyTimeBefore = minutesBefore;
                NewEvent.NotifyStatus = "Pending";
            }
            else
            {
                NewEvent.NotifyTimeBefore = null;
                NewEvent.NotifyStatus = null;
            }

            // Save to database
            try
            {
                using (var context = new calendadbEntities())
                {
                    context.events.Add(NewEvent);
                    context.SaveChanges();
                }

                MessageBox.Show("Event added successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save event to database: {ex.Message}");
            }
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
            NewEvent.Title = string.Empty;
            NewEvent.Description = string.Empty;
            NewEvent.EventDate = null;
            NewEvent.IsWholeDay = false;
            NewEvent.StartTimeOnly = null;
            NewEvent.EndTimeOnly = null;
            NewEvent.AddNotification = false;
            NewEvent.SelectedNotificationTime = null;
        }
    }
}
