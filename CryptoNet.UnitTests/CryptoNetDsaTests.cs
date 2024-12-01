#if !MACOS
// <copyright file="CryptoNetDsaTests.cs" company="NextBix" year="2024">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2024</date>
// <summary>Unit tests for the CryptoNetDsa class, part of CryptoNet project</summary>

using CryptoNet.ExtPack;
using CryptoNet.Models;

using NUnit.Framework;
using NUnit.Framework.Legacy;

using Shouldly;

using System.Text;
using System;
using System.IO;
using System.Linq;
using Moq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CryptoNet.UnitTests
{
    [TestFixture]
    public class CryptoNetDsaTests
    {
        private static readonly string BaseFolder = AppContext.BaseDirectory;
        private static readonly string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
        private static readonly string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

        public CryptoNetDsaTests()
        {
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();
            cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
            cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);
        }

        [Test]
        public void SelfGenerated_AsymmetricKey_And_TypeValidation_Test()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            } 

            // Arrange & Act
            var privateKeyCrypto = new CryptoNetDsa(new FileInfo(PrivateKeyFile));
            var publicKeyCrypto = new CryptoNetDsa(new FileInfo(PublicKeyFile));

            // Assert
            privateKeyCrypto.Info.KeyType.ShouldBe(KeyType.PrivateKey);
            publicKeyCrypto.Info.KeyType.ShouldBe(KeyType.PublicKey);
        }

        [Test]
        public void Create_And_Verify_Signature_Test()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            }

            // Arrange
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();
            string testMessage = "Test message for signature";
            var privateKey = cryptoNet.GetKey(true);

            // Act
            var signature = cryptoNet.CreateSignature(testMessage);
            var isVerified = new CryptoNetDsa(privateKey).IsContentVerified(testMessage, signature);

            // Assert
            isVerified.ShouldBeTrue();
        }

        [Test]
        public void Verify_Invalid_Signature_Test()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            }

            // Arrange
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();
            string testMessage = "Test message for signature";
            string invalidMessage = "Different message";

            // Act
            var signature = cryptoNet.CreateSignature(testMessage);
            var isVerified = cryptoNet.IsContentVerified(invalidMessage, signature);

            // Assert
            isVerified.ShouldBeFalse();
        }

        [Test]
        public void Save_And_Load_Keys_Test()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            }

            // Arrange
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();
            string privateKeyPath = Path.Combine(BaseFolder, "savedPrivateKey");
            string publicKeyPath = Path.Combine(BaseFolder, "savedPublicKey");

            // Act
            cryptoNet.SaveKey(privateKeyPath, true);
            cryptoNet.SaveKey(publicKeyPath, false);

            var loadedPrivateKeyCrypto = new CryptoNetDsa(new FileInfo(privateKeyPath));
            var loadedPublicKeyCrypto = new CryptoNetDsa(new FileInfo(publicKeyPath));

            // Assert
            loadedPrivateKeyCrypto.Info.KeyType.ShouldBe(KeyType.PrivateKey);
            loadedPublicKeyCrypto.Info.KeyType.ShouldBe(KeyType.PublicKey);
        }

        [Test]
        public void CreateSignature_With_Empty_Content_Should_Throw_Exception()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            }

            // Arrange
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => cryptoNet.CreateSignature(string.Empty));
        }

        [Test]
        public void VerifySignature_With_Null_Bytes_Should_Throw_Exception()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            }

            // Arrange
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => cryptoNet.IsContentVerified((byte[])null!, new byte[0]));
        }

        [Test]
        public void Key_Type_Check_ShouldReturnCorrectType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            }

            // Arrange
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();

            // Act
            var keyType = cryptoNet.Info.KeyType;

            // Assert
            keyType.ShouldBe(KeyType.PrivateKey);
        }

        [Test]
        public void SelfGenerated_Signature_And_Verification_With_Stored_Keys_Test()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.Inconclusive("Test not applicable on macOS.");
            }

            // Arrange
            ICryptoNetDsa cryptoNet = new CryptoNetDsa();
            string testMessage = "Message for signing";
            cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
            cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);

            var loadedPrivateKeyCrypto = new CryptoNetDsa(new FileInfo(PrivateKeyFile));
            var loadedPublicKeyCrypto = new CryptoNetDsa(new FileInfo(PublicKeyFile));

            // Act
            var signature = loadedPrivateKeyCrypto.CreateSignature(testMessage);
            var isVerified = loadedPublicKeyCrypto.IsContentVerified(testMessage, signature);

            // Assert
            isVerified.ShouldBeTrue();
        }

    }
}
#endif