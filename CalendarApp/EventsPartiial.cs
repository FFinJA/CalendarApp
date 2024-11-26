using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp
{
    public partial class events : INotifyPropertyChanged
    //partial clas to prevent overwriting the entity class from overwriting
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        // CallerMemberNameAttribute is used to get the name of the calling property
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // PropertyChanged is an event that is raised when a property is changed
        }

        // encapsulation of id
        public int Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        // of user_id
        public int UserId
        {
            get => user_id;
            set
            {
                if (user_id != value)
                {
                    user_id = value;
                    OnPropertyChanged();
                }
            }
        }

        // of recurrence_id
        public int? RecurrenceId
        {
            get => recurrence_id;
            set
            {
                if (recurrence_id != value)
                {
                    recurrence_id = value;
                    OnPropertyChanged();
                }
            }
        }

        // of category_id
        public int? CategoryId
        {
            get => category_id;
            set
            {
                if (category_id != value)
                {
                    category_id = value;
                    OnPropertyChanged();
                }
            }
        }

        // of title
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        // of description
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }

        // of start_time
        public DateTime StartTime
        {
            get => start_time;
            set
            {
                if (start_time != value)
                {
                    start_time = value;
                    OnPropertyChanged();
                }
            }
        }

        // of end_time
        public DateTime EndTime
        {
            get => end_time;
            set
            {
                if (end_time != value)
                {
                    end_time = value;
                    OnPropertyChanged();
                }
            }
        }

        // of notify_time_before
        public int? NotifyTimeBefore
        {
            get => notify_time_before;
            set
            {
                if (notify_time_before != value)
                {
                    notify_time_before = value;
                    OnPropertyChanged();
                }
            }
        }

        // of notify_status
        public string NotifyStatus
        {
            get => notify_status;
            set
            {
                if (notify_status != value)
                {
                    notify_status = value;
                    OnPropertyChanged();
                }
            }
        }

        // of created_at
        public DateTime? CreatedAt
        {
            get => created_at;
            set
            {
                if (created_at != value)
                {
                    created_at = value;
                    OnPropertyChanged();
                }
            }
        }

        //additional properties for NewEventWindow.xaml.cs
        private bool _isRecurring;
        public bool IsRecurring
        {
            get => _isRecurring;
            set
            {
                if (_isRecurring != value)
                {
                    _isRecurring = value;
                    OnPropertyChanged();
                }
            }
        }

        //additional properties for NewEventWindow.xaml.cs
        private string _selectedRecurrence;
        public string SelectedRecurrence
        {
            get => _selectedRecurrence;
            set
            {
                if (_selectedRecurrence != value)
                {
                    _selectedRecurrence = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                if (_categoryName != value)
                {
                    _categoryName = value;
                    OnPropertyChanged();
                }
            }
        }



        // additional properties for NewEventWindow.xaml.cs
        private bool _isWholeDay;
        public bool IsWholeDay
        {
            get => _isWholeDay;
            set
            {
                if (_isWholeDay != value)
                {
                    _isWholeDay = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _addNotification;
        public bool AddNotification
        {
            get => _addNotification;
            set
            {
                if (_addNotification != value)
                {
                    _addNotification = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedNotificationTime;
        public string SelectedNotificationTime
        {
            get => _selectedNotificationTime;
            set
            {
                if (_selectedNotificationTime != value)
                {
                    _selectedNotificationTime = value;
                    OnPropertyChanged();
                }
            }
        }

        // additional properties for NewEventWindow.xaml.cs (timepicker)
        private TimeSpan? _startTimeOnly;
        public TimeSpan? StartTimeOnly
        {
            get => _startTimeOnly;
            set
            {
                if (_startTimeOnly != value)
                {
                    _startTimeOnly = value;
                    OnPropertyChanged();
                }
            }
        }
        // additional properties for NewEventWindow.xaml.cs (timepicker)
        private TimeSpan? _endTimeOnly;
        public TimeSpan? EndTimeOnly
        {
            get => _endTimeOnly;
            set
            {
                if (_endTimeOnly != value)
                {
                    _endTimeOnly = value;
                    OnPropertyChanged();
                }
            }
        }

        //additional properties for NewEventWindow.xaml.cs (datepicker)
        private DateTime? _eventDate;
        public DateTime? EventDate
        {
            get => _eventDate;
            set
            {
                if (_eventDate != value)
                {
                    _eventDate = value;
                    OnPropertyChanged();
                }
            }
        }

        
        
    }
}


