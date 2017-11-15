using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Contracts
{
  public interface IHierarchyMap
  {    
    string Path { get; set; }
    bool IsFile { get; set; }
    List<IHierarchyMap> Hierarchies { get; set; }
  }
}
