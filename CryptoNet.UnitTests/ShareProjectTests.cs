using System;
using System.IO;
using CryptoNet.Share;
using CryptoNet.Share.Extensions;
using NUnit.Framework;
using Shouldly;

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class ShareProjectTests
    {
        [Test]
        public void TryGetSolutionDirectoryInfo_ShouldReturnNull_WhenNoSolutionFileExists()
        {
            // Act
            var result = DirectoryExension.TryGetSolutionDirectoryInfo();

            // Assert
            result.ShouldNotBeNull();
            result!.FullName.ShouldContain("CryptoNet");
        }

        [Test]
        public void TryGetSolutionDirectoryInfo_ShouldReturnDirectoryWithTestFiles()
        {
            // Arrange
            string solutionFilePath = Path.Combine(Common.TestFilesPath);

            // Act
            var result = DirectoryExension.TryGetSolutionDirectoryInfo();
            var testFiles = Path.Combine(result!.FullName, "Resources", "TestFiles");
            var di = new DirectoryInfo(testFiles);
            var files = di.GetFiles("test.*");

            // Assert
            files.ShouldNotBeNull();
            files.Count().ShouldBe(4);
        }

        [Test]
        public void CheckContent_WhenContentsAreSame_ShouldReturnTrue()
        {
            // Arrange
            string originalContent = "This is a test string";
            string decryptedContent = "This is a test string";

            // Act
            bool result = Common.CheckContent(originalContent, decryptedContent);

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
            bool result = Common.CheckContent(originalContent, decryptedContent);

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
            bool result = Common.CheckContent(originalContent, decryptedContent);

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
            bool result = Common.CheckContent(originalContent, decryptedContent!);

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
            bool result = Common.CheckContent(originalContent!, decryptedContent!);

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
            bool result = Common.CheckContent(originalContent, decryptedContent);

            // Assert
            result.ShouldBeTrue("because both contents are identical even with special characters.");
        }
    }
}
