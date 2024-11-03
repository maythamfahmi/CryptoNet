using CryptoNet.ExtPack.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoNet.Shared
{
    internal static class TestConfig
    {
        public const string ConfidentialDummyData = @"Some Secret Data";

        private static readonly DirectoryInfo? WorkingDirectory = DirectoryExension.TryGetSolutionDirectoryInfo();
        public static readonly string ResourcePath = $"{WorkingDirectory}/Resources";
        public static readonly string TestFilesPath = Path.Combine($"{ResourcePath}", "TestFiles");
        public static readonly string RsaKeysPath = Path.Combine($"{ResourcePath}", "RsaKeys");

        public static readonly string RsaStoredKeyPair = Path.Combine(RsaKeysPath, "RsaKeys");
        public static readonly string EncryptedContentFile = Path.Combine(RsaKeysPath, "encrypted.txt");
        public static readonly string[] DummyFiles =
        [
            EncryptedContentFile
        ];
    }
}
