using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using Chupachupa_DesktopApp.PrivateService;


namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {
        private RssChannel _selectedChannel;
        public RssChannel SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                _selectedChannel = value;
                NotifyPropertyChanged("SelectedChannel");
            }
        }

        private RssChannel _currentChannel;
        public RssChannel CurrentChannel
        {
            get { return _currentChannel; }
            set
            {
                _currentChannel = value;
                NotifyPropertyChanged("CurrentChannel");
            }
        }

        private IList<RssChannel> _channelsList;
        public IList<RssChannel> ChannelsList
        {
            get { return _channelsList; }
            set
            {
                _channelsList = value;
                NotifyPropertyChanged("ChannelsList");
            }
        }

        private bool _isFlyoutAddChannelOpenned;
        public bool IsFlyoutAddChannelOpenned
        {
            get { return _isFlyoutAddChannelOpenned; }
            set
            {
                _isFlyoutAddChannelOpenned = value;
                NotifyPropertyChanged("IsFlyoutAddChannelOpenned");
            }
        }

        private bool _isFlyoutEditChannelOpenned;
        public bool IsFlyoutEditChannelOpenned
        {
            get { return _isFlyoutEditChannelOpenned; }
            set
            {
                _isFlyoutEditChannelOpenned = value;
                NotifyPropertyChanged("IsFlyoutEditChannelOpenned");
            }
        }

        private string _channelTitleText;
        public string ChannelTitleText
        {
            get { return _channelTitleText; }
            set
            {
                _channelTitleText = value;
                NotifyPropertyChanged("ChannelTitleText");
            }
        }
        
        
        public ICommand LoadChannelCmd { get; set; }
        public ICommand LoadAllChannelsCmd { get; set; }

        public ICommand DeleteChannelCmd { get; set; }
        public ICommand AddChannelCmd { get; set; }
        public ICommand AddChannelCmdValidate { get; set; }
        public ICommand EditChannelCmd { get; set; }
        public ICommand EditChannelCmdValidate { get; set; }

        public ICommand SetReadChannelCmd { get; set; }
        public ICommand SetUnreadChannelCmd { get; set; }
        
        private void ManageChannels()
        {
            // ******** suppression d'un channel *********
            DeleteChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    try
                    {
                        ChannelsList.Remove(SelectedChannel);
                        _serveur.dropChannel(SelectedChannel.IdEntity);

                        IList<RssChannel> correctList = ChannelsList;

                        ChannelsList = null;
                        ChannelsList = correctList;

                        SelectedChannel = null;
                    }
                    catch (Exception e)
                    {
                        IsProgressRingActive = false;
                        if (e.InnerException != null)
                            DebugText = e.InnerException.Message;
                        else
                            DebugText = e.Message;
                    }
                    
                }
            }));


            // ******** ajout d'un channel *********
            AddChannelCmd = new Command(new Action(async () =>
            {
                IsFlyoutAddChannelOpenned = true;
                CurrentChannel = new RssChannel();
                NewCategoryForChannel = CurrentCategory;
                if (ChannelsList == null)
                    ChannelsList = new List<RssChannel>();
            }));

            AddChannelCmdValidate = new Command(new Action(async () =>
            {
                try
                {
                    FlyoutMessage = "";
                    IsProgressRingActive = true;
                    CurrentChannel = await _serveur.addChannelInCategoryWithCategoryNameAsync(CurrentChannel.RssLink, NewCategoryForChannel.Name);
                    
                    ChannelsList.Add(CurrentChannel);
                    IList<RssChannel> correctList = ChannelsList;

                    ChannelsList = null;
                    ChannelsList = correctList;

                    IsFlyoutAddChannelOpenned = false;
                    IsProgressRingActive = false;
                }
                catch (Exception e)
                {
                    IsProgressRingActive = false;
                    if (e.InnerException != null)
                        FlyoutMessage = e.InnerException.Message;
                    else
                        FlyoutMessage = e.Message;
                }
            }));




            // ******** edition d'un channel *********
            EditChannelCmd = new Command(new Action(async () =>
            {
                if (SelectedChannel != null)
                {
                    CurrentChannel = SelectedChannel;
                    IsFlyoutEditChannelOpenned = true;
                    NewCategoryForChannel = CurrentCategory;
                }
            }));

            EditChannelCmdValidate = new Command(new Action(async () =>
            {
                IsFlyoutEditChannelOpenned = false;
                try
                {                    
                    if (NewCategoryForChannel != CurrentCategory)
                    {
                        _serveur.moveChannelToCategory(CurrentCategory.IdEntity, NewCategoryForChannel.IdEntity, CurrentChannel.IdEntity);
                        
                        CategoryList = _serveur.getCategories();
                        ChannelsList = _serveur.getRssChannelsInCategoryWithCategoryName(CurrentCategory.Name);
                    }
                    FlyoutMessage = "";
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        FlyoutMessage = e.InnerException.Message;
                    else
                        FlyoutMessage = e.Message;
                }
            }));


            // ******** Set Channel lu *********
            SetReadChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    try
                    {
                        if (IsOn)
                            _serveur.setChannelAsRead(SelectedChannel.IdEntity);
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null)
                            DebugText = "[Exception]" + e.InnerException.Message;
                        else
                            DebugText = "[Exception]" + e.Message;
                    }
                }
                else
                {
                    DebugText = "[Error] SelectedChannel == null";
                }
            }));

            // ******** Set Channel non lu *********
            SetUnreadChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    try
                    {
                        if (IsOn)
                            _serveur.setChannelAsUnread(SelectedChannel.IdEntity);
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null)
                            DebugText = "[Exception]" + e.InnerException.Message;
                        else
                            DebugText = "[Exception]" + e.Message;
                    }
                }
                else
                {
                    DebugText = "[Error] SelectedChannel == null";
                }
            }));


        }

    }
}
