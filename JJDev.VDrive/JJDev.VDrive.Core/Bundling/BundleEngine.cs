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
        // TODO: Progress updates
        // While reading/writing bundles, the directoryElements to be processed are know to us.
        // This means we can track how many directortyElements have been and still need to be processed.
        // Implement a mechanism for tracking and also raising progress update events.

        public delegate void StatusUpdateHandler(object obj, ProgressEventArgs args);
        public event StatusUpdateHandler ProgressChanged;

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
          var progressIndex = 0;
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

            progressIndex++;
            RaiseProgressUpdateEvent(progressIndex, directoryManifest.Elements.Count);
          }

          writer.Flush();
          writer.BaseStream.Position = 0;
          writer.Close();
        }

        private void WriteBinaryData(ICipher cipher, BinaryWriter writer, byte[] bytes, int position = 0)
        {
            // TODO:
            // When large files are encountered +-250mb, we run out of memory.
            // In an attempt to fix this issue, the logic was changed to split the data stream in to chunks.
            // Each chuck is compressed and encode and then written to the file stream.
            // # This issue here is that we need to know the combined byte size after compression and encoding
            // # before writting any of the actual file data.
            // # The code attempts to insert the total byte size afterwards, but this seems fail on decompile.


            var maxBufferSize = 20480;
            var chunks = new List<byte[]>();
            var rawBytesLength = bytes.Length;
            var totalEncodedLength = 0;
            var startPosition = writer.BaseStream.Position;

            using (var stream = new MemoryStream(bytes))
            { 
                using (var reader = new BinaryReader(stream))
                {
                    long remainingBytesLength = rawBytesLength;
                    while (remainingBytesLength > 0)
                    {
                        var bufferSize = remainingBytesLength > maxBufferSize ? maxBufferSize : (int)remainingBytesLength;
                        var buffer = new byte[bufferSize];
                        reader.Read(buffer, 0, bufferSize);

                        var compressedData = CompressEngine.Compress(buffer);
                        var base64Data = Convert.ToBase64String(compressedData);
                        var encodedData = cipher.Encode(SymmetricCipherType.Aes, base64Data);

                        chunks.Add(encodedData);
                        totalEncodedLength += bufferSize;
                        remainingBytesLength = rawBytesLength - stream.Position;

                        writer.Write(encodedData, 0, encodedData.Length);
                    }
                }
            }

            writer.BaseStream.Position = startPosition;
            writer.Write(totalEncodedLength);
            writer.BaseStream.Seek(0, SeekOrigin.End);
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
            var progressIndex = 0;
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

                progressIndex++;
                RaiseProgressUpdateEvent(progressIndex, directoryManifest.Elements.Count);
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


        private void RaiseProgressUpdateEvent(int progressIndex, int maxProgress)
        {
            var progressEventArgs = new ProgressEventArgs(progressIndex, maxProgress);
            ProgressChanged?.Invoke(this, progressEventArgs);
        }
    }
}
