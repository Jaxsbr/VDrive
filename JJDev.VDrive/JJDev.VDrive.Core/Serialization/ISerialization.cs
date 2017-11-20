using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Serialization
{
    public interface ISerialization
    {
        byte[] Serialize(object obj);
        T Deserialize<T>(byte[] data);
    }
}
