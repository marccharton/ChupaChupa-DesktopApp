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
using Chupachupa_DesktopApp.PublicService;
using Chupachupa_DesktopApp.Tools;
using Chupachupa_DesktopApp.PrivateService;


namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {
        private string accountSerializePath = @"account.xml";

        private string _currentUserName;
        public string CurrentUserName
        {
            get { return _currentUserName; }
            set
            {
                _currentUserName = value;
                NotifyPropertyChanged("CurrentUserName");
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

        private string _connectionErrorText;
        public string ConnectionErrorText
        {
            get { return _connectionErrorText; }
            set
            {
                _connectionErrorText = value;
                NotifyPropertyChanged("ConnectionErrorText");
            }
        }

        private bool _isFlyoutCreateAccountOppenned;
        public bool IsFlyoutCreateAccountOppenned
        {
            get { return _isFlyoutCreateAccountOppenned; }
            set
            {
                _isFlyoutCreateAccountOppenned = value;
                NotifyPropertyChanged("IsFlyoutCreateAccountOppenned");
            }
        }

        private string _createAccountLoginText;
        public string CreateAccountLoginText
        {
            get { return _createAccountLoginText; }
            set
            {
                _createAccountLoginText = value;
                NotifyPropertyChanged("CreateAccountLoginText");
            }
        }

        private string _createAccountPasswordText;
        public string CreateAccountPasswordText
        {
            get { return _createAccountPasswordText; }
            set
            {
                _createAccountPasswordText = value;
                NotifyPropertyChanged("CreateAccountPasswordText");
            }
        }

        private string _createAccountPasswordCheckText;
        public string CreateAccountPasswordCheckText
        {
            get { return _createAccountPasswordCheckText; }
            set
            {
                _createAccountPasswordCheckText = value;
                NotifyPropertyChanged("CreateAccountPasswordCheckText");
            }
        }

        private string _createAccountMessage;
        public string CreateAccountMessage
        {
            get { return _createAccountMessage; }
            set
            {
                _createAccountMessage = value;
                NotifyPropertyChanged("CreateAccountMessage");
            }
        }

        public ICommand LogUserCmd { get; set; }
        public ICommand ExitCmd { get; set; }

        public ICommand CreateAccountCmd { get; set; }
        public ICommand CreateAccountCmdValidate { get; set; }
        

        // ******** Log (in/out) user *********
        private void LogUser()
        {
            LogUserCmd = new CommandWithParameter(new Action<object>(async (o) =>
            {
                // TODO : Remettre pour passwordBox
                //if(o != null)
                //{
                //    var passwordBox = o as PasswordBox;
                //    AccountPasswordText = passwordBox.Password;
                //}
                if (!IsLoggedIn)
                {
                    if (AccountLoginText != null && AccountPasswordText != null && 
                        AccountLoginText != "" && AccountPasswordText != "")
                    {
                        IsProgressRingActive = true;
                        try
                        {
                            _serveur = new ServiceContractClient();
                            _serveur.ClientCredentials.UserName.UserName = AccountLoginText;
                            _serveur.ClientCredentials.UserName.Password = AccountPasswordText;
                            await _serveur.authenticateAsync(AccountLoginText, AccountPasswordText);

                            CurrentUserName = AccountLoginText;
                            IsProgressRingActive = false;
                            SelectedTabIndex = (int)ToolsBox.TabOnApplicationInit;
                            IsLoggedIn = true;
                            LogMessage = "LOG OUT";
                            ConnectionErrorText = "";
                            LoadCategories();
                        }
                        catch(Exception e)
                        {
                            if (e.InnerException == null)
                                ConnectionErrorText = e.Message;
                            else
                                ConnectionErrorText = e.InnerException.Message;
                            IsProgressRingActive = false;
                        }
                    }
                }
                else
                {
                    _serveur.disconnect();
                    IsLoggedIn = false;
                    LogMessage = "LOG IN";
                    SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_ACCOUNT;
                    ReinitiateData();
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


        private void ManageUser()
        {
            CreateAccountCmd = new Command(new Action(() =>
            {
                IsFlyoutCreateAccountOppenned = true;
            }));

            CreateAccountCmdValidate = new Command(new Action(() =>
            {
                if (CreateAccountPasswordText != CreateAccountPasswordCheckText)
                {
                    CreateAccountMessage = "Passwords doesn't match";
                    return;
                }
                else
                {
                    CreateAccountMessage = "";
                    _pubServeur = new PublicServiceContractClient();

                    if (!string.IsNullOrEmpty(CreateAccountPasswordText) &&
                        !string.IsNullOrEmpty(CreateAccountLoginText))
                    {
                        _pubServeur.createAccount(CreateAccountLoginText, CreateAccountPasswordText);
                        AccountLoginText = CreateAccountLoginText;
                        AccountPasswordText = CreateAccountPasswordText;
                    }
                }
                IsFlyoutCreateAccountOppenned = false;
            }));
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
            if (IsLoggedIn)
            {
                try
                {
                    _serveur.disconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        

    }

}