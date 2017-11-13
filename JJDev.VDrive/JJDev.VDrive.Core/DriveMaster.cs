using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace JJDev.VDrive.Core
{
    public class DriveMaster
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DefineDosDevice(int flags, string driveLetter, string drivePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int QueryDosDevice(string letter, StringBuilder buffer, int capacity);

        public static void MapDrive(string driveLetter, string drivePath)
        {
            if (!DefineDosDevice(0, driveLetter, drivePath)) { throw new Win32Exception(); }
        }

        public static void UnMapDrive(string driveLetter)
        {
            var directory = new DirectoryInfo(driveLetter);
            if (directory == null) { return; }
            if (!DefineDosDevice(2, driveLetter, null)) { throw new Win32Exception(); }
        }

        public static List<string> AvailableDrives(bool win32 = false)
        {
            var drives = new List<string>();
            if (!win32)
            {
                // Gets mounted disk partitions, Not physical drives.
                drives = Directory.GetLogicalDrives().ToList();
                drives = drives.Select(d => d.Replace("\\", string.Empty)).ToList();
                return drives;
            }

            var letters = GenerateDriveLetters();
            foreach (var letter in letters)
            {
                if (DriveValid(letter))
                {
                    drives.Add(letter);
                }
            }
            return drives;
        }

        public static List<string> GenerateDriveLetters()
        {
            var letters = new List<string>();
            var range = 26;
            var upperCase = 65; // 65 upper | 97 lower
            string letter;

            for (int i = 0; i < range; i++)
            {
                // Convert alphabet index to upper case drive letter.
                var letterVal = (Char)(i + upperCase);
                letter = $"{letterVal.ToString()}:";
                letters.Add(letter);
            }

            return letters;
        }

        public static bool DriveValid(string letter)
        {
            var buffer = new StringBuilder(256);
            if (QueryDosDevice(letter, buffer, buffer.Capacity) == 0)
            {
                var error = Marshal.GetLastWin32Error();
                if (error == 2)
                {
                    return false;
                }
                throw new Win32Exception();
            }
            return true;
        }
    }
}
