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
    void WriteBundle(string source, string destination, ICipher cipher);
    void ReadBundle(string source, string destination, ICipher cipher);
  }
}
