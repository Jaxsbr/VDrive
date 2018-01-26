using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JJDev.VDrive.Core.Serialization
{

public class JsonSerialization
  {
    public static T Deserialize<T>(string json)
    {
      return JsonConvert.DeserializeObject<T>(json);
    }

    public static string Serialize<T>(T obj)
    {
      return JsonConvert.SerializeObject(obj);
    }
  }

}