using System;
using System.Collections.Generic;
using System.Linq;
using Chupachupa_DesktopApp_Test.PrivateService;
using Chupachupa_DesktopApp_Test.PublicService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chupachupa_DesktopApp_Test
{
    [TestClass]
    public class UnitTestPrivateService
    {
        private ServiceContractClient       _serveur    = new ServiceContractClient();

        public UnitTestPrivateService()
        {
            _serveur.ClientCredentials.UserName.UserName = "UnitTest_user";
            _serveur.ClientCredentials.UserName.Password = "UnitTest_pass";
            _serveur.authenticate("UnitTest_user", "UnitTest_pass");
        }

        #region Entity Unit Testing : Categories

        [TestMethod]
        public void GetCategories_Success()
        {
            var CategoryList = _serveur.getCategories();
            Assert.IsNotNull(CategoryList);
            Assert.IsInstanceOfType(CategoryList, typeof(List<Category>));
        }

        [TestMethod]
        public void DropCategoryByName_Success()
        {
            string categoryName = "UnitTest_CategoryASupprimer";
            _serveur.addCategory(categoryName);

            _serveur.dropCategoryWithCategoryName(categoryName);
            var CategoryList = _serveur.getCategories();
            Assert.IsNotNull(CategoryList);
            Assert.IsInstanceOfType(CategoryList, typeof(List<Category>));
            foreach (var category in CategoryList)
            {
                if (category.Name == categoryName)
                    Assert.Fail("The category has not been dropped.");
            }
        }


        [TestMethod]
        public void AddCategory_Success()
        {
            var rd = new Random();

            var randomValue = rd.Next(0, 470);
            string categoryName = "UnitTest_Informatique" + randomValue;
            _serveur.addCategory(categoryName);

            var CategoryList = _serveur.getCategories();
            Assert.IsNotNull(CategoryList);
            Assert.IsInstanceOfType(CategoryList, typeof(List<Category>));
            var item = (from cat in CategoryList where cat.Name == categoryName select cat).First();
            Assert.IsNotNull(item);
            Assert.AreEqual(item.Name, categoryName);
        }


        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.FaultException))]
        public void AddCategory_NameAlreadyExists()
        {
            _serveur.addCategory("UnitTest_DeuxFoisLeMemeNom");
            _serveur.addCategory("UnitTest_DeuxFoisLeMemeNom");
        }

        [TestMethod]
        public void RenameCategory_Success()
        {
            var c = _serveur.addCategory("UnitTest_CategoryOldName");

            long id = c.IdEntity;
            _serveur.renameCategory(id, "UnitTest_Nouveau_Nom");

            var CategoryList = _serveur.getCategories();
            Assert.IsNotNull(CategoryList);
            Assert.IsInstanceOfType(CategoryList, typeof(List<Category>));
            var item = (from cat in CategoryList where cat.IdEntity == id select cat).First();
            Assert.IsNotNull(item);
            Assert.AreEqual(item.Name, "UnitTest_Nouveau_Nom");
            _serveur.dropCategoryWithCategoryName("UnitTest_Nouveau_Nom");
        }

        #endregion


        #region Entity Unit Testing : Channels

        [TestMethod]
        public void GetChannelsByCategoryName_Success()
        {
            var c   = _serveur.addCategory("UnitTest_CategoryToGetChannelsFrom");
            var ch = _serveur.addChannelInCategoryWithCategoryName("http://www.lequipe.fr/rss/actu_rss_Football.xml", "UnitTest_CategoryToGetChannelsFrom");
            c = _serveur.getCategoryById(c.IdEntity);

            var RssChannelList = _serveur.getRssChannelsInCategoryWithCategoryName(c.Name);
            Assert.IsNotNull(RssChannelList);
            Assert.IsInstanceOfType(RssChannelList, typeof(List<RssChannel>));
            Assert.AreEqual(c.RssChannels, RssChannelList.Count);

            _serveur.dropChannelFromCategory(c.IdEntity, ch.IdEntity);
            _serveur.dropCategoryWithCategoryName(c.Name);
        }

        [TestMethod]
        public void GetChannels_Success()
        {
            var CategoryList = _serveur.getCategories();
            Assert.IsNotNull(CategoryList);
            Assert.IsInstanceOfType(CategoryList, CategoryList.GetType());
            long totalChannels = 0;
            foreach (var category in CategoryList)
                totalChannels += category.RssChannels;
            
            var RssChannelList = _serveur.getRssChannels();
            Assert.IsNotNull(RssChannelList);
            Assert.IsInstanceOfType(RssChannelList, typeof(List<RssChannel>));
            Assert.AreEqual(RssChannelList.Count, totalChannels);
        }

        [TestMethod]
        public void AddRssChannel_Success()
        {
            string categoryName = "UnitTest_CategoryAvecRssChannels";
            _serveur.addCategory(categoryName);
            _serveur.addChannelInCategoryWithCategoryName("http://www.lequipe.fr/rss/actu_rss_Football.xml", "UnitTest_CategoryAvecRssChannels");

            var CategoryList = _serveur.getCategories();
            Assert.IsNotNull(CategoryList);
            Assert.IsInstanceOfType(CategoryList, typeof(List<Category>));
            var category = (from cat in CategoryList where cat.Name == "UnitTest_CategoryAvecRssChannels" select cat).First();

            var RssChannelList = new List<RssChannel>();
            IList<RssChannel> ret = _serveur.getRssChannelsInCategoryWithCategoryName("UnitTest_CategoryAvecRssChannels");
            if (_serveur != null && ret != null)
                RssChannelList = ret.ToList();
            Assert.IsNotNull(RssChannelList);
            Assert.IsInstanceOfType(RssChannelList, RssChannelList.GetType());
            Assert.AreEqual(category.RssChannels, RssChannelList.Count);
            var itemChannel = (from chan in RssChannelList where chan.RssLink == "http://www.lequipe.fr/rss/actu_rss_Football.xml" select chan).First();

            Assert.IsNotNull(itemChannel);
            Assert.AreEqual(itemChannel.RssLink, "http://www.lequipe.fr/rss/actu_rss_Football.xml");

            _serveur.dropChannelFromCategory(category.IdEntity, itemChannel.IdEntity);
            _serveur.dropCategoryWithCategoryName("UnitTest_CategoryAvecRssChannels");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ServiceModel.FaultException))]
        public void AddRssChannel_AlreadyExists()
        {
            _serveur.addChannelInCategoryWithCategoryName("http://www.lequipe.fr/rss/actu_rss_Football.xml", "UnitTest_CategoryAvecRssChannels");
            _serveur.addChannelInCategoryWithCategoryName("http://www.lequipe.fr/rss/actu_rss_Football.xml", "UnitTest_CategoryAvecRssChannels");
        }

        #endregion


        #region Entity Unit Testing : RssItems

        [TestMethod]
        public void GetItemById_Success()
        {
            var firstCategory = _serveur.getCategories().First();
            long id = firstCategory.IdEntity;
            var item = _serveur.getRssItemById(id);

            Assert.IsNotNull(item);
            Assert.IsInstanceOfType(item, typeof(RssItem));
            Assert.AreEqual(item.IdEntity, id);
        }

        [TestMethod]
        public void GetItemsWithCategoryId_Success()
        {
            var firstCategory = _serveur.getCategories().First();
            long catId = firstCategory.IdEntity;
            var itemsList = _serveur.getRssItemsWithCategoryId(catId);
            
            Assert.IsNotNull(itemsList);
            Assert.IsInstanceOfType(itemsList, typeof(List<RssItem>));

            var RssChannelList = _serveur.getRssChannelsInCategoryWithIdCategory(firstCategory.IdEntity);
            Assert.IsNotNull(RssChannelList);
            Assert.IsInstanceOfType(RssChannelList, typeof(List<RssChannel>));
            Assert.AreEqual(firstCategory.RssChannels, RssChannelList.Count);

            long totalItems = 0;
            foreach (var channel in RssChannelList)
                totalItems += channel.RssItems;
            Assert.AreEqual(itemsList.Count, totalItems);
        }

        [TestMethod]
        public void GetRssItemWithChannelId_Success()
        {
            var firstChannel = _serveur.getRssChannels().First();
            var itemsList = _serveur.getRssItemsWithChannelId(firstChannel.IdEntity);

            Assert.IsNotNull(itemsList);
            Assert.IsInstanceOfType(itemsList, typeof(List<RssItem>));
            Assert.AreEqual(itemsList.Count, firstChannel.RssItems);
        }

        #endregion


        #region Entity Unit Testing : Account / User



        #endregion


    }
}
