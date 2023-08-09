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
    public class CryptoNetAesTests
    {
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
                var encryptData = encryptClient.EncryptFromString(Common.ConfidentialDummyData);
                var decryptData = decryptClient.DecryptToString(encryptData);

                // assert
                Common.ConfidentialDummyData.ShouldBe(decryptData);
                encryptClient.Info.KeyType.ShouldBe(KeyType.SymmetricKey);
                encryptClient.Info.KeyType.ShouldNotBe(KeyType.PublicKey);
                encryptClient.Info.KeyType.ShouldNotBe(KeyType.PrivateKey);
                encryptClient.Info.KeyType.ShouldNotBe(KeyType.NotSet);
            }
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
            var encryptData = encryptClient.EncryptFromString(Common.ConfidentialDummyData);
            try
            {
                decryptClient.DecryptToString(encryptData);
            }
            catch (Exception e)
            {
                // assert
                e.Message.ShouldBe("Padding is invalid and cannot be removed.");
            }
        }

        [TestCase("test.docx")]
        [TestCase("test.xlsx")]
        [TestCase("test.png")]
        [TestCase("test.pdf")]
        public void Validate_Decrypted_File_Against_The_Original_File_By_Comparing_Bytes_Test(string filename)
        {
            // arrange
            var key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
            var iv = new byte[16];

            ICryptoNet encryptClient = new CryptoNetAes(key, iv);
            ICryptoNet decryptClient = new CryptoNetAes(key, iv);

            // act
            var filePath = Path.Combine(Common.TestFilesFolder, filename);
            byte[] originalFileBytes = File.ReadAllBytes(filePath);
            byte[] encrypted = encryptClient.EncryptFromBytes(originalFileBytes);
            byte[] decrypted = decryptClient.DecryptToBytes(encrypted);

            var isIdenticalFile = CryptoNetUtils.ByteArrayCompare(originalFileBytes, decrypted);

            // assert
            isIdenticalFile.ShouldBeTrue();
        }

    }

}
