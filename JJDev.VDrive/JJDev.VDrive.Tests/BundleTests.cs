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
        private ICipher Cipher
        {
            get 
            {
                return new SymmetricAlgorithmCipher()
                {
                    Key = new byte[] { 105, 195, 252, 185, 2, 140, 51, 126, 104, 229, 79, 123, 212, 18, 202, 2, 110, 30, 207, 111, 0, 244, 173, 234, 220, 14, 253, 178, 156, 52, 214, 127 },
                    IV = new byte[] { 8, 68, 137, 198, 127, 127, 18, 72, 241, 104, 126, 253, 191, 17, 44, 132 }
                };
            }                        
        }

        private string _desktopPath;
        private string DesktopPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_desktopPath))
                {
                    _desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                return _desktopPath;
            }
        }


        [SetUp]
        public void Init()
        {
            var path = @"C:\test";
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

            path = DesktopPath + @"\testOutput";
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
        }

        [TearDown]
        public void Cleanup()
        {
            var path = @"C:\test";
            Directory.Delete(path);

            path = DesktopPath + @"\testOutput";
            Directory.Delete(path);
        }


        [Test]
        public void CompressTest_GivenSource_ShouldCompressToDestination()
        {
            var sut = new BundleEngine();
            var cipher = Cipher;
            sut.Compress(@"C:\test", DesktopPath + @"\enc.txt", cipher);
        }

        [Test]
        public void DecompressTest_GivenSource_ShouldDecompressToDestination()
        {
            var sut = new BundleEngine();
            var cipher = Cipher;
            sut.Decompress(DesktopPath + @"\enc.txt", DesktopPath + @"\testOutput", cipher);
        }
    }
}
