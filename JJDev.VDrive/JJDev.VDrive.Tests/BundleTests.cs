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
    public void Test()
    {
      var sut = new BundleEngine();
      sut.Compress(@"C:\workspace\Personal\VDrive", "");
    }
  }
}
