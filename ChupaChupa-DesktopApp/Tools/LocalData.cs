using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Serialization;
using Chupachupa_DesktopApp.PrivateService;

namespace Chupachupa_DesktopApp.Tools
{
    public class LocalData
    {
        public static bool                 isOn = false;
        public static bool                 init = true;
        public static DateTime             LastRefresh;
        public static IList<Category>      CategoriesList;
        public static IList<RssChannel>    ChannelsList;
        public static IList<RssItem>       ItemsList;


        public static IList<Category> RefreshCategories(ServiceContractClient serveur)
        {
            if (init || LastRefresh <= DateTime.Now.AddMinutes(-ToolsBox.TimeToRefreshDatas))
            {
                if (isOn)
                {
                    try
                    {
                        IList<Category> ret = serveur.getCategories();
                        CategoriesList = ret;
                        SaveCategoriesToLocal();
                        return ret;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    return GetCategoriesFromLocal();
                }
            }
            return CategoriesList;
        }

        public static IList<RssChannel> RefreshChannels(ServiceContractClient serveur)
        {
            if (init || LastRefresh <= DateTime.Now.AddMinutes(-ToolsBox.TimeToRefreshDatas))
            {
                if (isOn)
                {
                    try
                    {
                        IList<RssChannel> ret = serveur.getRssChannels();
                        ChannelsList = ret;
                        SaveChannelsToLocal();
                        return ret;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    return GetChannelsFromLocal();
                }
            }
            return ChannelsList;
        }

        public static IList<RssItem> RefreshItems(ServiceContractClient serveur)
        {
            if (init || LastRefresh <= DateTime.Now.AddMinutes(-ToolsBox.TimeToRefreshDatas))
            {
                if (isOn)
                {
                    try
                    {
                        var ret = new List<RssItem>();
                        foreach (var category in CategoriesList)
                        {
                            var listOfChannels = serveur.getRssChannelsInCategoryWithIdCategory(category.IdEntity);
                            if (listOfChannels == null)
                                continue;
                            foreach (var channel in listOfChannels)
                            {
                                var listOfItems = serveur.getRssItemsWithChannelId(channel.IdEntity);
                                if (listOfItems == null)
                                    continue;
                                ret.AddRange(listOfItems);
                            }
                        }
                        ItemsList = ret;
                        SaveItemsToLocal();
                        return ret;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    return GetItemsFromLocal();
                }
            }
            return ItemsList;
        }


        public static IList<Category> GetCategoriesFromLocal()
        {
            if (File.Exists(ToolsBox.CategoriesSerializePath))
                using (var fs = new FileStream(ToolsBox.CategoriesSerializePath, FileMode.Open))
                {
                    try
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(List<Category>));
                        CategoriesList = xml.Deserialize(fs) as List<Category>;
                        return CategoriesList;
                    }
                    catch(Exception e)
                    {
                        throw e;
                    }
                }
            return null;
        }


        public static IList<RssChannel> GetChannelsFromLocal()
        {
            if (File.Exists(ToolsBox.ChannelsSerializePath))
                using (var fs = new FileStream(ToolsBox.ChannelsSerializePath, FileMode.Open))
                {
                    try
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(List<RssChannel>));
                        ChannelsList = xml.Deserialize(fs) as List<RssChannel>;
                        return ChannelsList;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            return null;
        }

        public static IList<RssItem> GetItemsFromLocal()
        {
            if (File.Exists(ToolsBox.ItemsSerializePath))
                using (var fs = new FileStream(ToolsBox.ItemsSerializePath, FileMode.Open))
                {
                    try
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(List<RssItem>));
                        ItemsList = xml.Deserialize(fs) as List<RssItem>;
                        return ItemsList;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            return null;
        }


        public static void SaveCategoriesToLocal()
        {
            if (File.Exists(ToolsBox.CategoriesSerializePath))
                File.Delete(ToolsBox.CategoriesSerializePath);
            Serializer.Serialize(CategoriesList, ToolsBox.CategoriesSerializePath, FileMode.OpenOrCreate, typeof(List<Category>));
            //return true;
        }

        public static void SaveChannelsToLocal()
        {
            if (File.Exists(ToolsBox.ChannelsSerializePath))
                File.Delete(ToolsBox.ChannelsSerializePath);
            Serializer.Serialize(ChannelsList, ToolsBox.ChannelsSerializePath, FileMode.OpenOrCreate, typeof(List<RssChannel>));
        }

        public static void SaveItemsToLocal()
        {
            if (File.Exists(ToolsBox.ItemsSerializePath))
                File.Delete(ToolsBox.ItemsSerializePath);
            Serializer.Serialize(ItemsList, ToolsBox.ItemsSerializePath, FileMode.OpenOrCreate, typeof(List<RssItem>));
        }



        public async static Task LogUser(ServiceContractClient serveur, string login, string password)
        {
            try
            {
                serveur.ClientCredentials.UserName.UserName = login;
                serveur.ClientCredentials.UserName.Password = password;
                await serveur.authenticateAsync(login, password);

                isOn = true;

                LastRefresh = DateTime.Now;
                RefreshCategories(serveur);
                RefreshChannels(serveur);
                RefreshItems(serveur);

                init = false;
            }
            catch (EndpointNotFoundException e) // If we are in OFF Mode
            { 
                LastRefresh = DateTime.Now;
                GetCategoriesFromLocal();
                GetChannelsFromLocal();
                GetItemsFromLocal();
                isOn = false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
