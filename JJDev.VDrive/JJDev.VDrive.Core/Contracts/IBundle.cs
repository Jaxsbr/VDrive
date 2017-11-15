using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Contracts
{
  public interface IBundle
  {
    IHierarchyMap HierarchyMap { get; }
    byte[] Content { get; }
  }
}
