using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Compression
{
  public class CompressEngine
  {
    public static byte[] Compress(byte[] input)
    {
      var text = Convert.ToBase64String(input);
      using (var stream = new MemoryStream())
      {
        using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress))
        {
          using (var writer = new StreamWriter(deflateStream))
          {
            writer.Write(text);
          }
        }
        
        return stream.ToArray();
      }
    }

    public static byte[] Decompress(byte[] input)
    {
      using (var stream = new MemoryStream(input))
      {
        using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
        {
          using (var reader = new StreamReader(deflateStream))
          {
            var text = reader.ReadToEnd();
            return Convert.FromBase64String(text);
          }
        }
      }
    }
  }
}
