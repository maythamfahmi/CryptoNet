// <copyright file="CryptoNetTests.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNetUnitTests project</summary>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CryptoNetLib;
using CryptoNetLib.helpers;
using NUnit.Framework;
using Shouldly;
using static CryptoNetLib.helpers.KeyHelper;

namespace CryptoNetUnitTests
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
        
        [Test, Order(1)]
        public void SelfGenerated_AsymmetricKey_And_TypeValidation_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet();

            // act
            CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
            CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());

            // assert
            var privateKey = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
            var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);

            new CryptoNet(privateKey).GetKeyType().ShouldBe(KeyType.PrivateKey);
            new CryptoNet(publicKey).GetKeyType().ShouldBe(KeyType.PublicKey);
        }

        [Test, Order(2)]
        public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_Test()
        {
            ICryptoNet cryptoNet = new CryptoNet();
            var privateKey = cryptoNet.ExportPrivateKey();
            var publicKey = cryptoNet.ExportPublicKey();

            // arrange
            ICryptoNet encryptClient = new CryptoNet(publicKey);
            var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

            // act
            ICryptoNet decryptClient = new CryptoNet(privateKey);
            var decrypt = decryptClient.DecryptToString(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, decrypt).ShouldBeTrue();
        }

        [Test, Order(3)]
        public void Encrypt_Decrypt_Content_With_Invalid_AsymmetricKey_Test()
        {
            // arrange
            ICryptoNet encryptClient = new CryptoNet();
            var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

            // act
            const string invalidKey = "invalid-key";
            string decrypt = string.Empty;
            try
            {
                ICryptoNet decryptClient = new CryptoNet(invalidKey);
                decrypt = decryptClient.DecryptToString(encrypt);
            }
            catch (Exception e)
            {
                // assert
                CheckContent(ConfidentialDummyData, decrypt).ShouldBeFalse();
                e.Message.ShouldContain("The provided XML could not be read.");
            }
        }

        [Test, Order(4)]
        public void Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet();
            var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);
            CryptoNetUtils.SaveKey(EncryptedContentFile, encrypt);

            // act
            var encryptFile = CryptoNetUtils.LoadFileToBytes(EncryptedContentFile);
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

            ICryptoNet cryptoNet = new CryptoNet();
            CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
            CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());
            var privateKey = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
            var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);

            // arrange
            ICryptoNet encryptClient = new CryptoNet(publicKey);
            var encrypt = encryptClient.EncryptFromBytes(testDocument);

            // act
            CryptoNet decryptClient = new CryptoNet(privateKey);
            var decrypt = decryptClient.DecryptToBytes(encrypt);

            // assert
            Assert.AreEqual(testDocument, decrypt);
        }

        [Test, Order(5)]
        public void Encrypt_Decrypt_Content_With_PreStored_SelfGenerated_AsymmetricKey_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet(CryptoNetUtils.LoadFileToString(_rsaStoredKeyPair));

            // act
            var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);
            var content = cryptoNet.DecryptToString(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, content);
        }

        [Test, Order(6)]
        public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Using_SelfGenerated_AsymmetricKey_That_Is_Stored_And_Loaded_Test()
        {
            // arrange
            var certificate = CryptoNetUtils.LoadFileToString(_rsaStoredKeyPair);
            // Export public key
            ICryptoNet cryptoNet = new CryptoNet(certificate);
            var publicKey = cryptoNet.ExportPublicKey();
            CryptoNetUtils.SaveKey(PublicKeyFile, publicKey);

            // Import public key and encrypt
            var importPublicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
            ICryptoNet cryptoNetEncryptWithPublicKey = new CryptoNet(importPublicKey);
            var encryptWithPublicKey = cryptoNetEncryptWithPublicKey.EncryptFromString(ConfidentialDummyData);

            // act
            ICryptoNet cryptoNetDecryptWithPublicKey = new CryptoNet(certificate);
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
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
        #endregion

    }

}
