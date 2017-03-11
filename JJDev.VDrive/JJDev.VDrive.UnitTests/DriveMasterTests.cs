using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using JJDev.VDrive.Core;

namespace JJDev.VDrive.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetDriveLettersTest_ShouldReturn_AllAlphabetLetterInSequence()
        {
            // Arrange
            var firstLetter = "A:";
            var lastLetter = "Z:";            
            List<string> result;

            // Act
            result = DriveMaster.GenerateDriveLetters();

            // Assert
            Assert.IsTrue(result.Count == 26, "Letters count is incorrect");
            Assert.IsTrue(result[0] == firstLetter, "First letter doesn't match alphabet");
            Assert.IsTrue(result[25] == lastLetter, "Last letter doesn't match alphabet");
        }

        [TestMethod]
        public void GetDriveLettersTest_ShouldNot_ContainForwardSlash()
        {
            // Arrange
            var forwardSlash = "/";
            var results = new List<string>();
            var errorCount = 0;

            // Act            
            results = DriveMaster.GenerateDriveLetters();
            results.ForEach(d => { if (d.Contains(forwardSlash)) errorCount++; });

            // Assert
            Assert.IsTrue(errorCount == 0, "Forward slash must not be present in generated drive letters");
        }

        [TestMethod]
        public void AvailableDrivesTest_ShouldReturn_LogicalDrivesWithoutForwardSlash()
        {
            // Arrange
            var forwardSlash = "/";
            var results = new List<string>();
            var errorCount = 0;

            // Act            
            results = DriveMaster.AvailableDrives(false);
            results.ForEach(d => { if (d.Contains(forwardSlash)) errorCount++; });

            // Assert
            Assert.IsFalse(errorCount > 0, "Forward slash not removed from logical drives");
        }

        [TestMethod]
        public void DriveValidTest_ShouldReturn_CDrive()
        {
            // Save assumption as most pcs have a C drive.
            // Test won't pass if no C drive exists

            // Arrange
            var drive = "C:";
            var result = false;

            // Act
            result = DriveMaster.DriveValid(drive);

            // Assert
            Assert.IsTrue(result, "Valid C drive not found");
        }

        [TestMethod]
        public void DriveValidTest_ShouldNotReturn_ZDrive()
        {
            // This test assumes there is no Z drive.
            // Although not impossible, unlikely.
            // Test won't pass if Z drive exists

            // Arrange
            var drive = "Z:";
            var result = false;

            // Act
            result = DriveMaster.DriveValid(drive);

            // Assert
            Assert.IsTrue(!result, "Invalid Z drive should not have been found");
        }
    }
}
