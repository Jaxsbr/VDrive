using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace JJDev.VDrive.Core
{
    public class DriveMaster
    {
        #region Win32
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DefineDosDevice(int flags, string driveLetter, string drivePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int QueryDosDevice(string letter, StringBuilder buffer, int capacity); 
        #endregion

        public static void Mount(string driveLetter, string drivePath)
        {
            if (!DefineDosDevice(0, driveLetter, drivePath)) { throw new Win32Exception(); }
        }

        public static void Dismount(string driveLetter)
        {
            var directory = new DirectoryInfo(driveLetter);
            if (directory == null) { return; }
            if (!DefineDosDevice(2, driveLetter, null)) { throw new Win32Exception(); }
        }

        public static bool IsDriveLetterValid(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return false; }
            if (input.Length > 2) { return false; }
            var numericStart = "01234567898".ToList().Any(x => x == input[0]);
            if (numericStart) { return false; }
            return true;
        }

        public static List<string> GetAvailableDriveLetters(bool lowercase = true)
        {
            var letters = new List<string>();
            var range = 26;
            var upperCase = lowercase ? 65 : 97;
            var letter = string.Empty;

            for (int i = 0; i < range; i++)
            {                
                var letterValue = (Char)(i + upperCase);
                letter = $"{letterValue.ToString()}:";
                if (!IsDriveInUse(letter) && IsDriveLetterValid(letter)) { letters.Add(letter); }
            }

            return letters;
        }

        public static bool IsDriveInUse(string letter)
        {            
            var buffer = new StringBuilder(256);
            var result = QueryDosDevice(letter, buffer, buffer.Capacity);
            return result == 0 ? false : true;
        }
    }
}
