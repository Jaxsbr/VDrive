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

        public static void PopulateKeyIV(ICipher cipher, SymmetricCipherType type)
        {
            switch (type)
            {
                case SymmetricCipherType.Aes:
                    var aes = Aes.Create();
                    aes.GenerateKey();
                    aes.GenerateIV();
                    cipher.Key = aes.Key;
                    cipher.IV = aes.IV;
                    break;
                case SymmetricCipherType.DES:
                    var des = DES.Create();
                    des.GenerateKey();
                    des.GenerateIV();
                    cipher.Key = des.Key;
                    cipher.IV = des.IV;
                    break;
                case SymmetricCipherType.RC2:
                    var rc2 = RC2.Create();
                    rc2.GenerateKey();
                    rc2.GenerateIV();
                    cipher.Key = rc2.Key;
                    cipher.IV = rc2.IV;
                    break;
                case SymmetricCipherType.Rijndael:
                    var rijndael = Rijndael.Create();
                    rijndael.GenerateKey();
                    rijndael.GenerateIV();
                    cipher.Key = rijndael.Key;
                    cipher.IV = rijndael.IV;
                    break;
                case SymmetricCipherType.TripleDES:
                    var tripleDES = TripleDES.Create();
                    tripleDES.GenerateKey();
                    tripleDES.GenerateIV();
                    cipher.Key = tripleDES.Key;
                    cipher.IV = tripleDES.IV;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid Symmetric Cipher Type Provided: {type}");
            }

            // TEMP:
            var sb = new StringBuilder();
            cipher.Key.ToList().ForEach(x => sb.Append(x + ","));
            sb.Clear();
            cipher.IV.ToList().ForEach(x => sb.Append(x + ","));
        }
    }
}
