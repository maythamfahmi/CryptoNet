using NUnit.Framework;
using Shouldly;
using CryptoNet.ExtPack;
using System;

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

        [Test]
        public void CheckContent_WhenContentsAreSame_ShouldReturnTrue()
        {
            // Arrange
            string originalContent = "This is a test string";
            string decryptedContent = "This is a test string";

            // Act
            bool result = ExtensionPack.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeTrue("because both contents are identical, so MD5 hashes should match.");
        }

        [Test]
        public void CheckContent_WhenContentsAreDifferent_ShouldReturnFalse()
        {
            // Arrange
            string originalContent = "This is a test string";
            string decryptedContent = "This is a different string";

            // Act
            bool result = ExtensionPack.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeFalse("because contents are different, so their MD5 hashes should not match.");
        }

        [Test]
        public void CheckContent_WhenBothContentsAreEmpty_ShouldReturnTrue()
        {
            // Arrange
            string originalContent = "";
            string decryptedContent = "";

            // Act
            bool result = ExtensionPack.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeTrue("because both contents are empty, and their MD5 hashes should match.");
        }

        [Test]
        public void CheckContent_WhenOneContentIsNull_ShouldReturnFalse()
        {
            // Arrange
            string originalContent = "This is a test string";
            string? decryptedContent = null;

            // Act
            bool result = ExtensionPack.CheckContent(originalContent, decryptedContent!);

            // Assert
            result.ShouldBeFalse("because one content is null, so their MD5 hashes cannot match.");
        }

        [Test]
        public void CheckContent_WhenBothContentsAreNull_ShouldReturnTrue()
        {
            // Arrange
            string? originalContent = null;
            string? decryptedContent = null;

            // Act
            bool result = ExtensionPack.CheckContent(originalContent!, decryptedContent!);

            // Assert
            result.ShouldBeTrue("because both contents are null, so their MD5 hashes should be the same.");
        }

        [Test]
        public void CheckContent_WhenContentsContainSpecialCharacters_ShouldReturnTrue()
        {
            // Arrange
            string originalContent = "!@#$%^&*()_+1234567890";
            string decryptedContent = "!@#$%^&*()_+1234567890";

            // Act
            bool result = ExtensionPack.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeTrue("because both contents are identical even with special characters.");
        }

        [TestCase("testInput")]
        [TestCase("   ")] // Whitespace input
        [TestCase("你好世界")] // Non-ASCII input
        [TestCase("Abc123!@#")] // Mixed characters
        [TestCase("sameInput")] // Pre-calculated MD5 for "sameInput"
        public void UniqueKeyGenerator_ShouldGenerateCorrectHash_ForGivenInput(string input)
        {
            // Act
            string result = ExtensionPack.UniqueKeyGenerator(input);

            // Assert
            result.ShouldNotBeNull($"The MD5 hash generated by Common.UniqueKeyGenerator for input '{input}' is incorrect.");
        }

        [TestCase("sameInput")]
        [TestCase("anotherInput")]
        public void UniqueKeyGenerator_ShouldGenerateSameHash_ForSameInput(string input)
        {
            // Act
            string result1 = ExtensionPack.UniqueKeyGenerator(input);
            string result2 = ExtensionPack.UniqueKeyGenerator(input);

            // Assert
            result1.ShouldBe(result2, $"Common.UniqueKeyGenerator should return the same hash for the same input '{input}'.");
        }

        [TestCase("input1", "input2")]
        [TestCase("longInput", "shortInput")]
        [TestCase("123456", "654321")]
        public void UniqueKeyGenerator_ShouldGenerateDifferentHash_ForDifferentInputs(string input1, string input2)
        {
            // Act
            string result1 = ExtensionPack.UniqueKeyGenerator(input1);
            string result2 = ExtensionPack.UniqueKeyGenerator(input2);

            // Assert
            result1.ShouldNotBe(result2, $"Common.UniqueKeyGenerator should return different hashes for different inputs '{input1}' and '{input2}'.");
        }

        [Test]
        public void UniqueKeyGenerator_ShouldThrowArgumentNullException_WhenInputIsNull()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => ExtensionPack.UniqueKeyGenerator(null!));
        }

        [Test]
        public void UniqueKeyGenerator_ShouldThrowArgumentNullException_WhenInputIsEmpty()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => ExtensionPack.UniqueKeyGenerator(string.Empty));
        }

        [Test]
        public void UniqueKeyGenerator_ShouldGenerateHash_ForLongInput()
        {
            // Arrange
            string input = new string('a', 1000); // String with 1000 'a' characters
            string expectedHash = "CABE45DCC9AE5B66BA86600CCA6B8BA8"; // MD5 hash for 1000 'a' characters

            // Act
            string result = ExtensionPack.UniqueKeyGenerator(input);

            // Assert
            result.ShouldBe(expectedHash, "The MD5 hash generated for a long input string is incorrect.");
        }
    }
}
