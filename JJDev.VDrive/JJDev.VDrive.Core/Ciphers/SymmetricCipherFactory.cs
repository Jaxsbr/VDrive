using JJDev.VDrive.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Ciphers
{
  public enum SymmetricCipherType
  {
    Aes,
    DES,
    RC2,
    Rijndael,
    TripleDES
  }

  public class SymmetricCipherFactory
  {
    public static SymmetricAlgorithm GetCipher(SymmetricCipherType type)
    {
      switch (type)
      {
        case SymmetricCipherType.Aes:
          return Aes.Create();
        case SymmetricCipherType.DES:
          return DES.Create();
        case SymmetricCipherType.RC2:
          return RC2.Create();
        case SymmetricCipherType.Rijndael:
          return Rijndael.Create();
        case SymmetricCipherType.TripleDES:
          return TripleDES.Create();
        default:
          throw new InvalidOperationException($"Invalid Symmetric Cipher Type Provided: {type}");
      }
    }
  }
}
