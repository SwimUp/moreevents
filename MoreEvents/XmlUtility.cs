using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RimOverhaul
{
    public static class XmlUtility
    {
        public static void Serialize<T>(T obj, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                serializer.Serialize(fs, obj);
            }
        }

        public static T Deserialize<T>(string path) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T @class = null;
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                @class = serializer.Deserialize(fs) as T;
            }

            return @class;
        }
    }
}
