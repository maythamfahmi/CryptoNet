// <copyright file="CryptoNetTests.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using CryptoNet.ExtPack;
using CryptoNet.Models;
using CryptoNet.Shared;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Shouldly;

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System;
using System.IO;
using System.Linq;


// ReSharper disable All

namespace CryptoNet.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CryptoNetRsaTests
{
    private static readonly string BaseFolder = AppContext.BaseDirectory;
    private static readonly string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
    private static readonly string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

    public CryptoNetRsaTests()
    {
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();
        cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
        cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);
    }

    [Test]
    public void SelfGenerated_AsymmetricKey_And_TypeValidation_Test()
    {
        // Arrange & Act
        var privateKeyCrypto = new CryptoNetRsa(new FileInfo(PrivateKeyFile));
        var publicKeyCrypto = new CryptoNetRsa(new FileInfo(PublicKeyFile));

        // Assert
        privateKeyCrypto.Info.KeyType.ShouldBe(KeyType.PrivateKey);
        publicKeyCrypto.Info.KeyType.ShouldBe(KeyType.PublicKey);
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_Test()
    {
        // Arrange
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();
        var privateKey = cryptoNet.GetKey(true);
        var publicKey = cryptoNet.GetKey(false);

        // Act
        var encrypt = new CryptoNetRsa(publicKey).EncryptFromString(TestConfig.ConfidentialDummyData);
        var decrypt = new CryptoNetRsa(privateKey).DecryptToString(encrypt);

        // Assert
        ExtensionPack.CheckContent(TestConfig.ConfidentialDummyData, decrypt).ShouldBeTrue();
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_Invalid_AsymmetricKey_Test()
    {
        // Arrange
        var encrypt = new CryptoNetRsa().EncryptFromString(TestConfig.ConfidentialDummyData);
        const string invalidKey = "invalid-key";
        string decrypt = string.Empty;

        // Act & Assert
        try
        {
            decrypt = new CryptoNetRsa(invalidKey).DecryptToString(encrypt);
        }
        catch (Exception e)
        {
            ExtensionPack.CheckContent(TestConfig.ConfidentialDummyData, decrypt).ShouldBeFalse();
            e.Message.ShouldContain("The provided XML could not be read.");
        }
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
    {
        // Arrange
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();
        var encrypt = cryptoNet.EncryptFromString(TestConfig.ConfidentialDummyData);
        File.WriteAllBytes(TestConfig.EncryptedContentFile, encrypt);

        // Act
        var encryptFile = File.ReadAllBytes(TestConfig.EncryptedContentFile);
        var content = cryptoNet.DecryptToString(encryptFile);

        // Assert
        ExtensionPack.CheckContent(TestConfig.ConfidentialDummyData, content).ShouldBeTrue();
    }

    [TestCase("test.docx"), Order(5)]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Encrypt_Decrypt_Documents_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test(string filename)
    {
        // Arrange
        var testDocument = File.ReadAllBytes(Path.Combine(TestConfig.TestFilesPath, filename));
        var encrypt = new CryptoNetRsa(new FileInfo(PublicKeyFile)).EncryptFromBytes(testDocument);

        // Act
        var decrypt = new CryptoNetRsa(new FileInfo(PrivateKeyFile)).DecryptToBytes(encrypt);

        // Assert
        ClassicAssert.AreEqual(testDocument, decrypt);
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_PreStored_SelfGenerated_AsymmetricKey_Test()
    {
        // Arrange
        ICryptoNetRsa cryptoNet = new CryptoNetRsa(new FileInfo(TestConfig.RsaStoredKeyPair));

        // Act
        var encrypt = cryptoNet.EncryptFromString(TestConfig.ConfidentialDummyData);
        var content = cryptoNet.DecryptToString(encrypt);

        // Assert
        ExtensionPack.CheckContent(TestConfig.ConfidentialDummyData, content).ShouldBeTrue();
    }

    [Test]
    public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Using_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
    {
        // Arrange
        var certificate = new FileInfo(TestConfig.RsaStoredKeyPair);
        new CryptoNetRsa(certificate).SaveKey(new FileInfo(PublicKeyFile), false);

        // Act
        var importPublicKey = new FileInfo(PublicKeyFile);
        var encryptWithPublicKey = new CryptoNetRsa(importPublicKey).EncryptFromString(TestConfig.ConfidentialDummyData);
        var decryptWithPrivateKey = new CryptoNetRsa(certificate).DecryptToString(encryptWithPublicKey);

        // Assert
        ExtensionPack.CheckContent(TestConfig.ConfidentialDummyData, decryptWithPrivateKey).ShouldBeTrue();
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Validate_Decrypted_File_Against_The_Original_File_By_Comparing_Bytes_Test(string filename)
    {
        // Arrange
        var certificate = new FileInfo(TestConfig.RsaStoredKeyPair);
        ICryptoNetRsa cryptoNet = new CryptoNetRsa(certificate);
        var filePath = Path.Combine(TestConfig.TestFilesPath, filename);
        byte[] originalFileBytes = File.ReadAllBytes(filePath);

        // Act
        byte[] encryptedBytes = cryptoNet.EncryptFromBytes(originalFileBytes);
        byte[] decryptedBytes = cryptoNet.DecryptToBytes(encryptedBytes);

        // Assert
        var isIdenticalFile = CryptoNetExtensions.ByteArrayCompare(originalFileBytes, decryptedBytes);
        isIdenticalFile.ShouldBeTrue();
    }

    [Test]
    public void Control_SelfGenerated_Exist_AsymmetricKey_Test()
    {
        // Arrange & Act
        var fileExistsPrivateKey = File.Exists(new FileInfo(PrivateKeyFile).FullName);
        var fileExistsPublicKey = File.Exists(new FileInfo(PublicKeyFile).FullName);

        // Assert
        fileExistsPrivateKey.ShouldBeTrue();
        fileExistsPublicKey.ShouldBeTrue();
    }

    [Test]
    public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content_Test()
    {
        // Arrange
        var publicKeyRsa = new CryptoNetRsa(new FileInfo(PublicKeyFile));
        var privateKeyRsa = new CryptoNetRsa(new FileInfo(PrivateKeyFile));

        // Act
        var encryptedData = publicKeyRsa.EncryptFromString(TestConfig.ConfidentialDummyData);
        var decryptedData = privateKeyRsa.DecryptToString(encryptedData);

        // Assert
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_Decrypt_Using_X509_Certificate_Test()
    {
        // Arrange
        // You can change to test real system certificate by using CryptoNetExtensions.GetCertificateFromStore("CN=MaythamCertificateName")
        X509Certificate2 ? certificate = TestConfig.CreateSelfSignedCertificate();
        var rsaPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
        var rsaPrivateKey = new CryptoNetRsa(certificate, KeyType.PrivateKey);

        // Act
        var encryptedData = rsaPublicKey.EncryptFromString(TestConfig.ConfidentialDummyData);
        var decryptedData = rsaPrivateKey.DecryptToString(encryptedData);

        // Assert
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData);

    }

    [Test]
    public void Export_Public_Key_For_X509_Certificate_Test()
    {
        // Arrange
        // You can change to test real system certificate by using CryptoNetExtensions.GetCertificateFromStore("CN=MaythamCertificateName")
        X509Certificate2? certificate = TestConfig.CreateSelfSignedCertificate();
        var rsa = new CryptoNetRsa(certificate, KeyType.PublicKey);

        // Act
        var publicKey = rsa.GetKey(false);

        // Assert
        publicKey.ShouldNotBeNull();
        publicKey.ShouldNotBeEmpty();
    }

    [Test]
    public void Customize_PEM_Key_Encryption_Decryption_Test()
    {
        // Arrange
        // You can change to test real system certificate by using CryptoNetExtensions.GetCertificateFromStore("CN=MaythamCertificateName")
        X509Certificate2? certificate = TestConfig.CreateSelfSignedCertificate();

        var pubKeyPem = ExtensionPack.ExportPemKey(certificate!, false);
        var priKeyPem = ExtensionPack.ExportPemKey(certificate!);
        var password = "password";
        var encryptedPriKeyBytes = ExtensionPack.ExportPemKeyWithPassword(certificate!, password);

        // Act
        ICryptoNetRsa cryptoNet1 = ImportPemKeyWithPassword(encryptedPriKeyBytes, password);
        var encryptedData1 = cryptoNet1.EncryptFromString(TestConfig.ConfidentialDummyData);

        ICryptoNetRsa cryptoNet2 = ImportPemKey(pubKeyPem);
        var encryptedData2 = cryptoNet2.EncryptFromString(TestConfig.ConfidentialDummyData);

        ICryptoNetRsa cryptoNet3 = ImportPemKey(priKeyPem);
        var decryptedData1 = cryptoNet3.DecryptToString(encryptedData1);
        var decryptedData2 = cryptoNet3.DecryptToString(encryptedData2);

        // Assert
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData1);
        TestConfig.ConfidentialDummyData.ShouldBe(decryptedData2);
    }

    public static ICryptoNetRsa ImportPemKey(char[] key)
    {
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail!.Rsa?.ImportFromPem(key);
        return cryptoNet;
    }

    public static ICryptoNetRsa ImportPemKeyWithPassword(byte[] encryptedPrivateKey, string password)
    {
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail?.Rsa?.ImportEncryptedPkcs8PrivateKey(password, encryptedPrivateKey, out _);
        return cryptoNet;
    }
}
