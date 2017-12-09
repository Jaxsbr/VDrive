using JJDev.VDrive.Core.Bundling;
using JJDev.VDrive.Core.Ciphers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Tests
{
    [TestFixture]
    public class BundleTests
    {
        private ICipher GetCipher()
        {
            var cipher = new SymmetricAlgorithmCipher()
            {
                Key = new byte[] { 105, 195, 252, 185, 2, 140, 51, 126, 104, 229, 79, 123, 212, 18, 202, 2, 110, 30, 207, 111, 0, 244, 173, 234, 220, 14, 253, 178, 156, 52, 214, 127 },
                IV = new byte[] { 8, 68, 137, 198, 127, 127, 18, 72, 241, 104, 126, 253, 191, 17, 44, 132 }
            };
            return cipher;
        }

        private string GetDesktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void Init(List<string> requiredDirectories)
        {
            requiredDirectories.ForEach(path =>
            {
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            });
        }

        private void Cleanup(List<string> cleanupDirectories)
        {
            cleanupDirectories.ForEach(path => Directory.Delete(path));
        }

        [Test]
        public void CompressTest_GivenSource_ShouldCompressToDestination()
        {
            //Init(new List<string>() { @"C:\test" });

            var sut = new BundleEngine();
            var cipher = GetCipher();
            sut.Compress(@"C:\test", GetDesktopPath() + @"\enc.txt", cipher);

            //Cleanup(new List<string>() { @"C:\test" });
        }

        [Test]
        public void DecompressTest_GivenSource_ShouldDecompressToDestination()
        {
            //Init(new List<string>() { GetDesktopPath() + @"\testOutput" });

            var sut = new BundleEngine();
            var cipher = GetCipher();
            sut.Decompress(GetDesktopPath() + @"\enc.txt", GetDesktopPath() + @"\testOutput", cipher);

            //Cleanup(new List<string>() { GetDesktopPath() + @"\testOutput" });
        }
    }
}
