// <copyright file="CryptoNetTests.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using CryptoNet.Extensions;
using CryptoNet.Models;
using CryptoNet.Share;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using SharperHacks.CoreLibs.IO;

using Shouldly;

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable All

namespace CryptoNet.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CryptoNetAesTests
{
    private const string ConfidentialData = @"Some Secret Data";
    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string SymmetricKeyFile = Path.Combine(BaseFolder, $"{KeyType.SymmetricKey}.xml");
    private static readonly byte[] symmetricKey = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");

    [Test]
    public void Encrypt_And_Decrypt_With_SymmetricKey_Test()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            // Arrange
            var iv = new byte[16];
            var cryptoNetAes = new CryptoNetAes(symmetricKey, iv);

            // Act
            var encryptedData = cryptoNetAes.EncryptFromString(ConfidentialData);
            var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

            // Assert
            ConfidentialData.ShouldBe(decryptedData);
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
        var iv = new byte[16];
        var cryptoNetAes = new CryptoNetAes(correctKey, iv);
        var encryptedData = cryptoNetAes.EncryptFromString(ConfidentialData);

        // Act & Assert
        Assert.Throws<CryptographicException>(() =>
        {
            new CryptoNetAes(wrongKey, iv).DecryptToString(encryptedData);
        });
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Validate_Decrypted_File_Against_Original_By_Comparing_Bytes_Test(string filename)
    {
        // Arrange
        var iv = new byte[16];
        var filePath = Path.Combine(Common.TestFilesPath, filename);
        byte[] originalFileBytes = File.ReadAllBytes(filePath);

        // Act
        byte[] encryptedBytes = new CryptoNetAes(symmetricKey, iv).EncryptFromBytes(originalFileBytes);
        byte[] decryptedBytes = new CryptoNetAes(symmetricKey, iv).DecryptToBytes(encryptedBytes);

        // Assert
        var filesMatch = CryptoNetExtensions.ByteArrayCompare(originalFileBytes, decryptedBytes);
        filesMatch.ShouldBeTrue();
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Encrypt_And_Decrypt_File_With_SymmetricKey_Test(string filename)
    {
        // Arrange
        var key = new CryptoNetAes().ExportKey();
        var filePath = Path.Combine(Common.TestFilesPath, filename);
        byte[] originalFileBytes = File.ReadAllBytes(filePath);

        // Act
        var encryptedBytes = new CryptoNetAes(key).EncryptFromBytes(originalFileBytes);
        var decryptedBytes = new CryptoNetAes(key).DecryptToBytes(encryptedBytes);

        // Assert
        var filesMatch = CryptoNetExtensions.ByteArrayCompare(originalFileBytes, decryptedBytes);
        filesMatch.ShouldBeTrue();
    }

    [Test]
    public void Encrypt_And_Decrypt_Content_With_SelfGenerated_SymmetricKey_Test()
    {
        // Arrange
        var key = new CryptoNetAes().ExportKey();
        var cryptoNetAes = new CryptoNetAes(key);

        // Act
        var encryptedData = cryptoNetAes.EncryptFromString(ConfidentialData);
        var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

        // Assert
        ConfidentialData.ShouldBe(decryptedData);
    }

    [Test]
    public void SelfGenerated_And_Save_SymmetricKey_Test()
    {
        // Arrange
        ICryptoNet cryptoNet = new CryptoNetAes();
        var file = new FileInfo(SymmetricKeyFile);

        // Act
        cryptoNet.ExportKeyAndSave(file);
        var encryptedData = cryptoNet.EncryptFromString(ConfidentialData);
        var decryptedData = new CryptoNetAes(file).DecryptToString(encryptedData);

        // Assert
        File.Exists(file.FullName).ShouldBeTrue();
        ConfidentialData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_And_Decrypt_Content_With_Own_SymmetricKey_Test()
    {
        // Arrange
        var key = "12345678901234567890123456789012"; // 32 characters
        var iv = "1234567890123456"; // 16 characters
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var ivBytes = Encoding.UTF8.GetBytes(iv);
        var cryptoNetAes = new CryptoNetAes(keyBytes, ivBytes);

        // Act
        var encryptedData = cryptoNetAes.EncryptFromString(ConfidentialData);
        var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

        // Assert
        ConfidentialData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_And_Decrypt_With_Human_Readable_Key_And_Secret_SymmetricKey_Test()
    {
        // Arrange
        var key = UniqueKeyGenerator("symmetricKey");
        var iv = new string(UniqueKeyGenerator("password").Take(16).ToArray());
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var ivBytes = Encoding.UTF8.GetBytes(iv);
        var cryptoNetAes = new CryptoNetAes(keyBytes, ivBytes);

        // Act
        var encryptedData = cryptoNetAes.EncryptFromString(ConfidentialData);
        var decryptedData = cryptoNetAes.DecryptToString(encryptedData);

        // Assert
        ConfidentialData.ShouldBe(decryptedData);
    }

    // Helper method
    private static string UniqueKeyGenerator(string input)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = MD5.HashData(inputBytes);

        var stringBuilder = new StringBuilder();
        foreach (var byteValue in hashBytes)
        {
            stringBuilder.Append(byteValue.ToString("X2"));
        }
        return stringBuilder.ToString();
    }

    [Test]
    public void Can_Save_And_Rertieve_Symetric_Keys()
    {
        var tmpDirPrefix = $"{nameof(CryptoNet.UnitTests)}-{nameof(Can_Save_And_Rertieve_Symetric_Keys)}-";
        var keyFileInfo = new FileInfo("key");
        var encoder = new CryptoNetAes();
        var keyOut = encoder.ExportKey();

        using var tmpDir = new TempDirectory(tmpDirPrefix);

        encoder.ExportKeyAndSave(keyFileInfo);

        var decoder = new CryptoNetAes(keyFileInfo);
        var keyIn = encoder.ExportKey();

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
        Assert.Throws<ArgumentNullException>(() => encoder.DecryptToBytes(new byte[0]));
    }

}

