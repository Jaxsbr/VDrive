using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Core.Bundling
{
  [Serializable]
  public class DirectoryManifest
  {
    public List<DirectoryElement> Elements { get; set; }
    public string SourceRootPath { get; private set; }


    public DirectoryManifest(string sourceRootPath)
    {
      SourceRootPath = sourceRootPath;
      Elements = new List<DirectoryElement>();
      PopulateManifestElements(SourceRootPath);
    }


    private void PopulateManifestElements(string path)
    {
      PopulateManifestFileElements(path);
      PopulateManifestFolderElements(path);
    }

    private void PopulateManifestFileElements(string path)
    {
      var files = SystemIO.GetFiles(path);
      files.ForEach(file => { Elements.Add(new DirectoryElement(file, false)); });
    }

    private void PopulateManifestFolderElements(string path)
    {
      var folders = SystemIO.GetFolders(path);
      folders.ForEach(folder =>
      {
        Elements.Add(new DirectoryElement(folder, true));
        PopulateManifestElements(folder);
      });
    }
  }
}
