// <copyright file="ExampleAes.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CryptoNet.Models;
using CryptoNet.Utils;

namespace CryptoNet.Cli;

public class ExampleAes
{
    private const string ConfidentialDummyData = @"Some Secret Data";

    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
    private readonly static string SymmetricKeyFile = Path.Combine(BaseFolder, $"{KeyType.SymmetricKey}.xml");

    public static void Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_SymmetricKey()
    {
        ICryptoNet cryptoNet = new CryptoNetAes();
        var key = cryptoNet.ExportKey();

        ICryptoNet encryptClient = new CryptoNetAes(key);
        var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetAes(key);
        var decrypt = decryptClient.DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_2_SelfGenerated_And_Save_SymmetricKey()
    {
        ICryptoNet cryptoNet = new CryptoNetAes();
        var file = new FileInfo(SymmetricKeyFile);
        cryptoNet.ExportKeyAndSave(file);

        Debug.Assert(File.Exists(file.FullName));

        var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);

        ICryptoNet cryptoNetKeyImport = new CryptoNetAes(file);
        var decrypt = cryptoNetKeyImport.DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_3_Encrypt_Decrypt_Content_With_Own_SymmetricKey()
    {
        var symmetricKey = "12345678901234567890123456789012";
        if (symmetricKey.Length != 32)
        {
            Console.WriteLine("key should be 32 character long");
            Environment.Exit(0);
        }

        var secret = "1234567890123456";
        if (secret.Length != 16)
        {
            Console.WriteLine("key should be 16 character long");
            Environment.Exit(1);
        }

        var key = Encoding.UTF8.GetBytes(symmetricKey);
        var iv = Encoding.UTF8.GetBytes(secret);

        ICryptoNet encryptClient = new CryptoNetAes(key, iv);
        var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetAes(key, iv);
        var decrypt = decryptClient.DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_4_Encrypt_Decrypt_Content_With_Human_Readable_Key_Secret_SymmetricKey()
    {
        var symmetricKey = UniqueKeyGenerator("symmetricKey");
        var secret = new string(UniqueKeyGenerator("password").Take(16).ToArray());

        var key = Encoding.UTF8.GetBytes(symmetricKey);
        var iv = Encoding.UTF8.GetBytes(secret);

        ICryptoNet encryptClient = new CryptoNetAes(key, iv);
        var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

        ICryptoNet decryptClient = new CryptoNetAes(key, iv);
        var decrypt = decryptClient.DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_5_Encrypt_And_Decrypt_PdfFile_With_SymmetricKey_Test(string filename)
    {
        ICryptoNet cryptoNet = new CryptoNetAes();
        var key = cryptoNet.ExportKey();

        FileInfo fi = new FileInfo(filename);

        ICryptoNet encryptClient = new CryptoNetAes(key);
        string pdfFilePath = Path.Combine(BaseFolder, filename);
        byte[] pdfFileBytes = File.ReadAllBytes(pdfFilePath);
        var encrypt = encryptClient.EncryptFromBytes(pdfFileBytes);

        ICryptoNet decryptClient = new CryptoNetAes(key);
        var decrypt = decryptClient.DecryptToBytes(encrypt);
        string pdfDecryptedFilePath = $"TestFiles\\{Path.GetFileNameWithoutExtension(fi.Name)}-decrypted.{fi.Extension}";
        File.WriteAllBytes(pdfDecryptedFilePath, decrypt);

        var isIdenticalFile = CryptoNetUtils.ByteArrayCompare(pdfFileBytes, decrypt);
        Debug.Assert(isIdenticalFile);
    }

    public static string UniqueKeyGenerator(string input)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hash = MD5.HashData(inputBytes);

        StringBuilder sb = new StringBuilder();
        foreach (var t in hash)
        {
            sb.Append(t.ToString("X2"));
        }
        return sb.ToString();
    }
}