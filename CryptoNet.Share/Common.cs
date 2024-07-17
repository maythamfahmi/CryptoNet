using System.Security.Cryptography;
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
    public static readonly string PrivateKeyFile = Path.Combine(RsaKeysPath, "privateKey");
    public static readonly string PublicKeyFile = Path.Combine(RsaKeysPath, "publicKey.pub");
    public static readonly string[] DummyFiles =
    [
        EncryptedContentFile,
        PublicKeyFile,
        PrivateKeyFile
    ];

    #region Private methods
    public static void DeleteTestFiles(string[] files)
    {
        Thread.Sleep(500);
        foreach (string file in files)
        {
            File.Delete(file);
        }
    }

    public static bool CheckContent(string originalContent, string decryptedContent)
    {
        return CalculateMd5(originalContent).Equals(CalculateMd5(decryptedContent));
    }

    public static string CalculateMd5(string content)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(content));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
    #endregion
}
