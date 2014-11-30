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

        
        public ICommand LoadChannelCmd { get; set; }
        public ICommand LoadAllChannelsCmd { get; set; }

        public ICommand DeleteChannelCmd { get; set; }
        public ICommand AddChannelCmd { get; set; }
        public ICommand AddChannelCmdValidate { get; set; }
        public ICommand EditChannelCmd { get; set; }
        public ICommand EditChannelCmdValidate { get; set; }


        private void ManageChannels()
        {
            // ******** suppression d'un channel *********
            DeleteChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    DebugText = SelectedChannel.Title;

                    // TODO : Supprimer la catégorie de la base 
                    ChannelsList.Remove(SelectedChannel);
                    IList<RssChannel> correctList = ChannelsList;

                    //TODO : puis mettre à jour ChannelList
                    ChannelsList = null;
                    ChannelsList = correctList;

                    SelectedChannel = null;
                }
            }));


            // ******** ajout d'un channel *********
            AddChannelCmd = new Command(new Action(async () =>
            {
                IsFlyoutAddChannelOpenned = true;
                //IsTabControlEnabled = false;
                CurrentChannel = new RssChannel();
                NewCategoryForChannel = CurrentCategory;
                if (ChannelsList == null)
                    ChannelsList = new List<RssChannel>();
            }));

            AddChannelCmdValidate = new Command(new Action(async () =>
            {
                try
                {
                    CurrentChannel = _serveur.addChannelInCategoryWithCategoryName(CurrentChannel.RssLink, NewCategoryForChannel.Name);
                    
                    ChannelsList.Add(CurrentChannel);
                    IList<RssChannel> correctList = ChannelsList;

                    ChannelsList = null;
                    ChannelsList = correctList;

                    IsFlyoutAddChannelOpenned = false;
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




            // ******** edition d'un channel *********
            EditChannelCmd = new Command(new Action(async () =>
            {
                if (SelectedChannel != null)
                {
                    CurrentChannel = SelectedChannel;
                    IsFlyoutEditChannelOpenned = true;
                    //IsTabControlEnabled = false;
                    NewCategoryForChannel = CurrentCategory;
                }
            }));

            EditChannelCmdValidate = new Command(new Action(async () =>
            {
                IsFlyoutEditChannelOpenned = false;
                try
                {
                    //_serveur.updateChannel(NewCategoryForChannel.Name);
                    if (NewCategoryForChannel != CurrentCategory)
                    {
                        NewCategoryForChannel.RssChannels.Add(CurrentChannel);
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
                //IsTabControlEnabled = true;

                // serveur.ChannelDao.Update(CurrentChannel, NewCategoryForChannel);
                // TODO : UPDATE CURRENT CHANNEL
            }));
        }

    }
}
