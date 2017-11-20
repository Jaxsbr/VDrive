using JJDev.VDrive.Core.Contracts;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

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
            var manifest = GetHierarchy(source).ToString();
            var bytes = ObjectToByteArray(manifest);
            
            // TODO:
            // - Encrypt bytes into stream
            // - Encrypt source content into stream
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

        private byte[] ObjectToByteArray(object obj)
        {
            // TODO:
            // Extract logic, create serizable types(binary, xml, protobuffers)

            if (obj == null) { return null; }
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        private object ByteArrayToObject(byte[] data)
        {
            // TODO:
            // Extract logic, create serizable types(binary, xml, protobuffers)

            object obj = null;
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                obj = binaryFormatter.Deserialize(memoryStream);
            }
            return obj;
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
