// <copyright file="CryptoNetTests.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using CryptoNet.Shared;
using CryptoNet.Models;
using CryptoNet.ExtPack;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using SharperHacks.CoreLibs.IO;

using Shouldly;

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Linq;
using Moq;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.DotNet.Scaffolding.Shared;

// ReSharper disable All

namespace CryptoNet.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CryptoNetAesTests
{
    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string SymmetricKeyFile = Path.Combine(BaseFolder, $"{KeyType.SymmetricKey}.json");
    private static readonly byte[] symmetricKey = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");

    [Test]
    public void Encrypt_And_Decrypt_With_SymmetricKey_Test()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            // Arrange
            var Iv = new byte[16];
            var cryptoNetAes = new CryptoNetAes(symmetricKey, Iv);

            // Act
            var encryptedData = cryptoNetAes.EncryptFromString(TestConfig.ConfidentialDummyData);
            var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

            // Assert
            TestConfig.ConfidentialDummyData.ShouldBe(decryptedData);
            cryptoNetAes.Info.KeyType.ShouldBe(KeyType.SymmetricKey);
            cryptoNetAes.Info.KeyType.ShouldNotBe(KeyType.PublicKey);
            cryptoNetAes.Info.KeyType.ShouldNotBe(KeyType.PrivateKey);
            cryptoNetAes.Info.KeyType.ShouldNotBe(KeyType.NotSet);
        }
    }

    [Test]
    public void Encrypt_And_Decrypt_With_Wrong_SymmetricKey_Should_Throw_Exception()
    {
        // Arrange
        var correctKey = symmetricKey;
        var wrongKey = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315b1916");
        var Iv = new byte[16];
        var cryptoNetAes = new CryptoNetAes(correctKey, Iv);
        var encryptedData = cryptoNetAes.EncryptFromString(TestConfig.ConfidentialDummyData);

        // Act & Assert
        Assert.Throws<CryptographicException>(() =>
        {
            new CryptoNetAes(wrongKey, Iv).DecryptToString(encryptedData);
        });
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Validate_Decrypted_File_Against_Original_By_Comparing_Bytes_Test(string filename)
    {
        // Arrange
        var Iv = new byte[16];
        var filePath = Path.Combine(TestConfig.TestFilesPath, filename);
        byte[] originalFileBytes = File.ReadAllBytes(filePath);

        // Act
        byte[] encryptedBytes = new CryptoNetAes(symmetricKey, Iv).EncryptFromBytes(originalFileBytes);
        byte[] decryptedBytes = new CryptoNetAes(symmetricKey, Iv).DecryptToBytes(encryptedBytes);

        // Assert
        var filesMatch = ExtShared.ByteArrayCompare(originalFileBytes, decryptedBytes);
        filesMatch.ShouldBeTrue();
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Encrypt_And_Decrypt_File_With_SymmetricKey_Test(string filename)
    {
        // Arrange
        var key = new CryptoNetAes().GetKey();
        var filePath = Path.Combine(TestConfig.TestFilesPath, filename);
        byte[] originalFileBytes = File.ReadAllBytes(filePath);

        // Act
        var encryptedBytes = new CryptoNetAes(key).EncryptFromBytes(originalFileBytes);
        var decryptedBytes = new CryptoNetAes(key).DecryptToBytes(encryptedBytes);

        // Assert
        var filesMatch = Shared.ExtShared.ByteArrayCompare(originalFileBytes, decryptedBytes);
        filesMatch.ShouldBeTrue();
    }

    [Test]
    public void Encrypt_And_Decrypt_Content_With_SelfGenerated_SymmetricKey_Test()
    {
        // Arrange
        var key = new CryptoNetAes().GetKey();
        var cryptoNetAes = new CryptoNetAes(key);

        // Act
        var encryptedData = cryptoNetAes.EncryptFromString(TestConfig.ConfidentialDummyData);
        var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

        // Assert
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData);
    }

    [Test]
    public void SelfGenerated_And_Save_SymmetricKey_Test()
    {
        // Arrange
        ICryptoNetAes cryptoNet = new CryptoNetAes();
        var file = new FileInfo(SymmetricKeyFile);

        // Act
        cryptoNet.SaveKey(file);
        var encryptedData = cryptoNet.EncryptFromString(TestConfig.ConfidentialDummyData);
        var decryptedData = new CryptoNetAes(file).DecryptToString(encryptedData);

        // Assert
        File.Exists(file.FullName).ShouldBeTrue();
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_And_Decrypt_Content_With_Own_SymmetricKey_Test()
    {
        // Arrange
        var key = "12345678901234567890123456789012"; // 32 characters
        var Iv = "1234567890123456"; // 16 characters
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var IvBytes = Encoding.UTF8.GetBytes(Iv);
        var cryptoNetAes = new CryptoNetAes(keyBytes, IvBytes);

        // Act
        var encryptedData = cryptoNetAes.EncryptFromString(TestConfig.ConfidentialDummyData);
        var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

        // Assert
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_And_Decrypt_With_Human_Readable_Key_And_Secret_SymmetricKey_Test()
    {
        // Arrange
        var key = ExtensionPack.UniqueKeyGenerator("symmetricKey");
        var Iv = new string(ExtensionPack.UniqueKeyGenerator("password").Take(16).ToArray());
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var IvBytes = Encoding.UTF8.GetBytes(Iv);
        var cryptoNetAes = new CryptoNetAes(keyBytes, IvBytes);

        // Act
        var encryptedData = cryptoNetAes.EncryptFromString(TestConfig.ConfidentialDummyData);
        var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

        // Assert
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData);
    }

    [Test]
    public void Can_Save_And_Rertieve_Symetric_Keys()
    {
        var tmpDirPrefix = $"{nameof(CryptoNet.UnitTests)}-{nameof(Can_Save_And_Rertieve_Symetric_Keys)}-";
        var keyFileInfo = new FileInfo("key");
        var encoder = new CryptoNetAes();
        var keyOut = encoder.GetKey();

        using var tmpDir = new TempDirectory(tmpDirPrefix);

        encoder.SaveKey(keyFileInfo);

        var keyIn = encoder.GetKey();

        ClassicAssert.AreEqual(keyOut, keyIn);
    }

    [Test]
    public void EncryptContent_Throws_ArgumentNullException()
    {
        var encoder = new CryptoNetAes();

        Assert.Throws<ArgumentNullException>(() => encoder.EncryptFromBytes(null!));
        Assert.Throws<ArgumentNullException>(() => encoder.EncryptFromString(string.Empty));
    }

    [Test]
    public void DecryptContent_Throws_ArgumentNullException()
    {
        var encoder = new CryptoNetAes();

        Assert.Throws<ArgumentNullException>(() => encoder.DecryptToBytes(null!));
        Assert.Throws<ArgumentNullException>(() => encoder.DecryptToBytes([]));
    }

    [Ignore("")]
    public void SaveKey_ShouldInvokeSaveKeyWithFileInfo_WhenGIvenFilename()
    {
        // Arrange
        var filename = "testfile.txt";

        // Create a mock for the CryptoNetAes class if SaveKey(FileInfo) is not directly testable
        var keySaverMock = new Mock<CryptoNetAes>() { CallBase = true };

        // Act
        keySaverMock.Object.SaveKey(filename);

        // Assert
        keySaverMock.Verify(saver => saver.SaveKey(It.Is<FileInfo>(fi => fi.FullName == filename)), Times.Once);
    }

    [Test]
    public void Constructor_ShouldGenerateNewKeyAndIv_WhenNoParametersPassed()
    {
        // Arrange & Act
        var crypto = new CryptoNetAes();

        // Assert
        crypto.Info.AesDetail?.AesKeyValue.Key.ShouldNotBeNull();
        crypto.Info.AesDetail?.AesKeyValue.Iv.ShouldNotBeNull();
        crypto.Info.AesDetail?.AesKeyValue.Key.Length.ShouldBe(32);
        crypto.Info.AesDetail?.AesKeyValue.Iv.Length.ShouldBe(16);
    }

    [Ignore("")]
    public void Constructor_ShouldImportKeyAndGenerateIv_WhenKeyStringPassed()
    {
        // Arrange
        string keyString = "YourBase64EncodedKeyStringHere";
        var expectedKey = ExtShared.ImportAesKey(keyString).Key;

        // Act
        var crypto = new CryptoNetAes(keyString);

        // Assert
        crypto.Info.AesDetail?.AesKeyValue.Key.ShouldBe(expectedKey);
        crypto.Info.AesDetail?.AesKeyValue.Iv.ShouldNotBeNull();
        crypto.Info.AesDetail?.AesKeyValue.Key.Length.ShouldBe(32);
        crypto.Info.AesDetail?.AesKeyValue.Iv.Length.ShouldBe(16);
    }

    [Ignore("")]
    public void Constructor_ShouldImportKeyFromMockedFile_WhenFileInfoPassed()
    {
        // Arrange
        string keyString = "YourBase64EncodedKeyStringHere";
        var mockFileInfo = new Mock<FileInfo>("tempKeyFile.txt");

        // Mock File.ReadAllText to return keyString without accessing the filesystem
        var mockFileSystem = new Mock<IFileSystem>();
        mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns(keyString);

        var expectedKey = ExtShared.ImportAesKey(keyString).Key;

        // Act
        var crypto = new CryptoNetAes(mockFileInfo.Object);

        // Assert
        crypto.Info.AesDetail?.AesKeyValue.Key.ShouldBe(expectedKey);
        crypto.Info.AesDetail?.AesKeyValue.Iv.ShouldNotBeNull();
        crypto.Info.AesDetail?.AesKeyValue.Key.Length.ShouldBe(32);
        crypto.Info.AesDetail?.AesKeyValue.Iv.Length.ShouldBe(16);
    }

    [Test]
    public void Constructor_ShouldSetSpecifiedKeyAndIv_WhenKeyAndIvByteArraysPassed()
    {
        // Arrange
        byte[] key = new byte[32]; // 32 bytes for AES-256
        byte[] Iv = new byte[16];  // 16 bytes for AES Iv
        RandomNumberGenerator.Fill(key);
        RandomNumberGenerator.Fill(Iv);

        // Act
        var crypto = new CryptoNetAes(key, Iv);

        // Assert
        crypto.Info.AesDetail?.AesKeyValue.Key.ShouldBe(key);
        crypto.Info.AesDetail?.AesKeyValue.Iv.ShouldBe(Iv);
    }
}

