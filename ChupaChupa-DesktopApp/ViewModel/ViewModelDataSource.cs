using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using ChupaChupa_Model.Entities;
using System.Xml.Linq;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Chupachupa_DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public partial class ViewModelDataSource : INotifyPropertyChanged
    {

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
        


        public ViewModelDataSource()
        {
            IsLoggedIn = false;
            IsTabControlEnabled = true;
            LogMessage = "LOG IN";
            DebugText = "[Ceci est un message de debug]";
          

            InitContent();



            #region Commands à Binder avec la View

            // ******** Log (in/out) du user *********
            LogUserCmd = new Command(new Action(async () =>
            {
                if (!IsLoggedIn)
                {
                    if (AccountLoginText != "" && AccountPasswordText != "")
                    {
                        // TODO : Connection à la base avec les données du user
                        IsLoggedIn = true;
                        LogMessage = "LOG OUT";
                        IsProgressRingActive = true;
                        await Task.Delay(3000);
                        IsProgressRingActive = false;
                        CurrentUser = new User() { LoginMail = AccountLoginText, Password = AccountPasswordText };
                        SelectedTabIndex = 1;
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


            ViewSettingsCmd = new Command(new Action(() =>
            {
                IsSettingsFloutOppenned = true;
            }));

            ViewSettingsCmdValidate = new Command(new Action(() =>
            {
                IsSettingsFloutOppenned = false;
            }));


            #region Chargements des principales entités

            // ******** chargement d'une catégorie *********
            LoadCategoryCmd = new Command(new Action(() =>
            {
                if (SelectedCategory != null)
                {
                    CurrentCategory = SelectedCategory;
                    
                    // TODO : Récupérer la liste des channels de la catégorie selectionnée
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
                    ItemsList = CurrentChannel.RssItems;

                    SelectedTabIndex = 3;
                }
            }));


            // ******** chargement d'un channel *********
            LoadItemCmd = new Command(new Action(() =>
            {
                if (SelectedItem != null)
                {
                    // TODO : Récupérer l'item selectionné
                    CurrentItem = SelectedItem;
                    SelectedTabIndex = 4;
                }
            }));

            #endregion

            #region Ajout/Suppression/Edition d'une catégorie

            // ******** suppression d'une catégorie *********
            DeleteCategoryCmd = new Command(new Action(() =>
            {
                if (SelectedCategory != null)
                {
                    DebugText = SelectedCategory.Name;
                    
                    // TODO : Supprimer la catégorie de la base 
                    CategoryList.Remove(SelectedCategory);
                    IList<Category> correctList = CategoryList;

                    //TODO : puis mettre à jour CategoryList
                    CategoryList = null;
                    CategoryList = correctList;

                    SelectedCategory = null;
                }
            }));


            // ******** ajout d'une catégorie *********
            AddCategoryCmd = new Command(new Action(async () =>
            {
                var metroWindow = GetMetroWindow();
                var result = await metroWindow.ShowInputAsync("New Category", "New Name");
                if (result == null) //user pressed cancel
                    return;

                //TODO : ADD NEW CATEGORY WITH NAME
                CategoryList.Add(new Category(){Name = result});
                IList<Category> correctList = CategoryList;

                //TODO : puis mettre à jour CategoryList
                CategoryList = null;
                CategoryList = correctList;
            }));


            // ******** edition d'une catégorie *********
            EditCategoryCmd = new Command(new Action(async () =>
            {
                if (SelectedCategory != null)
                {
                    var metroWindow = GetMetroWindow();
                    var result = await metroWindow.ShowInputAsync(SelectedCategory.Name, "New Name");
                    if (result == null) //user pressed cancel
                        return;
                    
                    //TODO : UPDATE SELECTED CATEGORY
                    await metroWindow.ShowMessageAsync("Modification d'une catégorie", "Ok je prend note je modifie ta catégorie avec le nom '" + result + "'.\nBon en fait rien ne se passe car je vais pas me faire chier pour rien car c'est le webserice qui fera la mise à jour");                
                }
            }));

            #endregion

            #region Ajout/Suppression/Edition d'un channel

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

            // ******** ajout d'un channel *********
            AddChannelCmdValidate = new Command(new Action(async () =>
            {
                ////TODO : ADD NEW CHANNEL WITH LINK
                
                ChannelsList.Add(new RssChannel() { Title = CurrentChannel.Link });
                IList<RssChannel> correctList = ChannelsList;

                //TODO : puis mettre à jour CHannel
                ChannelsList = null;
                ChannelsList = correctList;

                CurrentChannel = null;
                IsFlyoutAddChannelOpenned = false;
                //IsTabControlEnabled = true;

                ////TODO : ADD NEW CHANNEL WITH LINK
                //if (ChannelsList != null)
                //{
                //    ChannelsList.Add(new RssChannel() { Link = result });
                //    IList<RssChannel> correctList = ChannelsList;

                //    //TODO : puis mettre à jour CHannel
                //    ChannelsList = null;
                //    ChannelsList = correctList;
                //}
            }));




            // ******** edition d'un channel *********
            EditChannelCmd = new Command(new Action(async () =>
            {
                if (SelectedChannel != null)
                {
                //    var metroWindow = GetMetroWindow();
                //    var result = await metroWindow.ShowInputAsync(SelectedCategory.Name, "New Name");
                //    if (result == null) //user pressed cancel
                //        return;
                    CurrentChannel = SelectedChannel;
                    IsFlyoutEditChannelOpenned = true;
                    //IsTabControlEnabled = false;
                }
            }));

            EditChannelCmdValidate = new Command(new Action(async () =>
            {
                IsFlyoutEditChannelOpenned = false;
                //IsTabControlEnabled = true;

                // serveur.ChannelDao.Update(CurrentChannel);
                // TODO : UPDATE CURRENT CHANNEL
            }));

            #endregion



            #endregion





            // lien image barbie : "http://www.pubenstock.com/wp-content/uploads/2014/06/CHUPA-KIPIK-2.jpg"





        }

       

        private void InitContent()
        {
            //ItemsList = new List<RssItem>()
            var rssItem1 = new RssItem() { Title = "Les huitres du groenland", Description = "ouech salut sdcsd jhc "};
            var rssItem2 = new RssItem() { Title = "Les bons plan cuisto" , Description = "ouechdc sk doicu dcsdck sjdh c "};
            var rssItem3 = new RssItem() { Title = "La cuisine facile", Description = "ben ouai quoi c'est pas dur de cuisiner " };
            var rssItem4 = new RssItem() { Title = "coucou les moules" , Description = "ceci est ma description et tout ca "};
            var rssItem5 = new RssItem() { Title = "ouech ma gueule t'as faim ?" , Description = "ben parceque si cest pas le cas faut le dire sinon ca le fait pas"};
            var rssItem6 = new RssItem() { Title = "ouech ma gueule t'as faim ?" , Description = "ben parceque si cest pas le cas faut le dire sinon ca le fait pas"};
            var rssItem7 = new RssItem() { Title = "ouech ma gueule t'as faim ?" , Description = "ben parceque si cest pas le cas faut le dire sinon ca le fait pas"};

            var channel1 = new RssChannel() { Title = "Youporn", RssItems = new List<RssItem>(){rssItem2, rssItem4, rssItem6} };
            var channel2 = new RssChannel() { Title = "Intranet", RssItems = new List<RssItem>() { rssItem1, rssItem3, rssItem5, rssItem7 } };
            var channel3 = new RssChannel() { Title = "Youtube", RssItems = new List<RssItem>() { rssItem1, rssItem2, rssItem3, rssItem4, rssItem5, rssItem6, rssItem7 } };
            var channel4 = new RssChannel() { Title = "01.net", RssItems = new List<RssItem>() { rssItem1 } };

            var category1 = new Category() { Name = "Web", RssChannels = new List<RssChannel>() { channel1, channel2, channel3, channel4} };
            var category2 = new Category() { Name = "Cuisine", RssChannels = new List<RssChannel>() { channel4, channel3, channel2, channel1 } };
            var category3 = new Category() { Name = "Voitures", RssChannels = new List<RssChannel>() { channel1, channel3 } };
            var category4 = new Category() { Name = "Voitures", RssChannels = new List<RssChannel>() { channel2, channel4 } };
            var category5 = new Category() { Name = "Voitures", RssChannels = new List<RssChannel>() { channel4 } };

            
            var catList = new List<Category>() {category1, category2, category3, category4, category5};
            var chanList = new List<RssChannel>() { channel1, channel2, channel3, channel4 };
            var artList = new List<RssItem>() { rssItem1, rssItem2, rssItem3, rssItem4, rssItem5, rssItem6, rssItem7 };

            CategoryList = catList;
 

            CurrentItem = new RssItem()
            {
                Author = "Kram47",
                Title = "Ouech Ma Gueule",
                Source = new Source() { Url = @"http://fr.wikipedia.org/wiki/Wikip%C3%A9dia:Citez_vos_sources" },
                Description = "Ouech ma gueule alors ceci est un article et tout ca et en fait ben j'ai rien a dire simplement car ceci est un putain de texte de de test a la con et que je vais pas me faire chier en plus a rajouter quelquechose de potable ..."
            };

        }

        //public static async Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle dialogStyle)
        //{
        //    var metroWindow = (Application.Current.MainWindow as MetroWindow);
        //    metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
        //    return await metroWindow.ShowInputAsync(title, message);
        //    //return await metroWindow.ShowMessageAsync(title, message, dialogStyle, metroWindow.MetroDialogOptions);
        //}

        public MetroWindow GetMetroWindow()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            if (metroWindow != null)
                metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            return metroWindow;
        }

    }
}