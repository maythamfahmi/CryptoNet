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
            ICryptoNetRsa cryptoNet = new CryptoNetRsa();
            cryptoNet.SaveKey(new FileInfo(PrivateKeyFile), true);
            cryptoNet.SaveKey(new FileInfo(PublicKeyFile), false);
        }

        [Test]
        public static void Test()
        {
            Console.WriteLine(RuntimeInformation.OSDescription);
            Console.WriteLine(RuntimeInformation.OSArchitecture);
            true.ShouldBeTrue();
        }

        

    }
}
#endif