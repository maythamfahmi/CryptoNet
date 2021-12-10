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

        private readonly CryptoNet _cryptoNet;

        public CryptoNetTests()
        {
            _cryptoNet = new CryptoNet(Key);
        }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            //
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            Delete_Test_Files();
        }

        [Test, Order(1)]
        public void Create_SelfAssignedKeys_Encrypt_Decrypt_Test()
        {
            _cryptoNet.InitAsymmetricKeys();
            var encrypt = _cryptoNet.Encrypt(JsonDummyData);
            var decrypt = _cryptoNet.Decrypt(encrypt);
            CheckContent(JsonDummyData, decrypt).ShouldBeTrue();
        }

        [Test, Order(2)]
        public void Try_Decrypt_WithOut_SelfAssigned_PrivateKey_Test()
        {
            try
            {
                var encrypt = _cryptoNet.Encrypt(JsonDummyData);
                _cryptoNet.ImportKey("");
                _cryptoNet.Decrypt(encrypt);
            }
            catch (Exception e)
            {
                e.Message.ShouldStartWith("The provided XML could not be read.");
            }
        }

        [Test, Order(3)]
        public void Decrypt_Content_Using_SelfAssignedKeys_Test()
        {
            _cryptoNet.InitAsymmetricKeys();
            var encrypt = _cryptoNet.Encrypt(JsonDummyData);
            var decrypt = _cryptoNet.Decrypt(encrypt);
            CheckContent(JsonDummyData, decrypt).ShouldBeTrue();
        }

        [Test, Order(4)]
        public void Export_And_Reimport_Key_As_File_Test()
        {
            _cryptoNet.InitAsymmetricKeys();
            SaveKey();
            var encryptFile = _cryptoNet.Load(_encryptFile);
            var text = _cryptoNet.Decrypt(encryptFile);
            CheckContent(JsonDummyData, text).ShouldBeTrue();
        }

        [Test, Order(5)]
        public void Validate_PublicKey_Test()
        {
            _cryptoNet.InitAsymmetricKeys();
            var exportedKey = _cryptoNet.ExportPublicKey();
            exportedKey.ShouldContain("RSAKeyValue");
            exportedKey.ShouldContain("Modulus");
            exportedKey.ShouldContain("Exponent");
            exportedKey.ShouldNotContain("<P>");
            exportedKey.ShouldNotContain("<DP>");
            exportedKey.ShouldNotContain("<DQ>");
            exportedKey.ShouldNotContain("<InverseQ>");
            exportedKey.ShouldNotContain("<D>");
            var key = _cryptoNet.ImportKey(exportedKey);
            key.ShouldBe(KeyHelper.KeyType.PublicOnly);
        }

        [Test, Order(6)]
        public void Validate_Private_Key_Test()
        {
            _cryptoNet.InitAsymmetricKeys();
            var exportedKey = _cryptoNet.ExportPrivateKey();
            exportedKey.ShouldContain("RSAKeyValue");
            exportedKey.ShouldContain("Modulus");
            exportedKey.ShouldContain("Exponent");
            exportedKey.ShouldContain("<P>");
            exportedKey.ShouldContain("<DP>");
            exportedKey.ShouldContain("<DQ>");
            exportedKey.ShouldContain("<InverseQ>");
            exportedKey.ShouldContain("<D>");
            var key = _cryptoNet.ImportKey(exportedKey);
            key.ShouldBe(KeyHelper.KeyType.FullKeyPair);
        }

        [Test, Order(7)]
        public void Load_And_Import_PublicKey_And_Encrypt_Content_Test()
        {
            _cryptoNet.InitAsymmetricKeys();
            SaveAndExportPublicKey();
            var publicKey = _cryptoNet.LoadKey(_publicKeyFile);
            _cryptoNet.ImportKey(publicKey);
            var encrypt = _cryptoNet.Encrypt(JsonDummyData);
            _cryptoNet.Save(_encryptFile, encrypt);
        }


        [Test, Order(10)]
        public void Load_And_Import_PrivateKey_And_Decrypt_Content_Test()
        {
            _cryptoNet.InitAsymmetricKeys();
            SaveKey();
            SaveAndExportPrivateKey();
            var privateKey = _cryptoNet.LoadKey(_privateKeyFile);
            _cryptoNet.ImportKey(privateKey);
            var encrypt = _cryptoNet.Load(_encryptFile);
            var text = _cryptoNet.Decrypt(encrypt);
            CheckContent(JsonDummyData, text);
        }

        [Test, Order(11)]
        public void Load_And_Import_PrivateKey_From_Source()
        {
            _cryptoNet.InitAsymmetricKeys();
            SaveKey();
            var privateKey = _cryptoNet.LoadKey(_privateKey);
            _cryptoNet.ImportKey(privateKey);
            var encrypt = _cryptoNet.Load(_encryptFile);
            var text = _cryptoNet.Decrypt(encrypt);
            CheckContent(JsonDummyData, text);
        }

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

        private void SaveKey()
        {
            _cryptoNet.ExportPublicKey();
            var encrypt = _cryptoNet.Encrypt(JsonDummyData);
            _cryptoNet.Save(_encryptFile, encrypt);
        }

        private void SaveAndExportPrivateKey()
        {
            var privateKey = _cryptoNet.ExportPrivateKey();
            _cryptoNet.SaveKey(_privateKeyFile, privateKey);
        }

        private void SaveAndExportPublicKey()
        {
            var publicKey = _cryptoNet.ExportPublicKey();
            _cryptoNet.SaveKey(_publicKeyFile, publicKey);
        }

        private const string JsonDummyData = @"[
                                          {
                                            ""_id"": ""5bdc13aed21f6099814cb30a"",
                                            ""index"": 0,
                                            ""guid"": ""e7c4a8fd-1951-4fda-9081-1e73dc6553c2"",
                                            ""isActive"": true,
                                            ""balance"": ""$3,401.39"",
                                            ""picture"": ""http://placehold.it/32x32"",
                                            ""age"": 36,
                                            ""eyeColor"": ""blue"",
                                            ""name"": {
                                              ""first"": ""Nichole"",
                                              ""last"": ""Thornton""
                                            },
                                            ""company"": ""OPTYK"",
                                            ""email"": ""nichole.thornton@optyk.us"",
                                            ""phone"": ""+1 (853) 441-2456"",
                                            ""address"": ""133 Willow Street, Garberville, Guam, 2323"",
                                            ""about"": ""Non ullamco dolor do do qui ipsum veniam proident nostrud sit id ea. Veniam in cillum qui laborum. Irure nulla ad exercitation qui cillum. In aliqua officia id anim veniam est velit sint tempor laboris incididunt magna dolor. Quis consequat adipisicing laborum fugiat laboris veniam velit et. Irure id sit est minim dolor do anim sunt."",
                                            ""registered"": ""Sunday, December 21, 2014 7:56 PM"",
                                            ""latitude"": ""-86.760521"",
                                            ""longitude"": ""146.6825"",
                                            ""tags"": [
                                              ""minim"",
                                              ""dolor"",
                                              ""voluptate"",
                                              ""fugiat"",
                                              ""commodo""
                                            ],
                                            ""range"": [
                                              0,
                                              1,
                                              2,
                                              3,
                                              4,
                                              5,
                                              6,
                                              7,
                                              8,
                                              9
                                            ],
                                            ""friends"": [
                                              {
                                                ""id"": 0,
                                                ""name"": ""Pam Kelly""
                                              },
                                              {
                                                ""id"": 1,
                                                ""name"": ""Angelique Velez""
                                              },
                                              {
                                                ""id"": 2,
                                                ""name"": ""Bonita Bernard""
                                              }
                                            ],
                                            ""greeting"": ""Hello, Nichole! You have 10 unread messages."",
                                            ""favoriteFruit"": ""banana""
                                          }
                                        ]";

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
    }
}
