using JJDev.VDrive.Core.Ciphers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Bundling
{
  public interface IBundleEngine
  {
    Task WriteBundle(string source, string destination, ICipher cipher);
    Task ReadBundle(string source, string destination, ICipher cipher);
  }
}
