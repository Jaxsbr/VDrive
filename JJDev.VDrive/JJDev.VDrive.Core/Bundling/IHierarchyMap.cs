using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Bundling
{
  public interface IHierarchyMap
  {    
    string Source { get; set; }
    string Path { get; set; }
    bool IsFile { get; set; }
    byte[] Data { get; set; }
    List<IHierarchyMap> Hierarchies { get; set; }
  }
}
