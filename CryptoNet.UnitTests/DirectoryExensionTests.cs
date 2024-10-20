using System;
using System.IO;
using CryptoNet.Share;
using CryptoNet.Share.Extensions;
using NUnit.Framework;
using Shouldly;

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class DirectoryExensionTests
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
    }
}
