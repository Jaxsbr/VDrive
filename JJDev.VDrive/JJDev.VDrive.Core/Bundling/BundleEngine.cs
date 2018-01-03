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
        public object Compress(string source, string destination, ICipher cipher)
        {
            var directoryManifest = new DirectoryManifest(source);
            using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write))
            {
                var writer = new BinaryWriter(fileStream);
                GenerateEncodedBundle(cipher, directoryManifest, writer);
            }            
            return null;
        }
        
        public object Decompress(string source, string destination, ICipher cipher)
        {
            using (var fileStream = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                var reader = new BinaryReader(fileStream);
                UnpackEncodedBundle(cipher, reader, destination);
            }
            return null;
        }

        public object _Compress(string source, string destination, ICipher cipher)
        {            
            var serializer = new BinarySerialization();
            var outStream = new MemoryStream();
            var finalStream = new MemoryStream();
            var writer = new BinaryWriter(outStream);

            WriterManifestData(source, serializer, outStream, finalStream, writer);

            var compressedData = CompressEngine.Compress(finalStream.ToArray());
            var base64Data = Convert.ToBase64String(compressedData);
            var encodedData = cipher.Encode(SymmetricCipherType.Aes, base64Data);
            File.WriteAllBytes(destination, encodedData);

            return encodedData;
        }

        public object _Decompress(string source, string destination, ICipher cipher)
        {
            var serializer = new BinarySerialization();            
            var encodeData = File.ReadAllBytes(source);
            var base64Data = cipher.Decode(SymmetricCipherType.Aes, encodeData);
            var decodeData = Convert.FromBase64String(base64Data);
            var compressedData = CompressEngine.Decompress(decodeData);
            var inputStream = new MemoryStream(compressedData);
            var reader = new BinaryReader(inputStream);

            HierarchyMap manifest = ReadManifestData(serializer, reader);
            manifest.Hierarchies.ForEach(childHierarchy => CreateDataHierarchy(childHierarchy, destination));

            return manifest;
        }

        private static HierarchyMap ReadManifestData(BinarySerialization serializer, BinaryReader reader)
        {
            var manifestLength = reader.ReadInt32();
            var manifestData = reader.ReadBytes(manifestLength);
            var manifest = serializer.Deserialize<HierarchyMap>(manifestData);
            return manifest;
        }

        private HierarchyMap GetHierarchy(string source, string rootSource)
        {
            var hierarchyMap = new HierarchyMap() { Source = rootSource, Path = source, IsFile = false };
            var files = SystemIO.GetFiles(source);
            var folders = SystemIO.GetFolders(source);

            files.ForEach(file =>
            {
                hierarchyMap.Hierarchies.Add(new HierarchyMap()
                {
                    Source = rootSource,
                    Path = file,
                    IsFile = true,
                    Data = File.ReadAllBytes(file)
                });
            });

            folders.ForEach(folder =>
            {
                hierarchyMap.Hierarchies.Add(GetHierarchy(folder, rootSource));
            });

            return hierarchyMap;
        }

        private void CreateDataHierarchy(IHierarchyMap hierarchyMap, string destination)
        {
            var filePath = hierarchyMap.Path.Replace(hierarchyMap.Source, destination);
            if (hierarchyMap.IsFile)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(hierarchyMap.Data, 0, hierarchyMap.Data.Length);
                }
            }
            else if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                hierarchyMap.Hierarchies.ForEach(childHierarchy => CreateDataHierarchy(childHierarchy, destination));
            }
        }

        private void WriterManifestData(string source, BinarySerialization serializer, MemoryStream outStream, MemoryStream finalStream, BinaryWriter writer)
        {
            var manifest = GetHierarchy(source, source);
            var manifestData = serializer.Serialize(manifest);

            writer.Write(manifestData.Length);
            writer.Write(manifestData, 0, manifestData.Length);

            writer.Flush();
            outStream.Position = 0;
            outStream.CopyTo(finalStream);
            writer.Close();
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
