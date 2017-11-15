using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core
{
  public class SystemIO
  {
    public static List<string> GetFiles(string path)
    {
      try { return Directory.GetFiles(path).ToList(); }
      catch (Exception) { }
      return new List<string>();
    }

    public static List<string> GetFolders(string path)
    {
      try { return Directory.GetDirectories(path).ToList(); }
      catch (Exception) { }
      return new List<string>();
    }
  }
}
