using NUnit.Framework;
using Shouldly;
using CryptoNet.ExtPack;
using CryptoNet.Shared;

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class ExtensionPackTests
    {
        [TestCase("content1", "content2", false, TestName = "CheckContent_ShouldReturnFalse_WhenContentsDiffer")]
        [TestCase("sameContent", "sameContent", true, TestName = "CheckContent_ShouldReturnTrue_WhenContentsAreIdentical")]
        [TestCase(null, null, true, TestName = "CheckContent_ShouldReturnTrue_WhenBothContentsAreNull")]
        [TestCase("non-null", null, false, TestName = "CheckContent_ShouldReturnFalse_WhenOneContentIsNull")]
        public void CheckContent_Tests(string originalContent, string decryptedContent, bool expected)
        {
            var result = ExtensionPack.CheckContent(originalContent, decryptedContent);
            result.ShouldBe(expected);
        }

        [TestCase("test content", "9473fdd0d880a43c21b7778d34872157", TestName = "CalculateMd5_ShouldReturnExpectedHash")]
        [TestCase("different content", "fb9ca3de466c5e579dc8aaf5f1e6940e", TestName = "CalculateMd5_ShouldReturnDifferentHashForDifferentContent")]
        public void CalculateMd5_Tests(string content, string expectedHash)
        {
            var result = ExtensionPack.CalculateMd5(content);
            result.ShouldBe(expectedHash);
        }

        [TestCase("uniqueInput", 32, TestName = "UniqueKeyGenerator_ShouldReturn32CharHash_WhenInputIsValid")]
        [TestCase(null, 0, TestName = "UniqueKeyGenerator_ShouldThrowArgumentNullException_WhenInputIsNull")]
        [TestCase("", 0, TestName = "UniqueKeyGenerator_ShouldThrowArgumentNullException_WhenInputIsEmpty")]
        public void UniqueKeyGenerator_Tests(string input, int expectedLength)
        {
            if (string.IsNullOrEmpty(input))
            {
                Should.Throw<ArgumentNullException>(() => ExtensionPack.UniqueKeyGenerator(input));
            }
            else
            {
                var result = ExtensionPack.UniqueKeyGenerator(input);
                result.Length.ShouldBe(expectedLength);
            }
        }

        [TestCase(true, "PRIVATE KEY", TestName = "ExportPemKey_ShouldReturnPrivateKeyPem_WhenPrivateKeyIsTrue")]
        [TestCase(false, "PUBLIC KEY", TestName = "ExportPemKey_ShouldReturnPublicKeyPem_WhenPrivateKeyIsFalse")]
        public void ExportPemKey_Tests(bool privateKey, string expectedKeyType)
        {
            using var cert = TestConfig.CreateSelfSignedCertificate();
            var pemKey = ExtensionPack.ExportPemKey(cert, privateKey);

            pemKey.ShouldNotBeEmpty();
           
            string result = new string(pemKey);
            result.ShouldContain(expectedKeyType);

        }

        [TestCase("securepassword", TestName = "ExportPemKeyWithPassword_ShouldReturnEncryptedPrivateKey")]
        public void ExportPemKeyWithPassword_Tests(string password)
        {
            using var cert = TestConfig.CreateSelfSignedCertificate();
            var encryptedKey = ExtensionPack.ExportPemKeyWithPassword(cert, password);
            encryptedKey.ShouldNotBeEmpty();
        }
    }
}
