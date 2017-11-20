using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JJDev.VDrive.Core.Serialization
{
    public class XmlSerialization : ISerialization
    {
        public T Deserialize<T>(byte[] data)
        {
            object obj = null;
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                obj = xmlSerializer.Deserialize(memoryStream);
            }
            return (T)obj;
        }

        public byte[] Serialize(object obj)
        {
            if (obj == null) { return null; }

            var xmlSerializer = new XmlSerializer(obj.GetType());
            using (var memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    }
}
