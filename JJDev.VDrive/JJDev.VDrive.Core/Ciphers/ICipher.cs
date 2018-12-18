using JJDev.VDrive.Core.Ciphers;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Ciphers
{
    public interface ICipher
    {
        byte[] Key { get; set; }
        byte[] IV { get; set; }
        Task<string> Decode(SymmetricCipherType type, byte[] content);
        Task<byte[]> Encode(SymmetricCipherType type, string content);
        ICryptoTransform GetCryptoTransform(SymmetricAlgorithm cryptor, CryptoStreamMode cryptoStreamMode);
        CryptoStream GetCryptoStream(MemoryStream stream, ICryptoTransform cryptoTransform, CryptoStreamMode cryptoStreamMode);
    }
}
