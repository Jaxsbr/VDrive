using JJDev.VDrive.Core.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Ciphers
{
  public class AesChiper : ICipher
  {


    public string Decode(string encodedContent)
    {
      throw new NotImplementedException();
    }

    public byte[] Encode(string content)
    {
        byte[] encrypted = null;
        using (var aes = Aes.Create())
        {
            aes.Key = null;
            aes.IV = null;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var stream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(content);
                    }
                    encrypted = stream.ToArray();
                }
            }
        }
      return encrypted;
    }
  }
}
