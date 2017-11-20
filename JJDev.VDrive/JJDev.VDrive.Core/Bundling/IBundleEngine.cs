using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Bundling
{
  public interface IBundleEngine
  {
    object Compress(string source, string destination);
    object Decompress(string source, string destination);
  }
}
