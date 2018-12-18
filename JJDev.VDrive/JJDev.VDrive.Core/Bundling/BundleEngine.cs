using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JJDev.VDrive.Core.Serialization;
using JJDev.VDrive.Core.Ciphers;
using JJDev.VDrive.Core.Compression;

namespace JJDev.VDrive.Core.Bundling
{
    public class BundleEngine : IBundleEngine
    {                

        public async Task<object> Compress(string source, string destination, ICipher cipher)
        {
            var directoryManifest = new DirectoryManifest(source);
            using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write))
            {
                var writer = new BinaryWriter(fileStream);
                await GenerateEncodedBundle(cipher, directoryManifest, writer);                
            }
            return null;                     
        }

        private async Task GenerateEncodedBundle(ICipher cipher, DirectoryManifest directoryManifest, BinaryWriter writer)
        {          
          var serializer = new BinarySerialization();
          var directoryManifestBytes = serializer.Serialize(directoryManifest);
          await WriteBinaryData(cipher, writer, directoryManifestBytes);

          foreach (DirectoryElement directoryElement in directoryManifest.Elements)
          {
            if (!directoryElement.IsDirectory)
            {
              var fileBytes = directoryElement.GetFileData();
              await WriteBinaryData(cipher, writer, fileBytes);
            }
          }

          writer.Flush();
          writer.BaseStream.Position = 0;
          writer.Close();
        }

        private async Task WriteBinaryData(ICipher cipher, BinaryWriter writer, byte[] bytes, int position = 0)
        {
          var compressedData = await CompressEngine.CompressAsync(bytes);
          var base64Data = Convert.ToBase64String(compressedData);
          var encodedData = await cipher.Encode(SymmetricCipherType.Aes, base64Data);

          writer.Write(encodedData.Length);
          writer.Write(encodedData, position, encodedData.Length);
        }


        public async Task<object> Decompress(string source, string destination, ICipher cipher)
        {
            using (var fileStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                var reader = new BinaryReader(fileStream);
                await UnpackEncodedBundle(cipher, reader, destination);
            }
            return null;
        }

        private async Task UnpackEncodedBundle(ICipher cipher, BinaryReader reader, string destination)
        {
            var serializer = new BinarySerialization();
            var directoryManifestBytes = await ReadBinaryData(cipher, reader);
            var directoryManifest = serializer.Deserialize<DirectoryManifest>(directoryManifestBytes);

            foreach (DirectoryElement directoryElement in directoryManifest.Elements)
            {
                var newPath = directoryElement.SourceFullPath.Replace(directoryManifest.SourceRootPath, destination);

                if (directoryElement.IsDirectory && !Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                else
                {
                    var fileBytes = await ReadBinaryData(cipher, reader);
                    File.WriteAllBytes(newPath, fileBytes);
                }
            }
        }

        private async Task<byte[]> ReadBinaryData(ICipher cipher, BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var encodedData = reader.ReadBytes(length);
            var base64Data = await cipher.Decode(SymmetricCipherType.Aes, encodedData);
            var decompressedData = await CompressEngine.Decompress(Convert.FromBase64String(base64Data));
            return decompressedData;
        }
    }
}
