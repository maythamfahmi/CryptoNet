// <copyright file="ExampleRsa.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CryptoNet.Models;
using CryptoNet.Utils;
using Debug = CryptoNet.Share.Extensions.Debug;

namespace CryptoNet.Cli;

public static class ExampleRsa
{
    private const string ConfidentialDummyData = @"Some Secret Data";
    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;

    internal static readonly string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
    internal static readonly string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

    public static void Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_AsymmetricKey()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();

        var privateKey = cryptoNet.ExportKey(true);
        var publicKey = cryptoNet.ExportKey(false);

        var encrypt = new CryptoNetRsa(publicKey).EncryptFromString(ConfidentialDummyData);

        var decrypt = new CryptoNetRsa(privateKey).DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_2_SelfGenerated_And_Save_AsymmetricKey()
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();

        cryptoNet.ExportKeyAndSave(new FileInfo(PrivateKeyFile), true);
        cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile), false);

        Debug.Assert(File.Exists(new FileInfo(PrivateKeyFile).FullName));
        Debug.Assert(File.Exists(new FileInfo(PublicKeyFile).FullName));

        var encrypt = new CryptoNetRsa(new FileInfo(PublicKeyFile)).EncryptFromString(ConfidentialDummyData);

        var decrypt = new CryptoNetRsa(new FileInfo(PrivateKeyFile)).DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_3_Encrypt_With_PublicKey_Decrypt_With_PrivateKey_Of_Content()
    {
        var encryptWithPublicKey = new CryptoNetRsa(new FileInfo(PublicKeyFile)).EncryptFromString(ConfidentialDummyData);

        var decryptWithPrivateKey = new CryptoNetRsa(new FileInfo(PrivateKeyFile)).DecryptToString(encryptWithPublicKey);

        Debug.Assert(ConfidentialDummyData == decryptWithPrivateKey);
    }

    public static void Example_4_Using_X509_Certificate()
    {
        // Find and replace CN=Maytham with your own certificate
        X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        var encryptWithPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey).EncryptFromString(ConfidentialDummyData);

        var decryptWithPrivateKey = new CryptoNetRsa(certificate, KeyType.PrivateKey).DecryptToString(encryptWithPublicKey);

        Debug.Assert(ConfidentialDummyData == decryptWithPrivateKey);
    }

    public static void Example_5_Export_Public_Key_For_X509_Certificate()
    {
        // Find and replace CN=Maytham with your own certificate
        X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        var publicKey = new CryptoNetRsa(certificate, KeyType.PublicKey).ExportKey(false);

        Debug.Assert(!string.IsNullOrEmpty(publicKey));
    }

    /// <summary>
    /// CryptoNet interact with .net 5/6 for customization, like import/export PEM
    /// Work in Progress, not finished
    /// </summary>
    public static void Example_7_Customize()
    {
        X509Certificate2? cert = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

        var pubKeyPem = ExportPemKey(cert!, false);
        var priKeyPem = ExportPemKey(cert!);

        var password = "password";
        var encryptedPriKeyBytes = ExportPemKeyWithPassword(cert!, password);

        ICryptoNet cryptoNet1 = ImportPemKeyWithPassword(encryptedPriKeyBytes, password);
        var encrypt1 = cryptoNet1.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNet2 = ImportPemKey(pubKeyPem);
        var encrypt2 = cryptoNet2.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNet3 = ImportPemKey(priKeyPem);
        var decrypt2 = cryptoNet3.DecryptToString(encrypt2);

        Debug.Assert(ConfidentialDummyData == decrypt2);

        var decrypt1 = cryptoNet3.DecryptToString(encrypt1);

        Debug.Assert(ConfidentialDummyData == decrypt1);
    }

    public static char[] ExportPemCertificate(X509Certificate2 cert)
    {
        byte[] certBytes = cert!.RawData;
        char[] certPem = PemEncoding.Write("CERTIFICATE", certBytes);
        return certPem;
    }

    private static char[] ExportPemKey(X509Certificate2 cert, bool privateKey = true)
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

    private static ICryptoNet ImportPemKey(char[] key)
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail!.Rsa?.ImportFromPem(key);
        return cryptoNet;
    }

    private static byte[] ExportPemKeyWithPassword(X509Certificate2 cert, string password)
    {
        AsymmetricAlgorithm rsa = cert.GetRSAPrivateKey()!;
        byte[] pass = Encoding.UTF8.GetBytes(password);
        byte[] encryptedPrivateKey = rsa.ExportEncryptedPkcs8PrivateKey(pass,
            new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, iterationCount: 100_000));
        return encryptedPrivateKey;
    }

    private static ICryptoNet ImportPemKeyWithPassword(byte[] encryptedPrivateKey, string password)
    {
        ICryptoNet cryptoNet = new CryptoNetRsa();
        cryptoNet.Info.RsaDetail?.Rsa?.ImportEncryptedPkcs8PrivateKey(password, encryptedPrivateKey, out _);
        return cryptoNet;
    }
}