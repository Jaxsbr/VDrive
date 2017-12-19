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
    private string _sourceRootPath;


    public DirectoryManifest(string sourceRootPath)
    {
      _sourceRootPath = sourceRootPath;
      Elements = new List<DirectoryElement>();
      PopulateManifestElements(_sourceRootPath);
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
        Elements.Add(new DirectoryElement(folder, false));
        PopulateManifestElements(folder);
      });
    }
  }
}
