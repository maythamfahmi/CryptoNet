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
        var encrypt = new CryptoNetRsa(publicKey).EncryptFromString(Common.ConfidentialDummyData);
        var decrypt = new CryptoNetRsa(privateKey).DecryptToString(encrypt);

        // Assert
        Common.CheckContent(Common.ConfidentialDummyData, decrypt).ShouldBeTrue();
    }

    [Test]
    public void Encrypt_Decrypt_Content_With_Invalid_AsymmetricKey_Test()
    {
        // Arrange
        var encrypt = new CryptoNetRsa().EncryptFromString(Common.ConfidentialDummyData);
        const string invalidKey = "invalid-key";
        string decrypt = string.Empty;

        // Act & Assert
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
        // Arrange
        ICryptoNetRsa cryptoNet = new CryptoNetRsa();
        var encrypt = cryptoNet.EncryptFromString(Common.ConfidentialDummyData);
        File.WriteAllBytes(Common.EncryptedContentFile, encrypt);

        // Act
        var encryptFile = File.ReadAllBytes(Common.EncryptedContentFile);
        var content = cryptoNet.DecryptToString(encryptFile);

        // Assert
        Common.CheckContent(Common.ConfidentialDummyData, content).ShouldBeTrue();
    }

    [TestCase("test.docx"), Order(5)]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Encrypt_Decrypt_Documents_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test(string filename)
    {
        // Arrange
        var testDocument = File.ReadAllBytes(Path.Combine(Common.TestFilesPath, filename));
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
        ICryptoNetRsa cryptoNet = new CryptoNetRsa(new FileInfo(Common.RsaStoredKeyPair));

        // Act
        var encrypt = cryptoNet.EncryptFromString(Common.ConfidentialDummyData);
        var content = cryptoNet.DecryptToString(encrypt);

        // Assert
        Common.CheckContent(Common.ConfidentialDummyData, content).ShouldBeTrue();
    }

    [Test]
    public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Using_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
    {
        // Arrange
        var certificate = new FileInfo(Common.RsaStoredKeyPair);
        new CryptoNetRsa(certificate).SaveKey(new FileInfo(PublicKeyFile), false);

        // Act
        var importPublicKey = new FileInfo(PublicKeyFile);
        var encryptWithPublicKey = new CryptoNetRsa(importPublicKey).EncryptFromString(Common.ConfidentialDummyData);
        var decryptWithPrivateKey = new CryptoNetRsa(certificate).DecryptToString(encryptWithPublicKey);

        // Assert
        Common.CheckContent(Common.ConfidentialDummyData, decryptWithPrivateKey).ShouldBeTrue();
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Validate_Decrypted_File_Against_The_Original_File_By_Comparing_Bytes_Test(string filename)
    {
        // Arrange
        var certificate = new FileInfo(Common.RsaStoredKeyPair);
        ICryptoNetRsa cryptoNet = new CryptoNetRsa(certificate);
        var filePath = Path.Combine(Common.TestFilesPath, filename);
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
        var encryptedData = publicKeyRsa.EncryptFromString(Common.ConfidentialDummyData);
        var decryptedData = privateKeyRsa.DecryptToString(encryptedData);

        // Assert
        Common.ConfidentialDummyData.ShouldBe(decryptedData);
    }

    [Test]
    public void Encrypt_Decrypt_Using_X509_Certificate_Test()
    {
        // Arrange
        // You can change to test real system certificate by using CryptoNetExtensions.GetCertificateFromStore("CN=MaythamCertificateName")
        X509Certificate2 ? certificate = CreateSelfSignedCertificate();
        var rsaPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
        var rsaPrivateKey = new CryptoNetRsa(certificate, KeyType.PrivateKey);

        // Act
        var encryptedData = rsaPublicKey.EncryptFromString(Common.ConfidentialDummyData);
        var decryptedData = rsaPrivateKey.DecryptToString(encryptedData);

        // Assert
        Common.ConfidentialDummyData.ShouldBe(decryptedData);

    }

    [Test]
    public void Export_Public_Key_For_X509_Certificate_Test()
    {
        // Arrange
        // You can change to test real system certificate by using CryptoNetExtensions.GetCertificateFromStore("CN=MaythamCertificateName")
        X509Certificate2? certificate = CreateSelfSignedCertificate();
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
        X509Certificate2? certificate = CreateSelfSignedCertificate();

        var pubKeyPem = Common.ExportPemKey(certificate!, false);
        var priKeyPem = Common.ExportPemKey(certificate!);
        var password = "password";
        var encryptedPriKeyBytes = Common.ExportPemKeyWithPassword(certificate!, password);

        // Act
        ICryptoNetRsa cryptoNet1 = ImportPemKeyWithPassword(encryptedPriKeyBytes, password);
        var encryptedData1 = cryptoNet1.EncryptFromString(Common.ConfidentialDummyData);

        ICryptoNetRsa cryptoNet2 = ImportPemKey(pubKeyPem);
        var encryptedData2 = cryptoNet2.EncryptFromString(Common.ConfidentialDummyData);

        ICryptoNetRsa cryptoNet3 = ImportPemKey(priKeyPem);
        var decryptedData1 = cryptoNet3.DecryptToString(encryptedData1);
        var decryptedData2 = cryptoNet3.DecryptToString(encryptedData2);

        // Assert
        Common.ConfidentialDummyData.ShouldBe(decryptedData1);
        Common.ConfidentialDummyData.ShouldBe(decryptedData2);
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

    public static X509Certificate2 CreateSelfSignedCertificate()
    {
        using var rsa = RSA.Create(2048); // Generate a new RSA key pair for the certificate
        var request = new CertificateRequest(
            "CN=TestCertificate",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        // Add extensions (e.g., for key usage, if needed)
        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature,
                critical: true
            )
        );

        // Create a self-signed certificate that is valid for one year
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1),
            DateTimeOffset.Now.AddYears(1)
        );

        return certificate;
    }
}
