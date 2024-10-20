using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CryptoNet.Share.Extensions;

namespace CryptoNet.Share;

public static class Common
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

    public static bool CheckContent(string originalContent, string decryptedContent)
    {
        if (originalContent == null || decryptedContent == null)
        {
            return originalContent == decryptedContent;
        }

        return CalculateMd5(originalContent).Equals(CalculateMd5(decryptedContent));
    }

    public static string CalculateMd5(string content)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(content));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public static string UniqueKeyGenerator(string input)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = MD5.HashData(inputBytes);

        var stringBuilder = new StringBuilder();
        foreach (var byteValue in hashBytes)
        {
            stringBuilder.Append(byteValue.ToString("X2"));
        }
        return stringBuilder.ToString();
    }

    public static char[] ExportPemKey(X509Certificate2 cert, bool privateKey = true)
    {
        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;

        if (privateKey)
        {
            byte[] priKeyBytes = rsa.ExportPkcs8PrivateKey();
            return PemEncoding.Write("PRIVATE KEY", priKeyBytes);
        }

        byte[] pubKeyBytes = rsa.ExportSubjectPublicKeyInfo();
        return PemEncoding.Write("PUBLIC KEY", pubKeyBytes);
    }

    public static byte[] ExportPemKeyWithPassword(X509Certificate2 cert, string password)
    {
        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;
        byte[] pass = Encoding.UTF8.GetBytes(password);
        return rsa.ExportEncryptedPkcs8PrivateKey(pass,
            new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, iterationCount: 100_000));
    }
}
