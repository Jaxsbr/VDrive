using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core
{
  public static class UNCHelper
  {
    public static string GetUNCPathFromPath(string path)
    {
      var unc = $@"\\{Environment.MachineName}\{path.Replace(":", "$")}";
      return unc;
    }
  }
}
