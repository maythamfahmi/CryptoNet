using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using CryptoNet.Models;
using CryptoNet.Shared;
using NUnit.Framework;
using Shouldly;

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class SharedTests
    {
        [Test]
        public void TryGetSolutionDirectoryInfo_ShouldReturnDirectoryWithTestFiles()
        {
            // Arrange
            var result = TestConfig.TryGetSolutionDirectoryInfo();

            // Act
            var testFiles = Path.Combine(result!.FullName, "Resources", "TestFiles");
            var di = new DirectoryInfo(testFiles);
            var files = di.GetFiles("test.*").Select(e => e.FullName);

            // Assert
            files.ShouldNotBeNull();
            files.Count().ShouldBe(4);
        }

        [Test]
        public void GetParameters_ShouldReturnRsaParameters_WhenCertificateAndKeyTypeAreValid()
        {
            // Arrange
            var rsa = RSA.Create();
            var certificateRequest = new CertificateRequest("CN=TestCert", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            // Act
            RSAParameters parameters = ExtShared.GetParameters(certificate, KeyType.PrivateKey);

            // Assert
            parameters.D.ShouldNotBeNull();
        }

        [Test]
        public void GetCertificateFromStore_WithNonexistentCertName_ShouldReturnNull()
        {
            // Act
            var result = ExtShared.GetCertificateFromStore(StoreName.My, StoreLocation.CurrentUser, "NonexistentCertificate");

            // Assert
            result.ShouldBeNull();
        }

        [Test]
        public void BytesToString_ShouldConvertByteArrayToString()
        {
            // Arrange
            var bytes = Encoding.ASCII.GetBytes("Hello");

            // Act
            var result = ExtShared.BytesToString(bytes);

            // Assert
            result.ShouldBe("Hello");
        }

        [Test]
        public void StringToBytes_ShouldConvertStringToByteArray()
        {
            // Arrange
            var content = "Hello";

            // Act
            var result = ExtShared.StringToBytes(content);

            // Assert
            result.ShouldBeEquivalentTo(Encoding.ASCII.GetBytes("Hello"));
        }

        [Test]
        public void Base64BytesToString_ShouldEncodeBytesToBase64String()
        {
            // Arrange
            var bytes = Encoding.ASCII.GetBytes("Hello");

            // Act
            var result = ExtShared.Base64BytesToString(bytes);

            // Assert
            result.ShouldBe("SGVsbG8=");
        }

        [Test]
        public void Base64StringToBytes_ShouldDecodeBase64StringToBytes()
        {
            // Arrange
            var content = "SGVsbG8=";

            // Act
            var result = ExtShared.Base64StringToBytes(content);

            // Assert
            result.ShouldBeEquivalentTo(Encoding.ASCII.GetBytes("Hello"));
        }

        [Test]
        public void ByteArrayCompare_ShouldReturnTrue_WhenArraysAreEqual()
        {
            // Arrange
            var array1 = new byte[] { 1, 2, 3 };
            var array2 = new byte[] { 1, 2, 3 };

            // Act
            var result = ExtShared.ByteArrayCompare(array1, array2);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void ByteArrayCompare_ShouldReturnFalse_WhenArraysAreNotEqual()
        {
            // Arrange
            var array1 = new byte[] { 1, 2, 3 };
            var array2 = new byte[] { 1, 2, 4 };

            // Act
            var result = ExtShared.ByteArrayCompare(array1, array2);

            // Assert
            result.ShouldBeFalse();
        }

    }
}
