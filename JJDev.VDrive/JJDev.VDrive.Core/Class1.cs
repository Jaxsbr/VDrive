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
    }
}
