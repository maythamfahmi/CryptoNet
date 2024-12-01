using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CryptoNet.ExtPack
{
    /// <summary>
    /// Provides various cryptographic extension methods.
    /// </summary>
    public static class ExtensionPack
    {
        /// <summary>
        /// Compares two content strings by calculating their MD5 hashes and checking if they match.
        /// </summary>
        /// <param name="originalContent">The original content to compare.</param>
        /// <param name="decryptedContent">The decrypted content to compare with the original.</param>
        /// <returns>True if the MD5 hashes of both contents match; otherwise, false.</returns>
        public static bool CheckContent(string originalContent, string decryptedContent)
        {
            if (originalContent == null || decryptedContent == null)
            {
                return originalContent == decryptedContent;
            }

            return CalculateMd5(originalContent).Equals(CalculateMd5(decryptedContent));
        }

        /// <summary>
        /// Calculates the MD5 hash of the provided content string.
        /// </summary>
        /// <param name="content">The content to hash.</param>
        /// <returns>The MD5 hash of the content as a lowercase hexadecimal string.</returns>
        public static string CalculateMd5(string content)
        {
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(content));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Generates a unique key based on the MD5 hash of the provided input string.
        /// </summary>
        /// <param name="input">The input string to generate a unique key from.</param>
        /// <returns>The unique key as an uppercase hexadecimal string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input is null or empty.</exception>
        public static string UniqueKeyGenerator(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null or empty");
            }

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes);

            var stringBuilder = new StringBuilder();
            foreach (var byteValue in hashBytes)
            {
                stringBuilder.Append(byteValue.ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Exports the private or public key of an X.509 certificate in PEM format.
        /// </summary>
        /// <param name="cert">The certificate containing the key to export.</param>
        /// <param name="privateKey">If true, exports the private key; otherwise, exports the public key.</param>
        /// <returns>The PEM-encoded key as a character array.</returns>
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

        /// <summary>
        /// Exports the private key of an X.509 certificate in an encrypted PEM format using the specified password.
        /// </summary>
        /// <param name="cert">The certificate containing the private key to export.</param>
        /// <param name="password">The password to encrypt the private key.</param>
        /// <returns>The encrypted private key as a byte array.</returns>
        public static byte[] ExportPemKeyWithPassword(X509Certificate2 cert, string password)
        {
            AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;
            byte[] pass = Encoding.UTF8.GetBytes(password);
            return rsa.ExportEncryptedPkcs8PrivateKey(pass,
                new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, iterationCount: 100_000));
        }

        //todo: implement hashing helper
    }
}
