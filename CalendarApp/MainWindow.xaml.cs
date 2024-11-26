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

        public bool IsToday => DateTime.Today.Year == Year && DateTime.Today.Month == Month && DateTime.Today.Day == Day && IsCurrentMonth;

        public CalendarDay()
        {
            ShowDetailsCommand = new RelayCommand(OpenDateDetails);
        }

        private void OpenDateDetails(object parameter)
        {
            var selectedDate = parameter as DateTime?;
            if (selectedDate != null)
            {
                var events = new List<string> { "Event 1", "Event 2", "Event 3" };

                var detailsWindow = new DateDetailsWindow(selectedDate.Value , Holiday, events);
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
        private DateTime _currentMonth;

        public DateTime CurrentMonth
        {
            get => _currentMonth;
            set
            {
                _currentMonth = value;
                OnPropertyChanged(nameof(CurrentMonth));
                GenerateCalendarDays();
            }
        }

        public ObservableCollection<CalendarDay> CalendarDays { get; set; } = new ObservableCollection<CalendarDay>();

        public MainViewModel()
        {
            CurrentMonth = DateTime.Now; 
        }

        private void GenerateCalendarDays()
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
                    Holiday = "" // 可选
                });
            }
            
            for (int i = 1; i <= daysInMonth; i++)
            {
                CalendarDays.Add(new CalendarDay
                {
                    Year = CurrentMonth.Year,
                    Month = CurrentMonth.Month,
                    Day = i,
                    IsCurrentMonth = true,
                    Holiday = GetHolidayForDate(new DateTime(CurrentMonth.Year, CurrentMonth.Month, i)) 
                });
            }

            
            int remainingDays = 42 - CalendarDays.Count;
            for (int i = 1; i <= remainingDays; i++)
            {
                CalendarDays.Add(new CalendarDay
                {
                    Day = i,
                    IsCurrentMonth = false,
                    Holiday = "" 
                });
            }
        }

        private string GetHolidayForDate(DateTime date)
        {
            
            var holidays = new Dictionary<DateTime, string>
        {
            { new DateTime(2024, 1, 1), "New Year's Day" },
            { new DateTime(2024, 12, 25), "Christmas" }
        };

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
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            UserPartial currentUser = Application.Current.Properties["CurrentUser"] as UserPartial;

            if (currentUser == null) { 
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
                return;
            }

            InitializeComponent();
            WelcomeMessage.Text = $"Welcome, {currentUser.Name}";
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

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

        private void OpenLoginWindow_Click(object sender, RoutedEventArgs e) => OpenWindow<LoginWindow>();

        private void OpenReigsterWindow_Click(object sender, RoutedEventArgs e) => OpenWindow<RegisterWindow>();

        private void OpenNewEventWindow_Click(object sender, RoutedEventArgs e) => OpenWindow<NewEventWindow>();

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
                var datailsWindow = new DateDetailsWindow(new DateTime(_viewModel.CurrentMonth.Year, _viewModel.CurrentMonth.Month, clickedDate.Day), clickedDate.Holiday);
                datailsWindow.ShowDialog();
            }
        }


    }
}