using JJDev.VDrive.Core.Ciphers;
using JJDev.VDrive.Core.Contracts;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JJDev.VDrive.Tests
{
  [TestFixture]
  public class CipherTests
  {
    private static ICipher Setup_GetCipher()
    {
      var cipher = Substitute.For<ICipher>();
      Setup_AddKeyAndIV(cipher);
      return cipher;
    }

    private static void Setup_AddKeyAndIV(ICipher cipher)
    {
      cipher.Key = Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");
      cipher.IV = Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");
    }

    [Test]
    public void TestEncode_GivenEmptyInput_ShouldReturnEmptyByteArray()
    {
      var expected = new byte[0];
      var cipher = Setup_GetCipher();
      cipher.Encode(Arg.Any<SymmetricCipherType>(), Arg.Any<string>()).Returns(new byte[0]);

      var actual = cipher.Encode(SymmetricCipherType.Aes, string.Empty);

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDecode_GivenEmptyInput_ShouldReturnEmptyString()
    {
      var expected = string.Empty;
      var cipher = Setup_GetCipher();
      cipher.Decode(Arg.Any<SymmetricCipherType>(), Arg.Any<byte[]>()).Returns(string.Empty);

      var actual = cipher.Decode(SymmetricCipherType.Aes, new byte[0]);

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestAesEncode_GivenInput_ShouldReturnByteArray()
    {
      var expected = new byte[] { 182, 83, 183, 192, 122, 91, 118, 135, 243, 84, 60, 97, 134, 141, 227, 211 };
      var cipher = Substitute.For<SymmetricAlgorithmCipher>();
      Setup_AddKeyAndIV(cipher);

      var actual = cipher.Encode(SymmetricCipherType.Aes, "content777");

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestAesDecode_GivenInput_ShouldReturnByteArray()
    {
      var expected = "content777";
      var cipher = Substitute.For<SymmetricAlgorithmCipher>();
      Setup_AddKeyAndIV(cipher);

      var actual = cipher.Decode(SymmetricCipherType.Aes, 
        new byte[] { 182, 83, 183, 192, 122, 91, 118, 135, 243, 84, 60, 97, 134, 141, 227, 211 });

      Assert.AreEqual(expected, actual);
    }
  }
}
