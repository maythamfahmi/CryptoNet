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

using Shouldly;

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

// ReSharper disable All

namespace CryptoNet.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CryptoNetRsaTests
{
    private const string ConfidentialData = @"Some Secret Data";
    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;

    internal static readonly string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
    internal static readonly string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

    [Test]
    public void SelfGenerated_AsymmetricKey_And_TypeValidation_Test()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();

        cryptoNet.ExportKeyAndSave(new FileInfo(Common.PrivateKeyFile), true);
        cryptoNet.ExportKeyAndSave(new FileInfo(Common.PublicKeyFile), false);

        new CryptoNetRsa(new FileInfo(Common.PrivateKeyFile)).Info.KeyType.ShouldBe(KeyType.PrivateKey);
        new CryptoNetRsa(new FileInfo(Common.PublicKeyFile)).Info.KeyType.ShouldBe(KeyType.PublicKey);
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_Test()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        var privateKey = cryptoNet.ExportKey(true);
        var publicKey = cryptoNet.ExportKey(false);

        var encrypt = new CryptoNetRsa(publicKey).EncryptFromString(Common.ConfidentialDummyData);
        var decrypt = new CryptoNetRsa(privateKey).DecryptToString(encrypt);

        Common.CheckContent(Common.ConfidentialDummyData, decrypt).ShouldBeTrue();
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_Invalid_AsymmetricKey_Test()
    {
        var encrypt = new CryptoNetRsa().EncryptFromString(Common.ConfidentialDummyData);

        const string invalidKey = "invalid-key";
        string decrypt = string.Empty;
        try
        {
            decrypt = new CryptoNetRsa(invalidKey).DecryptToString(encrypt);
        }
        catch (Exception e)
        {
            Common.CheckContent(Common.ConfidentialDummyData, decrypt).ShouldBeFalse();
            e.Message.ShouldContain("The provided XML could not be read.");
        }
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        var encrypt = cryptoNet.EncryptFromString(Common.ConfidentialDummyData);
        File.WriteAllBytes(Common.EncryptedContentFile, encrypt);

        var encryptFile = File.ReadAllBytes(Common.EncryptedContentFile);
        var content = cryptoNet.DecryptToString(encryptFile);

        Common.CheckContent(Common.ConfidentialDummyData, content).ShouldBeTrue();

        Common.DeleteTestFiles(Common.DummyFiles);
    }

    [TestCase("test.docx"), Order(5)]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Encrypt_Decrypt_Documents_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test(string filename)
    {
        var testDocument = File.ReadAllBytes(Path.Combine(Common.TestFilesPath, filename));

        ICryptoNet cryptoNet = new CryptoNetRsa();
        cryptoNet.ExportKeyAndSave(new FileInfo(Common.PrivateKeyFile), true);
        cryptoNet.ExportKeyAndSave(new FileInfo(Common.PublicKeyFile), false);

        // arrange
        var encrypt = new CryptoNetRsa(new FileInfo(Common.PublicKeyFile)).EncryptFromBytes(testDocument);

        // act
        var decrypt = new CryptoNetRsa(new FileInfo(Common.PrivateKeyFile)).DecryptToBytes(encrypt);

        // assert
        ClassicAssert.AreEqual(testDocument, decrypt);
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_PreStored_SelfGenerated_AsymmetricKey_Test()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa(new FileInfo(Common.RsaStoredKeyPair));

        var encrypt = cryptoNet.EncryptFromString(Common.ConfidentialDummyData);
        var content = cryptoNet.DecryptToString(encrypt);

        Common.CheckContent(Common.ConfidentialDummyData, content);
    }

    [Test]
    public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Using_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
    {
        var certificate = new FileInfo(Common.RsaStoredKeyPair);
        
        // Export public key
        new CryptoNetRsa(certificate).ExportKeyAndSave(new FileInfo(Common.PublicKeyFile), false);

        // Import public key and encrypt
        var importPublicKey = new FileInfo(Common.PublicKeyFile);

        var encryptWithPublicKey = new CryptoNetRsa(importPublicKey).EncryptFromString(Common.ConfidentialDummyData);
        var decryptWithPrivateKey = new CryptoNetRsa(certificate).DecryptToString(encryptWithPublicKey);

        Common.CheckContent(Common.ConfidentialDummyData, decryptWithPrivateKey);

        Common.DeleteTestFiles(Common.DummyFiles);
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Validate_Decrypted_File_Against_The_Original_File_By_Comparing_Bytes_Test(string filename)
    {
        // arrange
        var certificate = new FileInfo(Common.RsaStoredKeyPair);
        // Export public key
        ICryptoNet cryptoNet = new CryptoNetRsa(certificate);

        var filePath = Path.Combine(Common.TestFilesPath, filename);
        byte[] originalFileBytes = File.ReadAllBytes(filePath);
        byte[] encryptedBytes = cryptoNet.EncryptFromBytes(originalFileBytes);
        byte[] decryptedBytes = cryptoNet.DecryptToBytes(encryptedBytes);

        var isIdenticalFile = CryptoNetExtensions.ByteArrayCompare(originalFileBytes, decryptedBytes);

        isIdenticalFile.ShouldBeTrue();
    }

    [Test]
    public void SelfGenerated_And_Save_AsymmetricKey_Test()
    {
        // Arrange
        var cryptoNet = new CryptoNetRsa();

        // Act
        cryptoNet.ExportKeyAndSave(new FileInfo(PrivateKeyFile), true);
        cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile), false);

