using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CryptoNet.UnitTests
{
    internal static class Common
    {
        public const string ConfidentialDummyData = @"Some Secret Data";

        public static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        public static string RsaStoredKeyPair = Path.Combine(BaseFolder, @"Resources/RsaKeys/RsaKeys");
        public static string TestFilesFolder = Path.Combine(BaseFolder, @"Resources/TestFiles/");

        public static string EncryptedContentFile = Path.Combine(BaseFolder, "encrypted.txt");
        public static string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
        public static string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");
        public static string[] DummyFiles = new string[]
        {
            EncryptedContentFile,
            PublicKeyFile,
            PrivateKeyFile
        };

        #region Private methods
        public static void DeleteTestFiles(string[] files)
        {
            try
            {
                Thread.Sleep(500);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {
                throw;
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
}
