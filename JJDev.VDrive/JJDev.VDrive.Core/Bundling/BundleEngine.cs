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

namespace JJDev.VDrive.Core.Bundling
{
    public class BundleEngine : IBundleEngine
    {
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
            var manifest = GetHierarchy(source).ToString();
            var filesInManifest = manifest.Split('\n').ToList();
            var manifestData = serializer.Serialize(manifest);            
            
            // NOTE: Add manifest size, then add manifest data.
            writer.Write(manifestData.Length);
            writer.Write(manifestData, 0, manifestData.Length);

            // NOTE: Ingore folders, serialize files in order read from manifest list.
            //       Add file size, then add file data.
            filesInManifest.ForEach(x =>
            {
                if (x.StartsWith("f "))
                {
                    var path = x.Substring(2, x.Length - 2);
                    var fileData = File.ReadAllBytes(path);
                    writer.Write(fileData.Length);
                    writer.Write(fileData, 0, fileData.Length);
                }
            });

            writer.Flush();
            outStream.Position = 0;
            outStream.CopyTo(finalStream);
            writer.Close();

            var base64Data = Convert.ToBase64String(finalStream.ToArray());      
            var encodedData = cipher.Encode(SymmetricCipherType.Aes, base64Data);
            File.WriteAllBytes(destination, encodedData);      

            return encodedData;
        }

        private HierarchyMap GetHierarchy(string source)
        {
            var hierarchyMap = new HierarchyMap() { Path = source, IsFile = false };
            var files = SystemIO.GetFiles(source);
            var folders = SystemIO.GetFolders(source);

            files.ForEach(file => { hierarchyMap.Hierarchies.Add(new HierarchyMap() { Path = file, IsFile = true }); });
            folders.ForEach(folder =>
            {
                hierarchyMap.Hierarchies.Add(GetHierarchy(folder));
            });

            return hierarchyMap;
        }


        // [Decompress]
        // Read content into stream
        // Read hirarchy map part and construct object
        // Read remaining stream into flat list of file content
        // Generate original directory structure at destination
        public object Decompress(string source, string destination)
        {
            throw new NotImplementedException();
        }
    }
}
