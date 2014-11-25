using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using ChupaChupa_Model.Entities;


namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {
        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                NotifyPropertyChanged("CurrentUser");
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                NotifyPropertyChanged("IsLoggedIn");
            }
        }

        private string _logMessage;
        public string LogMessage
        {
            get { return _logMessage; }
            set
            {
                _logMessage = value;
                NotifyPropertyChanged("LogMessage");
            }
        }
        

        private string _accountLoginText;
        public string AccountLoginText
        {
            get { return _accountLoginText; }
            set
            {
                _accountLoginText = value;
                NotifyPropertyChanged("AccountLoginText");
            }
        }

        private string _accountPasswordText;
        public string AccountPasswordText
        {
            get { return _accountPasswordText; }
            set
            {
                _accountPasswordText = value;
                NotifyPropertyChanged("AccountPasswordText");
            }
        }



        public ICommand LogUserCmd { get; set; }

    }

}