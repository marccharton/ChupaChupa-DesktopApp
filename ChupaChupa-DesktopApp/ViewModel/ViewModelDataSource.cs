using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Xml.Linq;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Chupachupa_DesktopApp.PrivateService;
using Chupachupa_DesktopApp.PublicService;
using Chupachupa_DesktopApp.Tools;

namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {

        #region Properties

        ServiceContractClient _serveur;
        PublicServiceContractClient _pubServeur;

        private int _selectedTabIndexUser;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndexUser; }
            set
            {
                _selectedTabIndexUser = value;
                NotifyPropertyChanged("SelectedTabIndex");
            }
        }

        private string _debugText;
        public string DebugText
        {
            get { return _debugText; }
            set
            {
                _debugText = value;
                NotifyPropertyChanged("DebugText");
            }
        }

        private bool _isTabControlEnabled;
        public bool IsTabControlEnabled
        {
            get { return _isTabControlEnabled; }
            set
            {
                _isTabControlEnabled = value;
                NotifyPropertyChanged("IsTabControlEnabled");
            }
        }

        private bool _isSettingsFloutOppenned;
        public bool IsSettingsFloutOppenned
        {
            get { return _isSettingsFloutOppenned;; }
            set
            {
                _isSettingsFloutOppenned = value;
                NotifyPropertyChanged("IsSettingsFloutOppenned");
            }
        }

        private bool _isProgressRingActive;
        public bool IsProgressRingActive
        {
            get { return _isProgressRingActive; ; }
            set
            {
                _isProgressRingActive = value;
                NotifyPropertyChanged("IsProgressRingActive");
            }
        }

        public ICommand ViewSettingsCmd { get; set; }
        public ICommand ViewSettingsCmdValidate { get; set; }
        public ICommand OpenInBrowserCmd { get; set; }

        private List<AccentColorMenuData> _accentColors;
        public List<AccentColorMenuData> AccentColors
        {
            get { return _accentColors; }
            set
            {
                _accentColors = value;
                NotifyPropertyChanged("AccentColors");
            }
        }

        private List<AppThemeMenuData> _appThemes;
        public List<AppThemeMenuData> AppThemes
        {
            get { return _appThemes; }
            set
            {
                _appThemes = value;
                NotifyPropertyChanged("AppThemes");
            }
        }

        #endregion


        #region Constructor

        public ViewModelDataSource()
        {
            ExitCmd = new Command(new Action(() => Application.Current.MainWindow.Close()));

            this.AccentColors = ThemeManager.Accents
                                           .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                           .ToList();

            this.AppThemes = ThemeManager.AppThemes
                                           .Select(a => new AppThemeMenuData() { Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();

            ConnectionErrorText = "";

            IsLoggedIn = false;
            IsTabControlEnabled = true;
            LogMessage = "LOG IN";
            
            // TODO : A supprimer
            DebugText = "";

            // Fonction appelant les initialisation des COMMANDs
            LogUser();
            LoadEntities();
            ManageUser();
            ManageCategories();
            ManageChannels();
            ManageItems();
            ManageSettings();
        }

        #endregion


        #region Methods

        private void LoadEntities()
        {   

            // ******** OUverture navigateur *********
            OpenInBrowserCmd = new Command(new Action(() =>
            {
                if (CurrentItem != null)
                    Process.Start(new ProcessStartInfo(CurrentItem.Link));
            }));


            // ******** chargement d'une catégorie *********
            LoadCategoryCmd = new Command(new Action(() =>
            {
                try
                {
                    if (SelectedCategory != null)
                    {
                        if (IsOn)
                        {
                            CurrentCategory = SelectedCategory;
                            SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_CHANNEL;

                            ChannelTitleText = CurrentCategory.Name;
                            ChannelsList = null;
                            ChannelsList = _serveur.getRssChannelsInCategoryWithCategoryName(CurrentCategory.Name);
                        }
                        SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_CHANNEL;
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        DebugText = e.InnerException.Message;
                    else
                        DebugText = e.Message;
                    DebugText += "Maybe, the category doesn't exist anymore...";
                }
            }));

            // ******** chargement d'un channel *********
            LoadChannelCmd = new Command(new Action(async () =>
            {
                if (SelectedChannel != null)
                {
                    CurrentChannel = SelectedChannel;
                    SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_ITEMS;
                    try
                    {
                        if (IsOn)
                        {
                            ItemsTitleText = CurrentChannel.Title;
                            ItemsList = null;
                            IsProgressRingActive = true;
                            ItemsList = await _serveur.getRssItemsWithChannelIdAsync(CurrentChannel.IdEntity);
                        }

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
            }));


            // ******** chargement d'un item *********
            LoadItemCmd = new Command(new Action(() =>
            {
                if (SelectedItem != null)
                {
                    CurrentItem = SelectedItem;
                    SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_VIEWER;
                    // CurrentItem.SetLu()
                    try
                    {
                        if (IsOn)
                        {
                            CurrentItem.IsRead = true;
                            IsProgressRingActive = true;
                            _serveur.setItemAsRead(CurrentItem.IdEntity);
                        }
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
            }));


            // ******** chargement de toutes les catégories *********
            LoadAllCategoriesCmd = new Command(new Action(async () =>
            {
                try
                {
                    SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_CHANNEL;
                    IsProgressRingActive = true;

                    if (IsOn)
                    {
                        CurrentCategory = null;
                        ChannelsList = null;
                        ChannelsList = await _serveur.getRssChannelsAsync();
                    }

                    ChannelTitleText = "All Channels";
                }
                catch (Exception e)
                {
                    IsProgressRingActive = false;
                    if (e.InnerException != null)
                        DebugText = "[Exception]" + e.InnerException.Message;
                    else
                        DebugText = "[Exception]" + e.Message;
                }
                IsProgressRingActive = false;
            }));


            // ******** chargement de tous les items de tous les channels d'une catégorie  *********
            LoadAllChannelsCmd = new Command(new Action(async () =>
            {
                if (CurrentCategory == null)
                {
                    LoadAllItemsCmd.Execute(null);
                    return ;
                }
                try
                {
                    SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_ITEMS;
                    if (IsOn)
                    {
                        ItemsTitleText = "All items In " + CurrentCategory.Name;
                        IsProgressRingActive = true;
                        ItemsList = null;
                        await LoadAllItemsOfChannels();
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        DebugText = "[Exception]" + e.InnerException.Message;
                    else
                        DebugText = "[Exception]" + e.Message;
                }
                IsProgressRingActive = false;
            }));


            // ******** chargement de tous les items *********
            LoadAllItemsCmd = new Command(new Action(async () =>
            {
                SelectedTabIndex = (int)ToolsBox.TabIndex.TAB_ITEMS;
                
                try
                {
                    ItemsTitleText = "All items";
                    if (IsOn)
                    {
                        IsProgressRingActive = true;
                        ItemsList = null;
                        await LoadAllItemsOfCategories();
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        DebugText = "[Exception]" + e.InnerException.Message;
                    else
                        DebugText = "[Exception]" + e.Message;
                }
                IsProgressRingActive = false;
            }));
        }


        async public Task LoadAllItemsOfChannels()
        {
            var list = new List<RssItem>();
            foreach (var channel in ChannelsList)
            {
                var listOfItems = await _serveur.getRssItemsWithChannelIdAsync(channel.IdEntity);
                if (listOfItems == null)
                    continue;
                list.AddRange(listOfItems);
            }
            ItemsList = null;
            ItemsList = list;
            //ItemsList = _serveur.GetRssItemsWithCategoryId(CurrentCategory.IdEntity);
        }

        async public Task LoadAllItemsOfCategories()
        {
            var list = new List<RssItem>();
            foreach (var category in CategoryList)
            {
                var listOfChannels = await _serveur.getRssChannelsInCategoryWithIdCategoryAsync(category.IdEntity);
                if (listOfChannels == null)
                    continue;
                foreach (var channel in listOfChannels)
                {
                    var listOfItems = await _serveur.getRssItemsWithChannelIdAsync(channel.IdEntity);
                    if (listOfItems == null)
                        continue;
                    list.AddRange(listOfItems);
                }
            }
            ItemsList = null;
            ItemsList = list;
        }





        private void ManageSettings()
        {
            ViewSettingsCmd = new Command(new Action(() =>
            {
                IsSettingsFloutOppenned = true;
            }));

            ViewSettingsCmdValidate = new Command(new Action(() =>
            {
                IsSettingsFloutOppenned = false;
            }));
        }

        public void ReinitiateData()
        {
            CategoryList = null;
            ChannelsList = null;
            ItemsList = null;
            CurrentCategory = null;
            CurrentChannel = null;
            CurrentItem = null;
            CurrentUserName = null;
            DebugText = "";
            FlyoutMessage = "";
        }


        #endregion


        #region Tools

        public MetroWindow GetMetroWindow()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            if (metroWindow != null)
                metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            return metroWindow;
        }

        #endregion

    }

    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand() { CanExecuteDelegate = x => true, ExecuteDelegate = x => this.DoChangeTheme(x) }); }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }
}