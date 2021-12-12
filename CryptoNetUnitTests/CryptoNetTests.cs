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
        private const string AsymmetricKey = "any-secret-key-that-should-be-the-same-for-encrypting-and-decrypting";
        private const string ConfidentialDummyData = @"Some Secret Data";

        private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string? Root = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
        private readonly string _rsaKeyPair = Path.Combine(Root ?? string.Empty, "test.certificate");

        internal static string EncryptedContentFile = Path.Combine(BaseFolder, "encrypted.txt");
        internal static string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
        internal static string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");


        [Test, Order(1)]
        public void Encrypt_Decrypt_Content_With_Same_SelfAssignedKeys_Test()
        {
            // arrange
            ICryptoNet encryptClient = new CryptoNet(AsymmetricKey, false);
            var encrypt = encryptClient.Encrypt(ConfidentialDummyData);

            // act
            ICryptoNet decryptClient = new CryptoNet(AsymmetricKey, false);
            var decrypt = decryptClient.Decrypt(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, decrypt).ShouldBeTrue();
        }

        [Test, Order(2)]
        public void Encrypt_Decrypt_Content_With_Wrong_SelfAssignedKeys_Test()
        {
            // arrange
            ICryptoNet encryptClient = new CryptoNet(AsymmetricKey);
            var encrypt = encryptClient.Encrypt(ConfidentialDummyData);

            // act
            const string asymmetricWrongKey = "wrong-secret-key";
            ICryptoNet decryptClient = new CryptoNet(asymmetricWrongKey);
            var decrypt = decryptClient.Decrypt(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, decrypt).ShouldBeFalse();
            decrypt.ShouldBe("The parameter is incorrect.");
        }

        [Test, Order(3)]
        public void Try_With_Missing_Key_Test()
        {
            try
            {
                // arrange and act
                ICryptoNet cryptoNet = new CryptoNet();

            }
            catch (Exception e)
            {
                // assert
                e.Message.ShouldStartWith("Missing Asymmetric Key Or RsaCertificate");
            }
        }

        [Test, Order(4)]
        public void Encrypt_Decrypt_FromFile_With_Same_SelfAssignedKeys_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet(AsymmetricKey);
            var encrypt = cryptoNet.Encrypt(ConfidentialDummyData);
            CryptoNetUtils.SaveKey(EncryptedContentFile, encrypt);

            // act
            var encryptFile = CryptoNetUtils.LoadFileToBytes(EncryptedContentFile);
            var content = cryptoNet.Decrypt(encryptFile);

            // assert
            CheckContent(ConfidentialDummyData, content).ShouldBeTrue();

            // finalize
            Delete_Test_Files();
        }

        [Test, Order(5)]
        public void Encrypt_Decrypt_FromFile_With_RsaCertificate_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet(CryptoNetUtils.LoadFileToString(_rsaKeyPair), true);

            // act
            var encrypt = cryptoNet.Encrypt(ConfidentialDummyData);
            var content = cryptoNet.Decrypt(encrypt);

            // assert
            CheckContent(ConfidentialDummyData, content);
        }

        [Test, Order(6)]
        public void Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Using_SelfGenerated_RsaCertificate_Test()
        {
            // arrange
            var certificate = CryptoNetUtils.LoadFileToString(_rsaKeyPair);
            // Export public key
            ICryptoNet cryptoNet = new CryptoNet(certificate, true);
            var publicKey = cryptoNet.ExportPublicKey();
            CryptoNetUtils.SaveKey(PublicKeyFile, publicKey);

            // Import public key and encrypt
            var importPublicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
            ICryptoNet cryptoNetEncryptWithPublicKey = new CryptoNet(importPublicKey, true);
            var encryptWithPublicKey = cryptoNetEncryptWithPublicKey.Encrypt(ConfidentialDummyData);

            // act
            ICryptoNet cryptoNetDecryptWithPublicKey = new CryptoNet(certificate, true);
            var decryptWithPrivateKey = cryptoNetDecryptWithPublicKey.Decrypt(encryptWithPublicKey);

            // assert
            CheckContent(ConfidentialDummyData, decryptWithPrivateKey);

            // finalize
            Delete_Test_Files();
        }

        [Test, Order(7)]
        public void Validate_PublicKey_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet("AsymmetricKey");

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
        }

        [Test, Order(8)]
        public void Validate_Private_Key_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet("AsymmetricKey");

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
            var key = cryptoNet.GetKeyType();
            key.ShouldBe(KeyType.FullKeyPair);
        }

        [Test, Order(9)]
        public void SelfGenerated_RsaCertificate_Test()
        {
            // arrange
            ICryptoNet cryptoNet = new CryptoNet(AsymmetricKey);

            // act
            CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
            CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());

            // assert
            var certificate = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
            new CryptoNet(certificate, true).GetKeyType().ShouldBe(KeyType.FullKeyPair);

            var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
            new CryptoNet(publicKey, true).GetKeyType().ShouldBe(KeyType.PublicOnly);
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
