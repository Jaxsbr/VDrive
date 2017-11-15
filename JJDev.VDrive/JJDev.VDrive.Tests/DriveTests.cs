using JJDev.VDrive.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Tests
{
  [TestFixture]
  public class DriveTests
  {
    [TestCase("E:")]
    [TestCase("c:")]
    public void IsDriveInUseTest_GivenValidDriveLetter_ShouldReturnTrue(string input)
    {
      var expected = true;      

      var actual = DriveMaster.IsDriveInUse(input);

      Assert.AreEqual(expected, actual);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(@"c:\")]
    [TestCase("asdv:")]
    [TestCase("1:")]
    [TestCase("z:")]
    public void IsDriveInUseTest_GivenInvalidDriveLetter_ShouldReturnFalse(string input)
    {
      var expected = false;

      var actual = DriveMaster.IsDriveInUse(input);

      Assert.AreEqual(expected, actual);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(@"c:\")]
    [TestCase("asdv:")]
    [TestCase("1:")]
    public void IsDriveLetterValidTest_GivenInvalidDriveLetter_ShouldReturnFalse(string input)
    {
      var expected = false;

      var actual = DriveMaster.IsDriveLetterValid(input);

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void MountTest_GivenDriveLetter_ShouldReturnTrue()
    {
      var expected = true;

      DriveMaster.Mount(@"z:", @"c:\test\");
      var actual = System.IO.Directory.Exists(System.IO.Path.GetPathRoot("z:\\"));

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void DismountTest_GivenDriveLetter_ShouldReturnTrue()
    {
      var expected = true;

      //DriveMaster.Dismount("z:");
      var actual = !System.IO.Directory.Exists(System.IO.Path.GetPathRoot("z:\\"));

      Assert.AreEqual(expected, actual);
    }
  }
}
