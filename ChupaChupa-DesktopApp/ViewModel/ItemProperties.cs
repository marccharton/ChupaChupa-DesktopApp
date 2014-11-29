using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using Chupachupa_DesktopApp.PrivateService;


namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {
        private RssItem _selectedItem;
        public RssItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        private RssItem _currentItem;
        public RssItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                NotifyPropertyChanged("CurrentItem");
            }
        }

        private IList<RssItem> _itemsList;
        public IList<RssItem> ItemsList
        {
            get { return _itemsList; }
            set
            {
                _itemsList = value;
                NotifyPropertyChanged("ItemsList");
            }
        }

        public ICommand LoadItemCmd { get; set; }
    }
}
