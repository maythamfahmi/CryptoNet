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
using CryptoNet.Share;
using NUnit.Framework;
using Shouldly;
using CryptoNet.Helpers;

// ReSharper disable All

namespace CryptoNet.UnitTests;

[TestFixture]
public class CryptoNetAesTests
{
    [Test]
    public void Encrypt_And_Decrypt_With_SymmetricKey_Test()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            var key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
            var iv = new byte[16];

            var encryptData = new CryptoNetAes(key, iv).EncryptFromString(Common.ConfidentialDummyData);
            var decryptData = new CryptoNetAes(key, iv).DecryptToString(encryptData);

            Common.ConfidentialDummyData.ShouldBe(decryptData);
            new CryptoNetAes(key, iv).Info.KeyType.ShouldBe(KeyType.SymmetricKey);
            new CryptoNetAes(key, iv).Info.KeyType.ShouldNotBe(KeyType.PublicKey);
            new CryptoNetAes(key, iv).Info.KeyType.ShouldNotBe(KeyType.PrivateKey);
            new CryptoNetAes(key, iv).Info.KeyType.ShouldNotBe(KeyType.NotSet);
        }
    }

    [Test]
    public void Encrypt_And_Decrypt_With_Wrong_SymmetricKey_Test()
    {
        var key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
        var keyWrong = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315b1916");
        var iv = new byte[16];

        var encryptData = new CryptoNetAes(key, iv).EncryptFromString(Common.ConfidentialDummyData);
        
        try
        {
            new CryptoNetAes(keyWrong, iv).DecryptToString(encryptData);
        }
        catch (Exception e)
        {
            e.Message.ShouldBe("Padding is invalid and cannot be removed.");
        }
    }

    [TestCase("test.docx")]
    [TestCase("test.xlsx")]
    [TestCase("test.png")]
    [TestCase("test.pdf")]
    public void Validate_Decrypted_File_Against_The_Original_File_By_Comparing_Bytes_Test(string filename)
    {
        var key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
        var iv = new byte[16];

        var filePath = Path.Combine(Common.TestFilesPath, filename);
        byte[] originalFileBytes = File.ReadAllBytes(filePath);
        byte[] encrypted = new CryptoNetAes(key, iv).EncryptFromBytes(originalFileBytes);
        byte[] decrypted = new CryptoNetAes(key, iv).DecryptToBytes(encrypted);

        var isIdenticalFile = CryptoNetHelpers.ByteArrayCompare(originalFileBytes, decrypted);

        isIdenticalFile.ShouldBeTrue();
    }

}
