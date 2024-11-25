using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp
{
    public partial class Events : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _start_time;
        public DateTime StartTime
        {
            get => _start_time;
            set
            {
                if (_start_time != value)
                {
                    _start_time = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _end_time;
        public DateTime EndTime
        {
            get => _end_time;
            set
            {
                if (_end_time != value)
                {
                    _end_time = value;
                    OnPropertyChanged();
                }
            }
        }

        // 同样可以扩展其他需要通知的属性
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
