using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Serialization;
using Chupachupa_DesktopApp.Class;
using ChupaChupa_Model.Entities;


namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {
        private string accountSerializePath = @"account.xml";

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
        public ICommand ExitCmd { get; set; }

        // ******** Log (in/out) user *********
        private void LogUser()
        {
            LogUserCmd = new CommandWithParameter(new Action<object>(async (o) =>
            {
                if(o != null)
                {
                    var passwordBox = o as PasswordBox;
                    AccountPasswordText = passwordBox.Password;
                }
                if (!IsLoggedIn)
                {
                    if (AccountLoginText != "" && AccountPasswordText != "")
                    {
                        IsLoggedIn = true;
                        LogMessage = "LOG OUT";
                        IsProgressRingActive = true;

                        // TODO : Connection à la base avec les données du user
                        await Task.Delay(1000);
                        CurrentUser = new User() { LoginMail = AccountLoginText, Password = AccountPasswordText };
                        
                        IsProgressRingActive = false;
                        SelectedTabIndex = 3;
                    }
                }
                else
                {
                    // TODO : Deconnection du user
                    CurrentUser = null;
                    IsLoggedIn = false;
                    LogMessage = "LOG IN";
                    SelectedTabIndex = 0;
                }
            }));

            if (File.Exists(this.accountSerializePath))
            {
                if (Unserialize() == true)
                    LogUserCmd.Execute(null);
                else
                {
                    MessageBox.Show("Ca t'amuses de mettre de la merde dans les credentials ?");
                }
            }
        }

        // ******** Serialize Credentials *********
        private bool Serialize()
        {
            List<String> usernameCredantials = new List<string>(){AccountLoginText, AccountPasswordText};

            if (File.Exists(this.accountSerializePath))
                File.Delete(this.accountSerializePath);
            Serializer.Serialize(usernameCredantials, this.accountSerializePath, FileMode.OpenOrCreate, typeof(List<string>));
            return true;
        }

        // ******** Unserialize Credentials *********
        private bool Unserialize()
        {
            List<String> userCredantials = null;
            if (File.Exists(this.accountSerializePath))
                using (var fs = new FileStream(this.accountSerializePath, FileMode.Open))
                {
                    try
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(List<string>));
                        userCredantials = xml.Deserialize(fs) as List<string>;
                    }
                    catch
                    {
                        return false;
                    }
                }
            if (userCredantials != null)
            {
                if (userCredantials.Count != 2)
                    return false;
                AccountLoginText = userCredantials[0];
                AccountPasswordText = userCredantials[1];
            }

            return true;
        }

        // ******** Destructor *********
        ~ViewModelDataSource()
        {
            if (IsLoggedIn)
                Serialize();
            else
            {
                if (File.Exists(this.accountSerializePath))
                    File.Delete(this.accountSerializePath);
            }
        }
        

    }

}