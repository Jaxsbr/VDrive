using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Serialization
{
    public class ProtoBufferSerialization : ISerialization
    {
        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public byte[] Serialize(object obj)
        {
            if (obj == null) { return null; }
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, obj);
                return stream.ToArray();
            }            
        }
    }
}
