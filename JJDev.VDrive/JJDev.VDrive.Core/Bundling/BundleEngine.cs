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

namespace JJDev.VDrive.Core.Bundling
{
    public class BundleEngine : IBundleEngine
    {
        // [Compress]
        // Map hirarchy (serializeable object)
        // Append to stream
        // Append encoded file content to stream, disregard hirarchy
        // Write stream to file

        public object Compress(string source, string destination)
        {
            var serializer = new BinarySerialization();
            var outStream = new MemoryStream();
            var writer = new BinaryWriter(outStream);
            var manifest = GetHierarchy(source).ToString();
            var filesInManifest = manifest.Split('\n').ToList();
            var manifestData = serializer.Serialize(manifest);            
            
            // NOTE: Add manifest size, then add manifest data
            writer.Write(manifestData.Length);
            writer.Write(manifestData, 0, manifestData.Length);

            // NOTE: Ingore folders, serialize files in order read from manifest list.
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

            // TODO:            
            // - Encrypt source content stream
            // - Write encrypted stream to file

            return null;
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
