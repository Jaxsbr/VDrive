using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JJDev.VDrive.Core.Serialization;

namespace JJDev.VDrive.Core.Bundling
{
  [Serializable]
  public class DirectoryElement
  {
    public Guid ID { get; private set; }    
    public string SourceFullPath { get; private set; }
    public bool IsDirectory { get; private set; }    


    public DirectoryElement(string sourceFullPath, bool isDirectory)
    {
      ID = Guid.NewGuid();
      SourceFullPath = sourceFullPath;
      IsDirectory = isDirectory;
    }


    public byte[] GetFileData()
    {
      if (IsDirectory) { return null; }
      return System.IO.File.ReadAllBytes(SourceFullPath);
    }
  }
}
