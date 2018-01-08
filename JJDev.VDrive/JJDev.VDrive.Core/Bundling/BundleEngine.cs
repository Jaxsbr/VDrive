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
        public delegate void StatusUpdateHandler(object obj, ProgressEventArgs args);
        public event StatusUpdateHandler ProgressChanged;
        private readonly int _maxBufferSize = 1024 * 1024; // 1 mb

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
            if (bytes.Length > _maxBufferSize)
            {
                WriteLargeBinaryData(cipher, writer, bytes);
                return;
            }

            var compressedData = CompressEngine.Compress(bytes);
            var base64Data = Convert.ToBase64String(compressedData);
            var encodedData = cipher.Encode(SymmetricCipherType.Aes, base64Data);

            writer.Write(encodedData.Length);
            writer.Write(encodedData, position, encodedData.Length);
        }

        private void WriteLargeBinaryData(ICipher cipher, BinaryWriter writer, byte[] bytes, int position = 0)
        {
            // TODO:
            // When large files are encountered +-250mb, we run out of memory.
            // In an attempt to fix this issue, the logic was changed to split the data stream in to chunks.
            // Each chuck is compressed and encode and then written to the file stream.
            // # This issue here is that we need to know the combined byte size after compression and encoding
            // # before writting any of the actual file data.            

            // SOLUTION 1:
            // We expect the majority of files to not be large enough to cause the memory issue.
            // Thus we have this method handle the expection(large files).            
            // Due to needing the total file byte size after compression and encoding, we perform the logic
            // twice, only writing to file on the second run.
            // We accept the performance knock temporarily as this should only be the exception.
                        
            var rawBytesLength = bytes.Length;
            var totalEncodedLength = 0;            

            for (int i = 0; i < 2; i++)
            {
                if (i == 1) { writer.Write(totalEncodedLength); }

                using (var stream = new MemoryStream(bytes))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        long remainingBytesLength = rawBytesLength;
                        while (remainingBytesLength > 0)
                        {
                            var bufferSize = remainingBytesLength > _maxBufferSize ? _maxBufferSize : (int)remainingBytesLength;
                            var buffer = new byte[bufferSize];
                            reader.Read(buffer, 0, bufferSize);

                            var compressedData = CompressEngine.Compress(buffer);
                            var base64Data = Convert.ToBase64String(compressedData);
                            var encodedData = cipher.Encode(SymmetricCipherType.Aes, base64Data);
                            
                            totalEncodedLength += bufferSize;
                            remainingBytesLength = rawBytesLength - stream.Position;

                            if (i == 1) { writer.Write(encodedData, 0, encodedData.Length); }
                        }
                    }
                } 
            }            
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

                if (directoryElement.IsDirectory && !Directory.Exists(newPath)) { Directory.CreateDirectory(newPath); }
                else { WriteFileData(cipher, reader, newPath); }

                progressIndex++;
                RaiseProgressUpdateEvent(progressIndex, directoryManifest.Elements.Count);
            }
        }

        private void WriteFileData(ICipher cipher, BinaryReader reader, string filePath)
        {
            var length = reader.ReadInt32();
            if (length > _maxBufferSize) { WriteChunkedFileData(cipher, reader, filePath, length); }
            else { File.WriteAllBytes(filePath, ReadBinaryDataFromLength(cipher, reader, length)); }            
        }

        private void WriteChunkedFileData(ICipher cipher, BinaryReader reader, string filePath, int length)
        {
            var remainingLength = length;
            using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                while (remainingLength > 0)
                {
                    var bufferSize = remainingLength > _maxBufferSize ? _maxBufferSize : (int)remainingLength;
                    var buffer = new byte[bufferSize];
                    reader.Read(buffer, 0, bufferSize);

                    var base64DecodedData = cipher.Decode(SymmetricCipherType.Aes, buffer);
                    var decodedData = Convert.FromBase64String(base64DecodedData);
                    var decompressedData = CompressEngine.Decompress(decodedData);

                    remainingLength -= bufferSize;
                    fileStream.Write(decompressedData, 0, decompressedData.Length);
                }
            }
        }

        private byte[] ReadBinaryData(ICipher cipher, BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return ReadBinaryDataFromLength(cipher, reader, length);
        }

        private byte[] ReadBinaryDataFromLength(ICipher cipher, BinaryReader reader, int length)
        {            
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
