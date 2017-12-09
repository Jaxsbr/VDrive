using JJDev.VDrive.Core.Ciphers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Desktop
{
    public class CipherKeys
    {        
        public static readonly byte[] Key = new byte[] { 105, 195, 252, 185, 2, 140, 51, 126, 104, 229, 79, 123, 212, 18, 202, 2, 110, 30, 207, 111, 0, 244, 173, 234, 220, 14, 253, 178, 156, 52, 214, 127 };
        public static readonly byte[] IV = new byte[] { 8, 68, 137, 198, 127, 127, 18, 72, 241, 104, 126, 253, 191, 17, 44, 132 };

        public static SymmetricAlgorithmCipher GetCipher()
        {
            var cipher = new SymmetricAlgorithmCipher() { Key = Key, IV = IV };
            return cipher;
        }
    }
}
