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

        public ICommand LoadChannelCmd { get; set; }
    }
}
