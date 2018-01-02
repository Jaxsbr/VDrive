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

        public object Decompress(string source, string destination, ICipher cipher)
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

        private void GenerateEncodedBundle(DirectoryManifest directoryManifest, BinaryWriter writer)
        {
            // TODO:
            // First encode and compress bytes
            // Then calculate content length
            // Then write length and data

            var serializer = new BinarySerialization();
            var directoryManifestBytes = serializer.Serialize(directoryManifest);
            
            WriteBinaryData(writer, directoryManifestBytes);

            foreach (DirectoryElement directoryElement in directoryManifest.Elements)
            {
                var directoryElementBytes = serializer.Serialize(directoryElement);
                WriteBinaryData(writer, directoryElementBytes);                

                if (!directoryElement.IsDirectory)
                {
                    var fileBytes = directoryElement.GetFileData();
                    WriteBinaryData(writer, fileBytes);                    
                }
            }

            writer.Flush();
            writer.BaseStream.Position = 0;            
            writer.Close();
        }

        private void WriteBinaryData(BinaryWriter writer, byte[] bytes, int position = 0)
        {
            writer.Write(bytes.Length);
            writer.Write(bytes, position, bytes.Length);
        }

        private void UnpackEncodedBundle()
        {
            // TODO:
        }
    }
}
