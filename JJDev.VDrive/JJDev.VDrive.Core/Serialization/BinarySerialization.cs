using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Serialization
{
    public class BinarySerialization : ISerialization
    {
        public T Deserialize<T>(byte[] data)
        {
            object obj = null;
            var binaryFormatter = new BinaryFormatter();            
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                obj = binaryFormatter.Deserialize(memoryStream);
            }
            return (T)obj;
        }

        public byte[] Serialize(object obj)
        {
            if (obj == null) { return null; }

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    }
}
