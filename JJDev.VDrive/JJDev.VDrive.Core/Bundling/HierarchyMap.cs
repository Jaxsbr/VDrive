using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Bundling
{
    [Serializable]
    public class HierarchyMap : IHierarchyMap
    {
        public string Source { get; set; }
        public string Path { get; set; }
        public bool IsFile { get; set; }
        public byte[] Data { get; set; }
        public List<IHierarchyMap> Hierarchies { get; set; } = new List<IHierarchyMap>();
    }
}
