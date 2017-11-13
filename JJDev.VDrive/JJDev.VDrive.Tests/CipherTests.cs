using JJDev.VDrive.Core.Ciphers;
using JJDev.VDrive.Core.Contracts;
using NSubstitute;
using NUnit.Framework;

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
                    cipher.Key = new byte[] { 110, 118, 139, 107, 92, 75, 250, 148, 166, 165, 22, 233, 74, 131, 105, 140 };
                    cipher.IV = new byte[] { 210, 64, 11, 132, 61, 29, 73, 253 };
                    break;
                case SymmetricCipherType.Rijndael:
                    cipher.Key = new byte[] { 187, 136, 26, 38, 169, 83, 131, 237, 205, 215, 81, 219, 166, 220, 93, 211, 25, 114, 19, 246, 18, 219, 176, 197, 18, 175, 95, 183, 9, 1, 241, 222 };
                    cipher.IV = new byte[] { 186, 186, 148, 241, 211, 29, 97, 52, 254, 235, 110, 109, 5, 134, 76, 95 };
                    break;
                case SymmetricCipherType.TripleDES:
                    cipher.Key = new byte[] { 50, 181, 3, 3, 107, 23, 19, 186, 32, 123, 183, 16, 199, 11, 198, 94, 252, 75, 153, 78, 26, 63, 215, 132 };
                    cipher.IV = new byte[] { 135, 1, 207, 137, 109, 70, 219, 10 };
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

        [Test]
        public void TestRC2Encode_GivenInput_ShouldReturnByteArray()
        {
            var expected = new byte[] { 220, 127, 16, 101, 141, 35, 33, 61, 11, 159, 81, 239, 46, 106, 162, 107 };
            var cipher = Substitute.For<SymmetricAlgorithmCipher>();
            SetupKeyIV(cipher, SymmetricCipherType.RC2);

            var actual = cipher.Encode(SymmetricCipherType.RC2, "content777");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestRC2Decode_GivenInput_ShouldReturnByteArray()
        {
            var expected = "content777";
            var cipher = Substitute.For<SymmetricAlgorithmCipher>();
            SetupKeyIV(cipher, SymmetricCipherType.RC2);

            var actual = cipher.Decode(SymmetricCipherType.RC2,
              new byte[] { 220, 127, 16, 101, 141, 35, 33, 61, 11, 159, 81, 239, 46, 106, 162, 107 });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestRijndaelEncode_GivenInput_ShouldReturnByteArray()
        {
            var expected = new byte[] { 83, 59, 165, 116, 117, 105, 24, 154, 63, 129, 250, 104, 15, 145, 201, 93 };
            var cipher = Substitute.For<SymmetricAlgorithmCipher>();
            SetupKeyIV(cipher, SymmetricCipherType.Rijndael);

            var actual = cipher.Encode(SymmetricCipherType.Rijndael, "content777");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestRijndaelDecode_GivenInput_ShouldReturnByteArray()
        {
            var expected = "content777";
            var cipher = Substitute.For<SymmetricAlgorithmCipher>();
            SetupKeyIV(cipher, SymmetricCipherType.Rijndael);

            var actual = cipher.Decode(SymmetricCipherType.Rijndael,
              new byte[] { 83, 59, 165, 116, 117, 105, 24, 154, 63, 129, 250, 104, 15, 145, 201, 93 });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestTripleDESEncode_GivenInput_ShouldReturnByteArray()
        {
            var expected = new byte[] { 153, 126, 106, 58, 25, 174, 201, 147, 114, 110, 12, 12, 225, 232, 89, 255 };
            var cipher = Substitute.For<SymmetricAlgorithmCipher>();
            SetupKeyIV(cipher, SymmetricCipherType.TripleDES);

            var actual = cipher.Encode(SymmetricCipherType.TripleDES, "content777");            

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestTripleDESDecode_GivenInput_ShouldReturnByteArray()
        {
            var expected = "content777";
            var cipher = Substitute.For<SymmetricAlgorithmCipher>();
            SetupKeyIV(cipher, SymmetricCipherType.TripleDES);

            var actual = cipher.Decode(SymmetricCipherType.TripleDES,
              new byte[] { 153, 126, 106, 58, 25, 174, 201, 147, 114, 110, 12, 12, 225, 232, 89, 255 });

            Assert.AreEqual(expected, actual);
        }
    }
}
