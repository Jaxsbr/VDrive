using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Serialization
{
    public class ProtoBufferSerialization : ISerialization
    {
        public T Deserialize<T>(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
