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
    public string Path { get; set; }
    public bool IsFile { get; set; }
    public List<IHierarchyMap> Hierarchies { get; set; } = new List<IHierarchyMap>();

    public override string ToString()
    {
      var value = string.Empty;
      GetPaths(this).ForEach(x => value += (string.IsNullOrWhiteSpace(value) ? string.Empty : "\n") + x);
      return value;
    }

    private List<string> GetPaths(IHierarchyMap hierarchyMap)
    {
      var paths = new List<string>();

      hierarchyMap.Hierarchies.ForEach(x =>
      {
        paths.Add((x.IsFile ? "f " : "d ") + x.Path);
        paths.AddRange(GetPaths(x));
      });

      return paths;
    }
  }
}
