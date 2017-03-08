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
        public void TestMethod1()
        {
            // Arrange
            var firstLetter = "A:";
            var lastLetter = "Z:";            
            List<string> result;

            // Act
            result = Class1.GenerateDriveLetters();

            // Assert
            Assert.IsTrue(result.Count == 26, "Letters count is incorrect");
            Assert.IsTrue(result[0] == firstLetter, "First letter doesn't match alphabet");
            Assert.IsTrue(result[25] == lastLetter, "Last letter doesn't match alphabet");
        }
    }
}
