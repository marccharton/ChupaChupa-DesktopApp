using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using ChupaChupa_Model.Entities;


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




        public ViewModelDataSource()
        {
            IsLoggedIn = false;
            LogMessage = "LOG IN";

            DebugText = "[Affichez votre debug ici]";
          

            InitContent();



            #region Commands à Binder avec la View

            // ******** Log (in/out) du user *********
            LogUserCmd = new Command(new Action(() =>
            {
                if (!IsLoggedIn)
                {
                    if (AccountLoginText != "" && AccountPasswordText != "")
                    {
                        // TODO : Connection à la base avec les données du user
                        CurrentUser = new User() { LoginMail = AccountLoginText, Password = AccountPasswordText };
                        IsLoggedIn = true;
                        LogMessage = "LOG OUT";
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


            
            #region Chargements des principales entités

            // ******** chargement d'une catégorie *********
            LoadCategoryCmd = new Command(new Action(() =>
            {
                if (SelectedCategory != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    CurrentCategory = SelectedCategory;
                    ChannelsList = CurrentCategory.RssChannels;
                    SelectedTabIndex = 2;
                }
            }));

            // ******** chargement d'un channel *********
            LoadChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    CurrentChannel = SelectedChannel;
                    ItemsList = CurrentChannel.RssItems;
                    SelectedTabIndex = 3;
                }
            }));


            // ******** chargement d'un channel *********
            LoadItemCmd = new Command(new Action(() =>
            {
                if (SelectedItem != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
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
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    DebugText = SelectedCategory.Name;
                }
            }));


            // ******** ajout d'une catégorie *********
            AddCategoryCmd = new Command(new Action(() =>
            {
                if (SelectedCategory != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    DebugText = SelectedCategory.Name;
                }
            }));


            // ******** edition d'une catégorie *********
            EditCategoryCmd = new Command(new Action(() =>
            {
                if (SelectedCategory != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    DebugText = SelectedCategory.Name;
                }
            }));

            #endregion

            #region Ajout/Suppression/Edition d'un channel

            // ******** suppression d'une catégorie *********
            DeleteChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    DebugText = SelectedChannel.Title;
                }
            }));


            // ******** ajout d'une catégorie *********
            AddChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    DebugText = SelectedChannel.Title;
                }
            }));


            // ******** edition d'une catégorie *********
            EditChannelCmd = new Command(new Action(() =>
            {
                if (SelectedChannel != null)
                {
                    // TODO : Récupérer la liste des artciles du channel selectionné
                    DebugText = SelectedChannel.Title;
                }
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



    }
}