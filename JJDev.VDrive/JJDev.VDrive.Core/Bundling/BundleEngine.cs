﻿using NSubstitute;
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
        // TODO: Progress updates
        // While reading/writing bundles, the directoryElements to be processed are know to us.
        // This means we can track how many directortyElements have been and still need to be processed.
        // Implement a mechanism for tracking and also raising progress update events.


        public void WriteBundle(string source, string destination, ICipher cipher)
        {
            var directoryManifest = new DirectoryManifest(source);
            using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write))
            {
                var writer = new BinaryWriter(fileStream);
                GenerateEncodedBundle(cipher, directoryManifest, writer);
            }            
        }

        private void GenerateEncodedBundle(ICipher cipher, DirectoryManifest directoryManifest, BinaryWriter writer)
        {
          var serializer = new BinarySerialization();
          var directoryManifestBytes = serializer.Serialize(directoryManifest);
          WriteBinaryData(cipher, writer, directoryManifestBytes);

          foreach (DirectoryElement directoryElement in directoryManifest.Elements)
          {
            if (!directoryElement.IsDirectory)
            {
              var fileBytes = directoryElement.GetFileData();
              WriteBinaryData(cipher, writer, fileBytes);
            }
          }

          writer.Flush();
          writer.BaseStream.Position = 0;
          writer.Close();
        }

        private void WriteBinaryData(ICipher cipher, BinaryWriter writer, byte[] bytes, int position = 0)
        {
          var compressedData = CompressEngine.Compress(bytes);
          var base64Data = Convert.ToBase64String(compressedData);
          var encodedData = cipher.Encode(SymmetricCipherType.Aes, base64Data);

          writer.Write(encodedData.Length);
          writer.Write(encodedData, position, encodedData.Length);
        }


        public void ReadBundle(string source, string destination, ICipher cipher)
        {
            using (var fileStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                var reader = new BinaryReader(fileStream);
                UnpackEncodedBundle(cipher, reader, destination);
            }
        }

        private void UnpackEncodedBundle(ICipher cipher, BinaryReader reader, string destination)
        {
            var serializer = new BinarySerialization();
            var directoryManifestBytes = ReadBinaryData(cipher, reader);
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
                    var fileBytes = ReadBinaryData(cipher, reader);
                    File.WriteAllBytes(newPath, fileBytes);
                }
            }
        }

        private byte[] ReadBinaryData(ICipher cipher, BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var encodedData = reader.ReadBytes(length);
            var base64Data = cipher.Decode(SymmetricCipherType.Aes, encodedData);
            var decompressedData = CompressEngine.Decompress(Convert.FromBase64String(base64Data));
            return decompressedData;
        }
    }
}
