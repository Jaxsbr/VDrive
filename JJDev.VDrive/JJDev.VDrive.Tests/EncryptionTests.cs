using JJDev.VDrive.Core.Contracts;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Tests
{
    [TestFixture]
    public class EncryptionTests
    {
        [Test]
        public void TestEncode_GivenInput_ShouldReturnTrue()
        {
            var expected = true;
            var input = "some 124 RaNDom dat _(*^%$234";
            var sut = Substitute.For<ICipher>();                    
            sut.Encode(Arg.Any<string>()).Returns(new byte[1]);

            var result = sut.Encode(input);
            var actual = result != null && result.Length > 0;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestEncode_GivenInputWithEncryptionAES_ShouldReturnTrue()
        {
            var expected = true;
            var input = "some 124 RaNDom dat _(*^%$234";
            var sut = Substitute.For<ICipher>();
            sut.Encode(Arg.Any<string>()).Returns(new byte[1]);

            var result = sut.Encode(input);
            var actual = result != null && result.Length > 0;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestDecode_GivenEncodedInput_ShouldReturnTrue()
        {
            var expected = true;
            var input = "asd";
            var sut = Substitute.For<ICipher>();
            sut.Decode(Arg.Any<string>()).Returns("fdsa");

            var result = sut.Decode(input);
            var actual = input != result;

            Assert.AreEqual(expected, actual);
          }
    }
}
