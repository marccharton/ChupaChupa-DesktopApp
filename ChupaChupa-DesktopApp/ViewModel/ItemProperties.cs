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

        private IList<RssItem> _selectedItemsList;
        public IList<RssItem> SelectedItemsList
        {
            get { return _selectedItemsList; }
            set
            {
                _selectedItemsList = value;
                NotifyPropertyChanged("SelectedItemsList");
            }
        }

        private string _itemsTitleText;
        public string ItemsTitleText
        {
            get { return _itemsTitleText; }
            set
            {
                _itemsTitleText = value;
                NotifyPropertyChanged("ItemsTitleText");
            }
        }

        public ICommand LoadItemCmd { get; set; }
        public ICommand LoadAllItemsCmd { get; set; }

        public ICommand SetReadItemCmd { get; set; }
        public ICommand SetUnreadItemCmd { get; set; }
        
        

        public void ManageItems()
        {
            // ******** Set article lu *********
            SetReadItemCmd = new Command(new Action(() =>
            {
                if (SelectedItem != null)
                {
                    try
                    {
                        SelectedItem.IsRead = true;
                        IsProgressRingActive = true;
                        _serveur.setItemAsRead(SelectedItem.IdEntity);
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null)
                            DebugText = e.InnerException.Message;
                        else
                            DebugText = e.Message;
                    }
                    IsProgressRingActive = false;
                }
                else
                {
                    DebugText = "[Error] SelectedItem = null";
                }
            }));



            // ******** Set article non lu *********
            SetUnreadItemCmd = new Command(new Action(() =>
            {
                if (SelectedItem != null)
                {
                    try
                    {
                        SelectedItem.IsRead = false;
                        IsProgressRingActive = true;
                        _serveur.setItemAsUnread(SelectedItem.IdEntity);
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null)
                            DebugText = e.InnerException.Message;
                        else
                            DebugText = e.Message;
                    }
                    IsProgressRingActive = false;
                }
                else
                {
                    DebugText = "[Error] SelectedItem = null";
                }
            }));

        }
        
    }
}
