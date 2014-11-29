using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Chupachupa_DesktopApp.Class
{
    public class Serializer
    {
        public static bool Serialize(Object obj, string path, FileMode fmode, Type type)
        {
            using (var fs = new FileStream(path, fmode))
            {
                try
                {
                    Console.WriteLine("[Serialisation] file: " + Path.GetFullPath(path));
                    XmlSerializer xml = new XmlSerializer(type);
                    xml.Serialize(fs, obj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Failed]\n" + ex.Message);
                    return false;
                }
            }
            Console.WriteLine("Succeed");
            return true;
        }

        public static Object Deserialize<T>(string path, FileMode fmode)
        {
            using (var fs = new FileStream(path, fmode))
            {
                try
                {
                    Console.WriteLine("[Deserialisation] file: " + Path.GetFullPath(path));
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    Console.WriteLine("Succeed");
                    return xml.Deserialize(fs) as Object;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Failed]\n" + ex.Message);
                    return null;
                }
            }
        }
    }
}
