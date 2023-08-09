// <copyright file="CryptoNetTests.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CryptoNet.Models;
using CryptoNet.Utils;
using NUnit.Framework;
using Shouldly;

// ReSharper disable All

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class CryptoNetTests
    {
        private const string ConfidentialDummyData = @"Some Secret Data";

        private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _rsaStoredKeyPair = Path.Combine(BaseFolder, @"Resources/RsaKeys/RsaKeys");
        private readonly string _testFilesFolder = Path.Combine(BaseFolder, @"Resources/TestFiles/");

        internal static string EncryptedContentFile = Path.Combine(BaseFolder, "encrypted.txt");
        internal static string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
        internal static string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

        [Test]
        public void Encrypt_And_Decrypt_With_SymmetricKey_Test()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // arrange
                var key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
                var iv = new byte[16];

                ICryptoNet encryptClient = new CryptoNetAes(key, iv);
                ICryptoNet decryptClient = new CryptoNetAes(key, iv);

                // act
                var encryptData = encryptClient.EncryptFromString(ConfidentialDummyData);
                var decryptData = decryptClient.DecryptToString(encryptData);

                // assert
                ConfidentialDummyData.ShouldBe(decryptData);
                encryptClient.Info.KeyType.ShouldBe(KeyType.SymmetricKey);
                encryptClient.Info.KeyType.ShouldNotBe(KeyType.PublicKey);
                encryptClient.Info.KeyType.ShouldNotBe(KeyType.PrivateKey);
                encryptClient.Info.KeyType.ShouldNotBe(KeyType.NotSet);
            }
        }

        [Test]
        public void Encrypt_And_Decrypt_PdfFile_With_SymmetricKey_Test()
        {
            ICryptoNet cryptoNet = new CryptoNetAes();
            var key = cryptoNet.ExportKey();

            ICryptoNet encryptClient = new CryptoNetAes(key);
            var pdfFilePath = Path.Combine(_testFilesFolder, $"test.pdf");
            byte[] pdfFileBytes = File.ReadAllBytes(pdfFilePath);
            var encrypt = encryptClient.EncryptFromBytes(pdfFileBytes);

            ICryptoNet decryptClient = new CryptoNetAes(key);
            var decrypt = decryptClient.DecryptToBytes(encrypt);

            var isIdenticalFile = CryptoNetUtils.ByteArrayCompare(pdfFileBytes, decrypt);
            isIdenticalFile.ShouldBeTrue();
        }

        [Test]
        public void Encrypt_And_Decrypt_With_Wrong_SymmetricKey_Test()
        {
            var key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
            var keyWrong = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315b1916");
            var iv = new byte[16];

            // arrange
            ICryptoNet encryptClient = new CryptoNetAes(key, iv);
            ICryptoNet decryptClient = new CryptoNetAes(keyWrong, iv);

            // act
            var encryptData = encryptClient.EncryptFromString(ConfidentialDummyData);
            try
            {
                var decryptData = decryptClient.DecryptToString(encryptData);
            }
            catch (Exception e)
            {
                // assert
                e.Message.ShouldBe("Padding is invalid and cannot be removed.");
            }
        }

        [Test]
        public void SelfGenerated_AsymmetricKey_And_TypeValidation_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNetRsa();

            // act
            cryptoNet.ExportKeyAndSave(new FileInfo(PrivateKeyFile), true);
            cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile), false);

            // assert
            new CryptoNetRsa(new FileInfo(PrivateKeyFile)).Info.KeyType.ShouldBe(KeyType.PrivateKey);
            new CryptoNetRsa(new FileInfo(PublicKeyFile)).Info.KeyType.ShouldBe(KeyType.PublicKey);
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_Test()
        {
            ICryptoNet cryptoNet = new CryptoNetRsa();
            var privateKey = cryptoNet.ExportKey(true);
            var publicKey = cryptoNet.ExportKey(false);

            // arrange
            ICryptoNet encryptClient = new CryptoNetRsa(publicKey);
            var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

            // act
            ICryptoNet decryptClient = new CryptoNetRsa(privateKey);
            var decrypt = decryptClient.DecryptToString(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, decrypt).ShouldBeTrue();
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_Invalid_AsymmetricKey_Test()
        {
            // arrange
            ICryptoNet encryptClient = new CryptoNetRsa();
            var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

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
                CheckContent(ConfidentialDummyData, decrypt).ShouldBeFalse();
                e.Message.ShouldContain("The provided XML could not be read.");
            }
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNetRsa();
            var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);
            File.WriteAllBytes(EncryptedContentFile, encrypt);

            // act
            var encryptFile = File.ReadAllBytes(EncryptedContentFile);
            var content = cryptoNet.DecryptToString(encryptFile);

            // assert
            CheckContent(ConfidentialDummyData, content).ShouldBeTrue();

            // finalize
            Delete_Test_Files();
        }

        [TestCase("WordDocument.docx"), Order(5)]
        [TestCase("ExcelDocument.xlsx")]
        [TestCase("Image.png")]
        public void Encrypt_Decrypt_Documents_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test(string filename)
        {
            var testDocument = File.ReadAllBytes(Path.Combine(_testFilesFolder, filename));

            ICryptoNet cryptoNet = new CryptoNetRsa();
            cryptoNet.ExportKeyAndSave(new FileInfo(PrivateKeyFile), true);
            cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile), false);

            // arrange
            ICryptoNet encryptClient = new CryptoNetRsa(new FileInfo(PublicKeyFile));
            var encrypt = encryptClient.EncryptFromBytes(testDocument);

            // act
            ICryptoNet decryptClient = new CryptoNetRsa(new FileInfo(PrivateKeyFile));
            var decrypt = decryptClient.DecryptToBytes(encrypt);

            // assert
            Assert.AreEqual(testDocument, decrypt);
        }

        [Test]
        public void Encrypt_Decrypt_Content_With_PreStored_SelfGenerated_AsymmetricKey_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNetRsa(new FileInfo(_rsaStoredKeyPair));

            // act
            var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);
            var content = cryptoNet.DecryptToString(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, content);
        }

        [Test]
        public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Using_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
        {
            // arrange
            var certificate = new FileInfo(_rsaStoredKeyPair);
            // Export public key
            ICryptoNet cryptoNet = new CryptoNetRsa(certificate);
            cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile), false);

            // Import public key and encrypt
            var importPublicKey = new FileInfo(PublicKeyFile);
            ICryptoNet cryptoNetEncryptWithPublicKey = new CryptoNetRsa(importPublicKey);
            var encryptWithPublicKey = cryptoNetEncryptWithPublicKey.EncryptFromString(ConfidentialDummyData);

            // act
            ICryptoNet cryptoNetDecryptWithPublicKey = new CryptoNetRsa(certificate);
            var decryptWithPrivateKey = cryptoNetDecryptWithPublicKey.DecryptToString(encryptWithPublicKey);

            // assert
            CheckContent(ConfidentialDummyData, decryptWithPrivateKey);

            // finalize
            Delete_Test_Files();
        }

        #region Private methods
        private static void Delete_Test_Files()
        {
            try
            {
                Thread.Sleep(500);
                File.Delete(EncryptedContentFile);
                File.Delete(PublicKeyFile);
                File.Delete(PrivateKeyFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool CheckContent(string originalContent, string decryptedContent)
        {
            return CalculateMd5(originalContent).Equals(CalculateMd5(decryptedContent));
        }

        private static string CalculateMd5(string content)
        {
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(content));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        #endregion

    }

}
