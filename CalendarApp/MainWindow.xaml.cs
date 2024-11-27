using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Security.Cryptography;
using CalendarApp;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.RightsManagement;

namespace CalendarApp
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>

    public class CalendarDay
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string Holiday { get; set; }
        public bool IsCurrentMonth { get; set; }
        public ICommand ShowDetailsCommand { get; set; }

        public ObservableCollection<events> Events { get; set; }

        public bool IsToday => DateTime.Today.Year == Year && DateTime.Today.Month == Month && DateTime.Today.Day == Day && IsCurrentMonth;

        public CalendarDay()
        {
            ShowDetailsCommand = new RelayCommand(OpenDateDetails);
            Events = new ObservableCollection<events>();
        }

        private void OpenDateDetails(object parameter)
        {
            var selectedDate = parameter as DateTime?;
            if (selectedDate != null)
            {
                var detailsWindow = new DateDetailsWindow(selectedDate.Value, Holiday, Events);
                detailsWindow.ShowDialog();
            }
        }
    }


    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private users _loggedInUser;
        private DateTime _currentMonth;

        public DateTime CurrentMonth
        {
            get => _currentMonth;
            set
            {
                _currentMonth = value;
                OnPropertyChanged(nameof(CurrentMonth));
                LoadEventsFromDatabase();
                GenerateCalendarDays();
            }
        }

        public ObservableCollection<CalendarDay> CalendarDays { get; set; } = new ObservableCollection<CalendarDay>();

        public ObservableCollection<events> Events { get; set; } = new ObservableCollection<events>();
        public MainViewModel(users loggedInUser=null)
        {
            if(loggedInUser != null)
            {
                _loggedInUser = loggedInUser;
            }
            CurrentMonth = DateTime.Now;
            LoadEventsFromDatabase();


        }

        public void LoadEventsFromDatabase()
        {
            Events.Clear();
            if (_loggedInUser != null)
            {
                Console.WriteLine($"Loading events for user {_loggedInUser.username}...");
                try
                {
                    using (var context = new calendadbEntities())
                    {
                        
                        var eventList = context.events.Where(e => e.user_id == _loggedInUser.id).ToList();
                        foreach (var evt in eventList)
                        {
                            Events.Add(evt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load events from database: {ex.Message}");
                }
            }
        }



        public void GenerateCalendarDays()
        {
            CalendarDays.Clear();
            DateTime firstDay = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);

            int firstDayOfWeek = (int)firstDay.DayOfWeek;

            DateTime prevMonth = firstDay.AddMonths(-1);
            int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            for (int i = firstDayOfWeek - 1; i >= 0; i--)
            {
                CalendarDays.Add(new CalendarDay
                {
                    Day = daysInPrevMonth - i,
                    IsCurrentMonth = false,
                    Holiday = "",
                    Events = new ObservableCollection<events>() // 默认空事件列表
                });
            }

            for (int i = 1; i <= daysInMonth; i++)
            {
                DateTime currentDay = new DateTime(CurrentMonth.Year, CurrentMonth.Month, i);
                var dayEvents = Events.Where(e => e.start_time.Date == currentDay).ToList();

                CalendarDays.Add(new CalendarDay
                {
                    Year = CurrentMonth.Year,
                    Month = CurrentMonth.Month,
                    Day = i,
                    IsCurrentMonth = true,
                    Holiday = GetHolidayForDate(currentDay),
                    Events = new ObservableCollection<events>(dayEvents) // 将当天的事件绑定到 CalendarDay
                });
            }

            int remainingDays = 42 - CalendarDays.Count;
            for (int i = 1; i <= remainingDays; i++)
            {
                CalendarDays.Add(new CalendarDay
                {
                    Day = i,
                    IsCurrentMonth = false,
                    Holiday = "",
                    Events = new ObservableCollection<events>()
                });
            }
        }
        private string GetHolidayForDate(DateTime date)
        {

            string filePath = @"..\..\data\holidays_2023-2026.txt";
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Path.Combine(currentDirectory, filePath);

            var holidays = new Dictionary<DateTime, string>();
            string dateStrFormat = "yyyy/M/d";

            if (!File.Exists(path))
            {
                Console.WriteLine($"File not found: {path}");
                return "";
            }
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');

                    try
                    {
                        // parse date from file
                        DateTime _dateTime = DateTime.ParseExact(values[6], dateStrFormat, CultureInfo.InvariantCulture);
                        string _fullHoliday = values[0] + " (" + values[2].Replace("\"", "") + ")";
                        // avoid duplicate holidays
                        if (!holidays.ContainsKey(_dateTime))
                        {
                            holidays.Add(_dateTime, _fullHoliday);
                        }
                        else
                        {
                            holidays[_dateTime] += ", " + _fullHoliday; // Append the new holiday name to the existing one
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Invalid date format in file: {values[6]}");
                    }



                }
            }
            //prototype of  holidays dictionary
            //var holidays = new Dictionary<DateTime, string>
            //{
            //    { new DateTime(2024, 1, 1), "New Year's Day" },
            //    { new DateTime(2024, 12, 25), "Christmas" }
            //};

            return holidays.TryGetValue(date, out string holiday) ? holiday : "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MainWindow : Window
    {
        private  MainViewModel _viewModel;

        public users LoggedInUser { get; set; }
        private users _loggedInUser;

        public MainWindow() : this(null) { }
        public MainWindow(users loggedInUser=null)
        {
            InitializeComponent();
            LoggedInUser = loggedInUser;

            _viewModel = new MainViewModel(LoggedInUser);
            DataContext = _viewModel;

            UpdateUIBasedOnLoginStatus();

            

            try
            {
                LoadAndTestDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error initializing database: {ex.Message}",
                                "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenWindow<T>() where T : Window, new()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is T existingWindow)
                {
                    existingWindow.Activate();
                    return;
                }
            }

            var newWindow = new T { Owner = this };
            newWindow.ShowDialog();
        }

        private void OpenLoginWindow_Click(object sender, RoutedEventArgs e)
{
            this.Hide();

            // open login window
            var loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();
            UpdateUIBasedOnLoginStatus();
            // if login is successful, show the main window
            if (result == true)
            {
                var loggedInUser = loginWindow.LoggedInUser;
                if (loggedInUser != null)
                {
                    // renew the logged in user
                    _loggedInUser = loggedInUser;
                    LoggedInUser = loggedInUser;
                    UpdateUIBasedOnLoginStatus();

                    // renew the view model
                    _viewModel = new MainViewModel(_loggedInUser);
                    DataContext = _viewModel;

                    // renew the welcome message
                    TextBlockH1.Text = $"Welcome, {_loggedInUser.username}!";

                    this.Show();
                }
                else
                {
                    // if login is successful but no user is returned, show the main window as guest
                    TextBlockH1.Text = "Welcome, Guest.";
                    this.Show();
                }
            }
            else
            {
                // if login is not successful, close the application
                Application.Current.Shutdown();
            }
        }


        private void OpenReigsterWindow_Click(object sender, RoutedEventArgs e) => OpenWindow<RegisterWindow>();

        private void OpenNewEventWindow_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser != null)
            {
                var newEventWindow = new NewEventWindow(_loggedInUser);
                newEventWindow.Owner = this;

                bool? result = newEventWindow.ShowDialog();
                if (result == true)
                {
                    _viewModel.LoadEventsFromDatabase();
                    _viewModel.GenerateCalendarDays();
                }
            }
            else
            {
                MessageBox.Show("You need to be logged in to create a new event.", "Not Logged In", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenDbConfigWindow_Click(object sender, RoutedEventArgs e) => OpenWindow<DbConfigWindow>();


        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentMonth = _viewModel.CurrentMonth.AddMonths(-1);
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentMonth = _viewModel.CurrentMonth.AddMonths(1);
        }

        private void LoadAndTestDatabase()
        {
            try
            {
                using (var context = DbContextFactory.CreateDbContext())
                {
                    var users = context.users.ToList();
                    foreach (var user in users)
                    {
                        Console.WriteLine($"Username: {user.username}, Email: {user.email}");
                    }
                    MessageBox.Show(this, "Database connected and data loaded successfully.",
                                    "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error connecting to database: {ex.Message}",
                                "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StackPanel_Clicked(object sender, MouseButtonEventArgs e)
        {
            // ensure the event is not already handled
            if (e.Handled) return;

            e.Handled = true; // prevent further handling
            var clickedDate = (sender as FrameworkElement)?.DataContext as CalendarDay;
            if (clickedDate != null)
            {
                var datailsWindow = new DateDetailsWindow(new DateTime(_viewModel.CurrentMonth.Year, _viewModel.CurrentMonth.Month, clickedDate.Day), clickedDate.Holiday, clickedDate.Events,_loggedInUser);
                datailsWindow.ShowDialog();
            }
        }

        private void DeleteUserInfoFile()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string userInfoFilePath = System.IO.Path.Combine(appDataPath, "CalendarApp", "user_info.json");

                if (File.Exists(userInfoFilePath))
                {
                    File.Delete(userInfoFilePath);
                    Console.WriteLine("User info file deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete user info file: {ex.Message}", "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdateUIBasedOnLoginStatus()
        {
            if (LoggedInUser != null)
            {
                TextBlockH1.Text = $"Calendar - Welcome {LoggedInUser.username}!";                
                ButtonOpenNewEventWindow.Visibility = Visibility.Visible;
                ButtonOpenDbConfigWindow.Visibility = Visibility.Collapsed;
                ButtonOpenRegisterWindow.Visibility = Visibility.Collapsed;
                ButtonOpenLoginWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextBlockH1.Text = "Calendar - Welcome, Guest.";               
                ButtonOpenDbConfigWindow.Visibility = Visibility.Visible;
                ButtonOpenRegisterWindow.Visibility = Visibility.Visible;
                ButtonOpenLoginWindow.Visibility = Visibility.Visible;
            }
        }
    }
}