        var fileExistsPrivateKey = File.Exists(new FileInfo(PrivateKeyFile).FullName);
        var fileExistsPublicKey = File.Exists(new FileInfo(PublicKeyFile).FullName);

        var encryptedData = new CryptoNetRsa(new FileInfo(PublicKeyFile)).EncryptFromString(ConfidentialData);
        var decryptedData = new CryptoNetRsa(new FileInfo(PrivateKeyFile)).DecryptToString(encryptedData);

        // Assert
        fileExistsPrivateKey.ShouldBeTrue();
        fileExistsPublicKey.ShouldBeTrue();
        ConfidentialData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content_Test()
    {
        // Arrange
        var publicKeyRsa = new CryptoNetRsa(new FileInfo(PublicKeyFile));
        var privateKeyRsa = new CryptoNetRsa(new FileInfo(PrivateKeyFile));

        // Act
        var encryptedData = publicKeyRsa.EncryptFromString(ConfidentialData);
        var decryptedData = privateKeyRsa.DecryptToString(encryptedData);

        // Assert
        ConfidentialData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_Decrypt_Using_X509_Certificate_Test()
    {
        // Arrange
        X509Certificate2? certificate = CryptoNetExtensions.GetCertificateFromStore("CN=Maytham");
        var rsaPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
        var rsaPrivateKey = new CryptoNetRsa(certificate, KeyType.PrivateKey);

        // Act
        var encryptedData = rsaPublicKey.EncryptFromString(ConfidentialData);
        var decryptedData = rsaPrivateKey.DecryptToString(encryptedData);

        // Assert
        ConfidentialData.ShouldBe(decryptedData);

    }

    [Test]
    public void Export_Public_Key_For_X509_Certificate_Test()
    {
        // Arrange
        X509Certificate2? certificate = CryptoNetExtensions.GetCertificateFromStore("CN=Maytham");
        var rsa = new CryptoNetRsa(certificate, KeyType.PublicKey);

        // Act
        var publicKey = rsa.ExportKey(false);

        // Assert
        publicKey.ShouldNotBeNull();
        publicKey.ShouldNotBeEmpty();
    }

    [Test]
    public void Customize_PEM_Key_Encryption_Decryption_Test()
    {
        // Arrange
        X509Certificate2? cert = CryptoNetExtensions.GetCertificateFromStore("CN=Maytham");

        var pubKeyPem = ExportPemKey(cert!, false);
        var priKeyPem = ExportPemKey(cert!);
        var password = "password";
        var encryptedPriKeyBytes = ExportPemKeyWithPassword(cert!, password);

        // Act
        ICryptoNet cryptoNet1 = ImportPemKeyWithPassword(encryptedPriKeyBytes, password);
        var encryptedData1 = cryptoNet1.EncryptFromString(ConfidentialData);

        ICryptoNet cryptoNet2 = ImportPemKey(pubKeyPem);
        var encryptedData2 = cryptoNet2.EncryptFromString(ConfidentialData);

        ICryptoNet cryptoNet3 = ImportPemKey(priKeyPem);
        var decryptedData1 = cryptoNet3.DecryptToString(encryptedData1);
        var decryptedData2 = cryptoNet3.DecryptToString(encryptedData2);

        // Assert
        ConfidentialData.ShouldBe(decryptedData1);
        ConfidentialData.ShouldBe(decryptedData2);
    }

    private static char[] ExportPemKey(X509Certificate2 cert, bool privateKey = true)
    {
        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;

        if (privateKey)
        {
            byte[] priKeyBytes = rsa.ExportPkcs8PrivateKey();
            return PemEncoding.Write("PRIVATE KEY", priKeyBytes);
        }

        byte[] pubKeyBytes = rsa.ExportSubjectPublicKeyInfo();
        return PemEncoding.Write("PUBLIC KEY", pubKeyBytes);
    }

    private static ICryptoNet ImportPemKey(char[] key)
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail!.Rsa?.ImportFromPem(key);
        return cryptoNet;
    }

    private static byte[] ExportPemKeyWithPassword(X509Certificate2 cert, string password)
    {
        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;
        byte[] pass = Encoding.UTF8.GetBytes(password);
        return rsa.ExportEncryptedPkcs8PrivateKey(pass,
            new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, iterationCount: 100_000));
    }

    private static ICryptoNet ImportPemKeyWithPassword(byte[] encryptedPrivateKey, string password)
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail?.Rsa?.ImportEncryptedPkcs8PrivateKey(password, encryptedPrivateKey, out _);
        return cryptoNet;
    }

}
