// <copyright file="CryptoNetTests.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using CryptoNet.Models;
using CryptoNet.Share;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Shouldly;
using CryptoNet.Extensions;

// ReSharper disable All

namespace CryptoNet.UnitTests;

[TestFixture]
public class CryptoNetRsaTests
{
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

}
