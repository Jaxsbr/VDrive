using JJDev.VDrive.Core.Bundling;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Tests
{
    [TestFixture]
    public class BundleTests
    {
        [Test]
        public void CompressTest_GivenSource_ShouldCompressToDestination()
        {
            var sut = new BundleEngine();
            sut.Compress(@"C:\test", @"C:\users\jacobr\desktop\enc.txt");
        }

        [Test]
        public void DecompressTest_GivenSource_ShouldDecompressToDestination()
        {
          var sut = new BundleEngine();      
          sut.Decompress(@"C:\users\jacobr\desktop\enc.txt", @"C:\users\jacobr\desktop");
        }
  }
}
