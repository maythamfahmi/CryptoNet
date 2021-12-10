using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CryptoNetLib;
using CryptoNetLib.helpers;
using NUnit.Framework;
using Shouldly;

namespace CryptoNetUnitTests
{
    [TestFixture]
    public class CryptoNetTests
    {
        private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string? Root = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
        private readonly string _encryptFile = $"{BaseFolder}testfile.enc";
        private readonly string _publicKeyFile = $"{BaseFolder}testkey.pub";
        private readonly string _privateKeyFile = $"{BaseFolder}testkey";
        private readonly string _privateKey = @$"{Root}\test.certificate";
        private const string Key = "any-unique-secret-key";

        [OneTimeSetUp]
        public void GlobalSetup()
        {
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
        }

        [Test, Order(1)]
        public void Encrypt_Decrypt_Content_SelfAssignedKeys_Test()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);
            cryptoNet.InitAsymmetricKeys();
            var encrypt = cryptoNet.Encrypt(ConfidentialDummyData);

            // act
            var decrypt = cryptoNet.Decrypt(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, decrypt).ShouldBeTrue();
        }

        [Test, Order(2)]
        public void Encrypt_Decrypt_Content_WithOut_SelfAssignedKeys_Test()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);
            var encrypt = cryptoNet.Encrypt(ConfidentialDummyData);

            // act
            var decrypt = cryptoNet.Decrypt(encrypt);

            // assert
            decrypt.ShouldBe(KeyHelper.KeyType.NotSet.ToString());
        }

        [Test, Order(3)]
        public void Try_WithOut_PrivateKey_Test()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);

            try
            {
                // act
                cryptoNet.ImportKey("");
            }
            catch (Exception e)
            {
                // assert
                e.Message.ShouldStartWith("The provided XML could not be read.");
            }
        }

        [Test, Order(4)]
        public void Encrypt_Decrypt_File_SelfAssignedKeys_Test()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);
            cryptoNet.InitAsymmetricKeys();
            var encrypt = cryptoNet.Encrypt(ConfidentialDummyData);

            // act
            cryptoNet.Save(_encryptFile, encrypt);
            var encryptFile = cryptoNet.Load(_encryptFile);
            var content = cryptoNet.Decrypt(encryptFile);

            // assert
            CheckContent(ConfidentialDummyData, content).ShouldBeTrue();

            // finalize
            Delete_Test_Files();
        }

        [Test, Order(5)]
        public void ExportPrivateKey_EncryptedAndSaveFile_And_ImportPrivateKey_LoadAndDecryptFile_Test()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);
            cryptoNet.InitAsymmetricKeys();
            var privateKey = cryptoNet.ExportPrivateKey();
            cryptoNet.SaveKey(_privateKeyFile, privateKey);
            var encrypt = cryptoNet.Encrypt(ConfidentialDummyData);
            cryptoNet.Save(_encryptFile, encrypt);

            // act
            CryptoNet cryptoNet1 = new CryptoNet(Key);
            var privateKey1 = cryptoNet1.LoadKey(_privateKeyFile);
            cryptoNet1.ImportKey(privateKey1);
            var encrypt1 = cryptoNet1.Load(_encryptFile);
            var text = cryptoNet1.Decrypt(encrypt1);

            // assert
            CheckContent(ConfidentialDummyData, text);

            // finalize
            Delete_Test_Files();
        }

        [Test, Order(7)]
        public void Load_And_Import_PrivateKey_From_Source()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);

            // act
            var privateKey = cryptoNet.LoadKey(_privateKey);
            cryptoNet.ImportKey(privateKey);
            var encrypt = cryptoNet.Load(_encryptFile);
            var content = cryptoNet.Decrypt(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, content);
        }

        [Test, Order(8)]
        public void Validate_PublicKey_Test()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);
            cryptoNet.InitAsymmetricKeys();

            // act
            var exportedKey = cryptoNet.ExportPublicKey();

            // assert
            exportedKey.ShouldContain("RSAKeyValue");
            exportedKey.ShouldContain("Modulus");
            exportedKey.ShouldContain("Exponent");
            exportedKey.ShouldNotContain("<P>");
            exportedKey.ShouldNotContain("<DP>");
            exportedKey.ShouldNotContain("<DQ>");
            exportedKey.ShouldNotContain("<InverseQ>");
            exportedKey.ShouldNotContain("<D>");
            var key = cryptoNet.ImportKey(exportedKey);
            key.ShouldBe(KeyHelper.KeyType.PublicOnly);
        }

        [Test, Order(9)]
        public void Validate_Private_Key_Test()
        {
            // arrange
            CryptoNet cryptoNet = new CryptoNet(Key);
            cryptoNet.InitAsymmetricKeys();

            // act
            var exportedKey = cryptoNet.ExportPrivateKey();

            // assert
            exportedKey.ShouldContain("RSAKeyValue");
            exportedKey.ShouldContain("Modulus");
            exportedKey.ShouldContain("Exponent");
            exportedKey.ShouldContain("<P>");
            exportedKey.ShouldContain("<DP>");
            exportedKey.ShouldContain("<DQ>");
            exportedKey.ShouldContain("<InverseQ>");
            exportedKey.ShouldContain("<D>");
            var key = cryptoNet.ImportKey(exportedKey);
            key.ShouldBe(KeyHelper.KeyType.FullKeyPair);
        }


        #region Private methods

        private void Delete_Test_Files()
        {
            try
            {
                Thread.Sleep(500);
                File.Delete(_encryptFile);
                File.Delete(_publicKeyFile);
                File.Delete(_privateKeyFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private const string ConfidentialDummyData = @"Some Secret Data";

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
