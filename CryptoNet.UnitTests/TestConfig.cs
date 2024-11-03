using CryptoNet.ExtPack.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
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

        public static X509Certificate2 CreateSelfSignedCertificate()
        {
            using var rsa = RSA.Create(2048); // Generate a new RSA key pair for the certificate
            var request = new CertificateRequest(
                "CN=TestCertificate",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            // Add extensions (e.g., for key usage, if needed)
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature,
                    critical: true
                )
            );

            // Create a self-signed certificate that is valid for one year
            var certificate = request.CreateSelfSigned(
                DateTimeOffset.Now.AddDays(-1),
                DateTimeOffset.Now.AddYears(1)
            );

            return certificate;
        }
    }
}
