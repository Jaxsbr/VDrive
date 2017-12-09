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
        // TEMP: Will be passed in by user of library
        private static readonly byte[] _key = new byte[] { 105, 195, 252, 185, 2, 140, 51, 126, 104, 229, 79, 123, 212, 18, 202, 2, 110, 30, 207, 111, 0, 244, 173, 234, 220, 14, 253, 178, 156, 52, 214, 127 };
        private static readonly byte[] _iv = new byte[] { 8, 68, 137, 198, 127, 127, 18, 72, 241, 104, 126, 253, 191, 17, 44, 132 };

        // [Compress]
        // Map hirarchy (serializeable object)
        // Append to stream
        // Append encoded file content to stream, disregard hirarchy
        // Write stream to file
        public object Compress(string source, string destination)
        {
            var cipher = new SymmetricAlgorithmCipher() { Key = _key, IV = _iv };
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

        // [Decompress]
    // Read content into stream
    // Read hirarchy map part and construct object
    // Read remaining stream into flat list of file content
    // Generate original directory structure at destination
        public object Decompress(string source, string destination)
        {
            var serializer = new BinarySerialization();
            var cipher = new SymmetricAlgorithmCipher() { Key = _key, IV = _iv };  
            var encodeData = File.ReadAllBytes(source);            
            var base64Data = cipher.Decode(SymmetricCipherType.Aes, encodeData);
            var decodeData = Convert.FromBase64String(base64Data);
            var compressedData = CompressEngine.Decompress(decodeData);
            var inputStream = new MemoryStream(compressedData);
            var reader = new BinaryReader(inputStream);

            var manifestLength = reader.ReadInt32();
            var manifestData = reader.ReadBytes(manifestLength);
            var manifest = serializer.Deserialize<HierarchyMap>(manifestData);
            manifest.Hierarchies.ForEach(childHierarchy => CreateDataHierarchy(childHierarchy, destination));
   
            return null;
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
                    fileStream.Write(hierarchyMap.Data, 0, hierarchyMap.Data.Length)    ;
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
    }
}
