using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            //var bytes = ObjectToByteArray(manifest);
            
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
