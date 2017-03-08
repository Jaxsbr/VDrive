using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace JJDev.VDrive.Core
{
    public class Class1
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DefineDosDevice(int flags, string devname, string path);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int QueryDosDevice(string devname, StringBuilder buffer, int bufSize);

        
        public static List<string> AvailableDrives()
        {            
            var drives = new List<string>();
            var letters = GenerateDriveLetters();


            return drives;            
        }

        public static List<string> GenerateDriveLetters()
        {
            var letters = new List<string>();
            var range = 26;
            var upperCase = 65; // 65 upper | 97 lower
            char letter;

            for (int i = 0; i < range; i++)
            {
                // Convert alphabet index to upper case drive letter.
                letter = (Char)(i + upperCase);
                letters.Add($"{letter.ToString()}:");
            }
            
            return letters;
        }
    }
}
