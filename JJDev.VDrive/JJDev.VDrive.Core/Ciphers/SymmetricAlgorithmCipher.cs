﻿using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Ciphers
{
    public class SymmetricAlgorithmCipher : ICipher
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        public async Task<string> Decode(SymmetricCipherType type, byte[] content)
        {
          return await Task.Run(() =>
          {
            var decoded = string.Empty;
            var cryptor = SymmetricCipherFactory.GetCipher(type);
            var cryptoTransform = GetCryptoTransform(cryptor, CryptoStreamMode.Read);
            using (var stream = new MemoryStream(content))
            {
              using (var cryptoStream = GetCryptoStream(stream, cryptoTransform, CryptoStreamMode.Read))
              {
                using (var reader = new StreamReader(cryptoStream))
                {
                  decoded = reader.ReadToEnd();
                }
              }
            }

            return decoded;
          });
        }

        public async Task<byte[]> Encode(SymmetricCipherType type, string content)
        {
            return await Task.Run(() =>
            {
              byte[] encoded;
              var cryptor = SymmetricCipherFactory.GetCipher(type);
              var cryptoTransform = GetCryptoTransform(cryptor, CryptoStreamMode.Write);
              using (var stream = new MemoryStream())
              {
                using (var cryptoStream = GetCryptoStream(stream, cryptoTransform, CryptoStreamMode.Write))
                {
                  using (var writer = new StreamWriter(cryptoStream))
                  {
                    writer.Write(content);
                  }
                  encoded = stream.ToArray();
                }
              }

              return encoded;
            });            
        }

        public CryptoStream GetCryptoStream(MemoryStream stream, ICryptoTransform cryptoTransform, CryptoStreamMode cryptoStreamMode)
        {
            return new CryptoStream(stream, cryptoTransform, cryptoStreamMode);
        }

        public ICryptoTransform GetCryptoTransform(SymmetricAlgorithm cryptor, CryptoStreamMode cryptoStreamMode)
        {
            if (cryptoStreamMode == CryptoStreamMode.Read) { return cryptor.CreateDecryptor(Key, IV); }
            else { return cryptor.CreateEncryptor(Key, IV); }
        }
    }
}
