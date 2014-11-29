using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using ChupaChupa_Model.Entities;
using System.Xml.Linq;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {

        #region Properties

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


            IsLoggedIn = false;
            IsTabControlEnabled = true;
            LogMessage = "LOG IN";
            
            // TODO : A supprimer
            DebugText = "[Ceci est un message de debug]";

            // TODO : A supprimer
            InitEntities();

            // Fonction appelant les initialisation des COMMANDs
            LogUser();
            LoadEntities();
            ManageCategories();
            ManageChannels();
            ManageSettings();
        }

        #endregion


        #region Methods

        private void LoadEntities()
        {
            // ******** chargement d'une catégorie *********
            LoadCategoryCmd = new Command(new Action(() =>
            {
                if (SelectedCategory != null)
                {
                    CurrentCategory = SelectedCategory;

                    // TODO : Récupérer la liste des channels de la catégorie selectionnée
                    // serveur.CategoryDAO.GetCategoryByName(CurrentCategory.Name).Channels

                    ChannelsList = CurrentCategory.RssChannels;

                    SelectedTabIndex = 2;
                }
            }));

            // ******** chargement d'un channel *********
            LoadChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    CurrentChannel = SelectedChannel;

                    // TODO : Récupérer la liste des artciles du channel selectionné
                    // serveur.RssChannelDAO.GetByTitle(CurrentChannel.Title).RssItems

                    ItemsList = CurrentChannel.RssItems;

                    SelectedTabIndex = 3;
                }
            }));


            // ******** chargement d'un item *********
            LoadItemCmd = new Command(new Action(() =>
            {
                if (SelectedItem != null)
                {
                    CurrentItem = SelectedItem;
                    SelectedTabIndex = 4;
                }
            }));


            // ******** chargement de toutes les catégories *********
            LoadAllCategoriesCmd = new Command(new Action(() =>
            {
                // TODO : Récupération de toutes les catégories de l'utilisateur
                //IList<Category> categories = serveur.FindAllCategories();
                //var channels = new List<RssChannel>();
                //foreach (Category category in categories)
                //{
                //    foreach (RssChannel channel in category.RssChannels)
                //    {
                //        channels.Add(channel);
                //    }
                //}


                SelectedTabIndex = 2;
            }));


            // ******** chargement de tous les channels *********
            LoadAllChannelsCmd = new Command(new Action(() =>
            {
                // TODO : Récupération de touts les channels de la catégorie actuelle
                //IList<RssChannel> channels = serveur.FindCategory(CurrentCategory).FindAllChannels();
                //var items = new List<RssItem>();
                //foreach (RssChannel channel in channels)
                //{
                //    foreach (RssItem item in channel.RssItems)
                //    {
                //        items.Add(item);
                //    }
                //}

                SelectedTabIndex = 3;
            }));
        }

        private void ManageCategories()
        {
            // ******** suppression d'une catégorie *********
            DeleteCategoryCmd = new Command(new Action(() =>
            {
                if (SelectedCategory != null)
                {
                    // TODO : Supprimer la catégorie de la base 
                    // serveur.CategoryDAO.DeleteCategory(SelectedCategory.IdEntity)

                    CategoryList.Remove(SelectedCategory);
                    IList<Category> correctList = CategoryList;

                    //TODO : puis mettre à jour CategoryList
                    CategoryList = null;
                    // CategoryList = serveur.CategoryDAO.FindAll()
                    CategoryList = correctList;

                    SelectedCategory = null;
                }
            }));


            // ******** ajout d'une catégorie *********
            AddCategoryCmd = new Command(new Action(async () =>
            {
                IsFlyoutAddCategoryOpenned = true;
                //IsTabControlEnabled = false;
                CurrentCategory = new Category();
            }));

            AddCategoryCmdValidate = new Command(new Action(async () =>
            {
                //TODO : ADD NEW CATEGORY WITH NAME
                // serveur.AddCategory(CurrentCategory);
                CategoryList.Add(CurrentCategory);
                IList<Category> correctList = CategoryList;

                //TODO : puis mettre à jour CategoryList
                // CategoryList = serveur.FindAllCategories();
                CategoryList = null;
                CategoryList = correctList;
                IsFlyoutAddCategoryOpenned = false;
                SelectedTabIndex = 1;
            }));



            // ******** edition d'une catégorie *********
            EditCategoryCmd = new Command(new Action(async () =>
            {
                if (SelectedCategory != null)
                {
                    CurrentCategory = SelectedCategory;
                    IsFlyoutEditCategoryOpenned = true;
                    //IsTabControlEnabled = false;
                }
            }));

            EditCategoryCmdValidate = new Command(new Action(async () =>
            {
                IsFlyoutEditCategoryOpenned = false;
                //IsTabControlEnabled = true;

                // TODO : UPDATE CURRENT CATEGORY
                // serveur.UpdateCategory(CurrentCategory)
            }));
        }

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
                if (ChannelsList == null)
                    ChannelsList = new List<RssChannel>();
            }));

            AddChannelCmdValidate = new Command(new Action(async () =>
            {
                // TODO : ADD NEW CHANNEL WITH LINK
                // serveur.addChannel(CurrentChannel.Link, CurrentCategory);
                CurrentChannel.RssItems = new List<RssItem>(); // Juste pour avoir un 0 dans le count
                ChannelsList.Add(CurrentChannel);

                IList<RssChannel> correctList = ChannelsList;

                //TODO : puis mettre à jour CHannel
                ChannelsList = null;
                ChannelsList = correctList;

                CurrentChannel = null;
                IsFlyoutAddChannelOpenned = false;
                //IsTabControlEnabled = true;

            }));




            // ******** edition d'un channel *********
            EditChannelCmd = new Command(new Action(async () =>
            {
                if (SelectedChannel != null)
                {
                    CurrentChannel = SelectedChannel;
                    IsFlyoutEditChannelOpenned = true;
                    //IsTabControlEnabled = false;
                }
            }));

            EditChannelCmdValidate = new Command(new Action(async () =>
            {
                IsFlyoutEditChannelOpenned = false;
                //IsTabControlEnabled = true;

                // serveur.ChannelDao.Update(CurrentChannel, NewCategoryForChannel);
                // TODO : UPDATE CURRENT CHANNEL
            }));
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


        // TODO : A supprimer
        private void InitEntities()
        {
            //ItemsList = new List<RssItem>()
            var rssItem1 = new RssItem() { Title = "Les huitres du groenland", Description = "ouech salut sdcsd jhc ", IdEntity = 1};
            var rssItem2 = new RssItem() { Title = "Les bons plan cuisto" , Description = "ouechdc sk doicu dcsdck sjdh c "};
            var rssItem3 = new RssItem() { Title = "La cuisine facile", Description = "ben ouai quoi c'est pas dur de cuisiner " };
            var rssItem4 = new RssItem() { Title = "coucou les moules" , Description = "ceci est ma description et tout ca "};
            var rssItem5 = new RssItem() { Title = "ouech ma gueule t'as faim ?" , Description = "ben parceque si cest pas le cas faut le dire sinon ca le fait pas"};
            var rssItem6 = new RssItem() { Title = "ouech ma gueule t'as faim ?" , Description = "ben parceque si cest pas le cas faut le dire sinon ca le fait pas"};
            var rssItem7 = new RssItem() { Title = "ouech ma gueule t'as faim ?" , Description = "ben parceque si cest pas le cas faut le dire sinon ca le fait pas"};

            var channel1 = new RssChannel() { Title = "Deezer", Link = "http://www.deezer.com/playlist/51228683", RssItems = new List<RssItem>() { rssItem2, rssItem4, rssItem6 } };
            var channel2 = new RssChannel() { Title = "Intranet", Link = "http://intra.epitech.eu/", RssItems = new List<RssItem>() { rssItem1, rssItem3, rssItem5, rssItem7 } };
            var channel3 = new RssChannel() { Title = "Youtube", Link = "https://www.youtube.com/", RssItems = new List<RssItem>() { rssItem1, rssItem2, rssItem3, rssItem4, rssItem5, rssItem6, rssItem7 } };
            var channel4 = new RssChannel() { Title = "01.net", Link = "http://www.01net.com/", RssItems = new List<RssItem>() { rssItem1 } };

            var category1 = new Category() { Name = "Web", RssChannels = new List<RssChannel>() { channel1, channel2, channel3, channel4} };
            var category2 = new Category() { Name = "Cuisine", RssChannels = new List<RssChannel>() { channel4, channel3, channel2, channel1 } };
            var category3 = new Category() { Name = "Voitures", RssChannels = new List<RssChannel>() { channel1, channel3 } };
            var category4 = new Category() { Name = "Voitures", RssChannels = new List<RssChannel>() { channel2, channel4 } };
            var category5 = new Category() { Name = "Voitures", RssChannels = new List<RssChannel>() { channel4 } };

            
            var catList = new List<Category>() {category1, category2, category3, category4, category5};
            //var chanList = new List<RssChannel>() { channel1, channel2, channel3, channel4 };
            //var artList = new List<RssItem>() { rssItem1, rssItem2, rssItem3, rssItem4, rssItem5, rssItem6, rssItem7 };

            CategoryList = catList;
 

            CurrentItem = new RssItem()
            {
                Author = "Kram47",
                Title = "Ouech Ma Gueule",
                Source = new Source() { Url = @"http://fr.wikipedia.org/wiki/Wikip%C3%A9dia:Citez_vos_sources" },
                Description = @"<p><!--[if !mso]><br /><style><br />v\:* {behavior:url(#default#VML);}<br />o\:* {behavior:url(#default#VML);}<br />w\:* {behavior:url(#default#VML);}<br />.shape {behavior:url(#default#VML);}<br /></style><br /><![endif]--></p><br /><p><!--[if gte mso 9]><xml><br /><w:WordDocument><br /><w:View>Normal</w:View><br /><w:Zoom>0</w:Zoom><br /><w:TrackMoves /><br /><w:TrackFormatting /><br /><w:HyphenationZone>21</w:HyphenationZone><br /><w:PunctuationKerning /><br /><w:ValidateAgainstSchemas /><br /><w:SaveIfXMLInvalid>false</w:SaveIfXMLInvalid><br /><w:IgnoreMixedContent>false</w:IgnoreMixedContent><br /><w:AlwaysShowPlaceholderText>false</w:AlwaysShowPlaceholderText><br /><w:DoNotPromoteQF /><br /><w:LidThemeOther>FR</w:LidThemeOther><br /><w:LidThemeAsian>X-NONE</w:LidThemeAsian><br /><w:LidThemeComplexScript>X-NONE</w:LidThemeComplexScript><br /><w:Compatibility><br /><w:BreakWrappedTables /><br /><w:SnapToGridInCell /><br /><w:WrapTextWithPunct /><br /><w:UseAsianBreakRules /><br /><w:DontGrowAutofit /><br /><w:SplitPgBreakAndParaMark /><br /><w:EnableOpenTypeKerning /><br /><w:DontFlipMirrorIndents /><br /><w:OverrideTableStyleHps /><br /></w:Compatibility><br /><w:BrowserLevel>MicrosoftInternetExplorer4</w:BrowserLevel><br /><m:mathPr><br /><m:mathFont m:/><br /><m:brkBin m:val=<p><!--[if !mso]><br /><style><br />v\:* {behavior:url(#default#VML);}<br />o\:* {behavior:url(#default#VML);}<br />w\:* {behavior:url(#default#VML);}<br />.shape {behavior:url(#default#VML);}<br /></style><br /><![endif]--></p><br /><p><!--[if gte mso 9]><xml><br /><w:WordDocument><br /><w:View>Normal</w:View><br /><w:Zoom>0</w:Zoom><br /><w:TrackMoves /><br /><w:TrackFormatting /><br /><w:HyphenationZone>21</w:HyphenationZone><br /><w:PunctuationKerning /><br /><w:ValidateAgainstSchemas /><br /><w:SaveIfXMLInvalid>false</w:SaveIfXMLInvalid><br /><w:IgnoreMixedContent>false</w:IgnoreMixedContent><br /><w:AlwaysShowPlaceholderText>false</w:AlwaysShowPlaceholderText><br /><w:DoNotPromoteQF /><br /><w:LidThemeOther>FR</w:LidThemeOther><br /><w:LidThemeAsian>X-NONE</w:LidThemeAsian><br /><w:LidThemeComplexScript>X-NONE</w:LidThemeComplexScript><br /><w:Compatibility><br /><w:BreakWrappedTables /><br /><w:SnapToGridInCell /><br /><w:WrapTextWithPunct /><br /><w:UseAsianBreakRules /><br /><w:DontGrowAutofit /><br /><w:SplitPgBreakAndParaMark /><br /><w:EnableOpenTypeKerning /><br /><w:DontFlipMirrorIndents /><br /><w:OverrideTableStyleHps /><br /></w:Compatibility><br /><w:BrowserLevel>MicrosoftInternetExplorer4</w:BrowserLevel><br /><m:mathPr><br /><m:mathFont m:/><br /><m:brkBin m:val="
            };

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


        // TODO:A supprimer
        //public static async Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle dialogStyle)
        //{
        //    var metroWindow = (Application.Current.MainWindow as MetroWindow);
        //    metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
        //    return await metroWindow.ShowInputAsync(title, message);
        //    //return await metroWindow.ShowMessageAsync(title, message, dialogStyle, metroWindow.MetroDialogOptions);
        //}

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