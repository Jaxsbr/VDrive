using JJDev.VDrive.Core.Ciphers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Contracts
{
  public interface ICipher
  {
    byte[] Key { get; set; }
    byte[] IV { get; set; }
    string Decode(SymmetricCipherType type, byte[] content);
    byte[] Encode(SymmetricCipherType type, string content);    
    ICryptoTransform GetCryptoTransform(SymmetricAlgorithm cryptor, CryptoStreamMode cryptoStreamMode);
    CryptoStream GetCryptoStream(MemoryStream stream, ICryptoTransform cryptoTransform, CryptoStreamMode cryptoStreamMode);
  }
}
