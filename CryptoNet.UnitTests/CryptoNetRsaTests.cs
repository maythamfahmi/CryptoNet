// <copyright file="CryptoNetTests.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using System.Text;
using CryptoNet.Models;
using CryptoNet.Utils;
using NUnit.Framework;
using Shouldly;

// ReSharper disable All

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class CryptoNetRsaTests
    {
        [Test]
        public void SelfGenerated_AsymmetricKey_And_TypeValidation_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNetRsa();

            // act
            cryptoNet.ExportKeyAndSave(new FileInfo(Common.PrivateKeyFile), true);
            cryptoNet.ExportKeyAndSave(new FileInfo(Common.PublicKeyFile), false);

            // assert
            new CryptoNetRsa(new FileInfo(Common.PrivateKeyFile)).Info.KeyType.ShouldBe(KeyType.PrivateKey);
            new CryptoNetRsa(new FileInfo(Common.PublicKeyFile)).Info.KeyType.ShouldBe(KeyType.PublicKey);
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_Test()
        {
            ICryptoNet cryptoNet = new CryptoNetRsa();
            var privateKey = cryptoNet.ExportKey(true);
            var publicKey = cryptoNet.ExportKey(false);

            // arrange
            ICryptoNet encryptClient = new CryptoNetRsa(publicKey);
            var encrypt = encryptClient.EncryptFromString(Common.ConfidentialDummyData);

            // act
            ICryptoNet decryptClient = new CryptoNetRsa(privateKey);
            var decrypt = decryptClient.DecryptToString(encrypt);

            // assert
            Common.CheckContent(Common.ConfidentialDummyData, decrypt).ShouldBeTrue();
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_Invalid_AsymmetricKey_Test()
        {
            // arrange
            ICryptoNet encryptClient = new CryptoNetRsa();
            var encrypt = encryptClient.EncryptFromString(Common.ConfidentialDummyData);

            // act
            const string invalidKey = "invalid-key";
            string decrypt = string.Empty;
            try
            {
                ICryptoNet decryptClient = new CryptoNetRsa(invalidKey);
                decrypt = decryptClient.DecryptToString(encrypt);
            }
            catch (Exception e)
            {
                // assert
                Common.CheckContent(Common.ConfidentialDummyData, decrypt).ShouldBeFalse();
                e.Message.ShouldContain("The provided XML could not be read.");
            }
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNetRsa();
            var encrypt = cryptoNet.EncryptFromString(Common.ConfidentialDummyData);
            File.WriteAllBytes(Common.EncryptedContentFile, encrypt);

            // act
            var encryptFile = File.ReadAllBytes(Common.EncryptedContentFile);
            var content = cryptoNet.DecryptToString(encryptFile);

            // assert
            Common.CheckContent(Common.ConfidentialDummyData, content).ShouldBeTrue();

            // finalize
            Common.DeleteTestFiles(Common.DummyFiles);
        }

        [TestCase("test.docx"), Order(5)]
        [TestCase("test.xlsx")]
        [TestCase("test.png")]
        [TestCase("test.pdf")]
        public void Encrypt_Decrypt_Documents_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test(string filename)
        {
            var testDocument = File.ReadAllBytes(Path.Combine(Common.TestFilesFolder, filename));

            ICryptoNet cryptoNet = new CryptoNetRsa();
            cryptoNet.ExportKeyAndSave(new FileInfo(Common.PrivateKeyFile), true);
            cryptoNet.ExportKeyAndSave(new FileInfo(Common.PublicKeyFile), false);

            // arrange
            ICryptoNet encryptClient = new CryptoNetRsa(new FileInfo(Common.PublicKeyFile));
            var encrypt = encryptClient.EncryptFromBytes(testDocument);

            // act
            ICryptoNet decryptClient = new CryptoNetRsa(new FileInfo(Common.PrivateKeyFile));
            var decrypt = decryptClient.DecryptToBytes(encrypt);

            // assert
            Assert.AreEqual(testDocument, decrypt);
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_PreStored_SelfGenerated_AsymmetricKey_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNetRsa(new FileInfo(Common.RsaStoredKeyPair));

            // act
            var encrypt = cryptoNet.EncryptFromString(Common.ConfidentialDummyData);
            var content = cryptoNet.DecryptToString(encrypt);

            // assert
            Common.CheckContent(Common.ConfidentialDummyData, content);
        }

        [Test]
        public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Using_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
        {
            // arrange
            var certificate = new FileInfo(Common.RsaStoredKeyPair);
            // Export public key
            ICryptoNet cryptoNet = new CryptoNetRsa(certificate);
            cryptoNet.ExportKeyAndSave(new FileInfo(Common.PublicKeyFile), false);

            // Import public key and encrypt
            var importPublicKey = new FileInfo(Common.PublicKeyFile);
            ICryptoNet cryptoNetEncryptWithPublicKey = new CryptoNetRsa(importPublicKey);
            var encryptWithPublicKey = cryptoNetEncryptWithPublicKey.EncryptFromString(Common.ConfidentialDummyData);

            // act
            ICryptoNet cryptoNetDecryptWithPublicKey = new CryptoNetRsa(certificate);
            var decryptWithPrivateKey = cryptoNetDecryptWithPublicKey.DecryptToString(encryptWithPublicKey);

            // assert
            Common.CheckContent(Common.ConfidentialDummyData, decryptWithPrivateKey);

            // finalize
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

            // act
            var filePath = Path.Combine(Common.TestFilesFolder, filename);
            byte[] originalFileBytes = File.ReadAllBytes(filePath);
            byte[] encrypted = cryptoNet.EncryptFromBytes(originalFileBytes);
            byte[] decrypted = cryptoNet.DecryptToBytes(encrypted);

            var isIdenticalFile = CryptoNetUtils.ByteArrayCompare(originalFileBytes, decrypted);

            // assert
            isIdenticalFile.ShouldBeTrue();
        }

    }

}
