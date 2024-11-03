using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CryptoNet.ExtPack;

public static class ExtensionPack
{
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
