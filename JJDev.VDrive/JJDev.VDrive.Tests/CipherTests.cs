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
    private void SetupKeyIV(ICipher cipher, SymmetricCipherType symmetricCipherType)
    {      
      switch (symmetricCipherType)
      {
        case SymmetricCipherType.Aes:
          cipher.Key = new byte[] { 105, 195, 252, 185, 2, 140, 51, 126, 104, 229, 79, 123, 212, 18, 202, 2, 110, 30, 207, 111, 0, 244, 173, 234, 220, 14, 253, 178, 156, 52, 214, 127 };
          cipher.IV = new byte[] { 8, 68, 137, 198, 127, 127, 18, 72, 241, 104, 126, 253, 191, 17, 44, 132 };
          break;
        case SymmetricCipherType.DES:
          cipher.Key = new byte[] { 193, 114, 251, 87, 29, 105, 15, 251 };
          cipher.IV = new byte[] { 228, 26, 220, 137, 168, 134, 150, 239 };
          break;
        case SymmetricCipherType.RC2:
          cipher.Key = null;
          cipher.IV = null;
          break;
        case SymmetricCipherType.Rijndael:
          cipher.Key = null;
          cipher.IV = null;
          break;
        case SymmetricCipherType.TripleDES:
          cipher.Key = null;
          cipher.IV = null;
          break;
        default:
          break;
      }
    }

    [Test]
    public void TestEncode_GivenEmptyInput_ShouldReturnEmptyByteArray()
    {
      var expected = new byte[0];
      var cipher = Substitute.For<ICipher>();
      cipher.Encode(Arg.Any<SymmetricCipherType>(), Arg.Any<string>()).Returns(new byte[0]);

      var actual = cipher.Encode(SymmetricCipherType.Aes, string.Empty);

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDecode_GivenEmptyInput_ShouldReturnEmptyString()
    {
      var expected = string.Empty;
      var cipher = Substitute.For<ICipher>();
      cipher.Decode(Arg.Any<SymmetricCipherType>(), Arg.Any<byte[]>()).Returns(string.Empty);

      var actual = cipher.Decode(SymmetricCipherType.Aes, new byte[0]);

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestAesEncode_GivenInput_ShouldReturnByteArray()
    {
      var expected = new byte[] { 51, 218, 85, 191, 9, 115, 231, 102, 129, 101, 85, 25, 233, 160, 16, 84 };
      var cipher = Substitute.For<SymmetricAlgorithmCipher>();
      SetupKeyIV(cipher, SymmetricCipherType.Aes);

      var actual = cipher.Encode(SymmetricCipherType.Aes, "content777");

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestAesDecode_GivenInput_ShouldReturnByteArray()
    {
      var expected = "content777";
      var cipher = Substitute.For<SymmetricAlgorithmCipher>();
      SetupKeyIV(cipher, SymmetricCipherType.Aes);

      var actual = cipher.Decode(SymmetricCipherType.Aes, 
        new byte[] { 51, 218, 85, 191, 9, 115, 231, 102, 129, 101, 85, 25, 233, 160, 16, 84 });

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDESEncode_GivenInput_ShouldReturnByteArray()
    {
      var expected = new byte[] { 78, 194, 203, 79, 146, 183, 225, 222, 136, 18, 91, 166, 154, 219, 60, 204 };
      var cipher = Substitute.For<SymmetricAlgorithmCipher>();
      SetupKeyIV(cipher, SymmetricCipherType.DES);

      var actual = cipher.Encode(SymmetricCipherType.DES, "content777");

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestDESDecode_GivenInput_ShouldReturnByteArray()
    {
      var expected = "content777";
      var cipher = Substitute.For<SymmetricAlgorithmCipher>();
      SetupKeyIV(cipher, SymmetricCipherType.DES);

      var actual = cipher.Decode(SymmetricCipherType.DES,
        new byte[] { 78, 194, 203, 79, 146, 183, 225, 222, 136, 18, 91, 166, 154, 219, 60, 204 });

      Assert.AreEqual(expected, actual);
    }
  }
}
