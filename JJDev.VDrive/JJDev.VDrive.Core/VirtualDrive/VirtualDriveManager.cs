using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace JJDev.VDrive.Core.VirtualDrive
{
  public static class VirtualDriveManager
  {
    public static void Mount(string networkPath, string virtualDriveLetter = "z")
    {
      try
      {
        var command = $"New-PSDrive -Persist -Name \"{virtualDriveLetter}\" -PSProvider \"FileSystem\" -Root \"{networkPath}\"";        
        ExecuteCommand(command);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public static void Dismount(string virtualDriveLetter = "z")
    {
      try
      {
        var command = $"Remove-PSDrive -Name \"{virtualDriveLetter}\" -PSProvider \"FileSystem\" -Force";
        //var command = $"net use {virtualDriveLetter.Replace(":", "")} /delete";
        ExecuteCommand(command);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private static void ExecuteCommand(string command)
    {
      ExecutePowershellCommand(new string[] { command });
    }

    private static void ExecutePowershellCommand(string[] commands, string workingDirectory = "")
    {
      using (var powerShell = PowerShell.Create())
      {
        if (string.IsNullOrWhiteSpace(workingDirectory))
        {
          powerShell.AddScript($"cd {workingDirectory}");
        }

        commands.ToList().ForEach(c => powerShell.AddScript(c));

        var results = powerShell.Invoke();
      }
    }
  }
}
