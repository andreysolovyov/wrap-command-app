using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperApp
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _hasInstalled;
        public bool HasInstalled
        {
            get
            {
                return _hasInstalled;
            }

            set
            {
                _hasInstalled = value;
                NotifyPropertyChanged("HasInstalled");
            }
        }
        private bool _inProgress;
        public bool InProgress 
        {
            get
            {
                return _inProgress;
            }

            set
            {
                _inProgress = value;
                NotifyPropertyChanged("InProgress");
            }
        }
        private string _LoadingIndicatorText;
        public string LoadingIndicatorText
        {
            get
            {
                return _LoadingIndicatorText;
            }

            set
            {
                _LoadingIndicatorText = value;
                NotifyPropertyChanged("LoadingIndicatorText");
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
