// Copyright and trademark notices at the end of this file.

using System.Security.Cryptography;
using System.Text;
using CryptoNet.Share.Extensions;
using CryptoNet.Models;
using CryptoNet.Utils;
using CryptoNet.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;

namespace CryptoNet.Cli;

public static class ExampleAes
{
    private const string ConfidentialDummyData = @"Some Secret Data";

    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
    private readonly static string SymmetricKeyFile = Path.Combine(BaseFolder, $"{KeyType.SymmetricKey}.xml");

#if false // This is covered in AESExample.
    public static void Example_1_Encrypt_Decrypt_Content_With_SelfGenerated_SymmetricKey()
    {
        var key = new CryptoNetAes().ExportKey();

        var encrypt = new CryptoNetAes(key).EncryptFromString(ConfidentialDummyData);
        var decrypt = new CryptoNetAes(key).DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }
#endif
    public static void Example_2_SelfGenerated_And_Save_SymmetricKey()
    {
        ICryptoNet cryptoNet = new CryptoNetAes();
        var file = new FileInfo(SymmetricKeyFile);
        cryptoNet.ExportKeyAndSave(file);

        Debug.Assert(File.Exists(file.FullName));

        var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);
        var decrypt = new CryptoNetAes(file).DecryptToString(encrypt);

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

        var encrypt = new CryptoNetAes(key, iv).EncryptFromString(ConfidentialDummyData);
        var decrypt = new CryptoNetAes(key, iv).DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_4_Encrypt_Decrypt_Content_With_Human_Readable_Key_Secret_SymmetricKey()
    {
        var symmetricKey = UniqueKeyGenerator("symmetricKey");
        var secret = new string(UniqueKeyGenerator("password").Take(16).ToArray());

        var key = Encoding.UTF8.GetBytes(symmetricKey);
        var iv = Encoding.UTF8.GetBytes(secret);

        var encrypt = new CryptoNetAes(key, iv).EncryptFromString(ConfidentialDummyData);
        var decrypt = new CryptoNetAes(key, iv).DecryptToString(encrypt);

        Debug.Assert(ConfidentialDummyData == decrypt);
    }

    public static void Example_5_Encrypt_And_Decrypt_File_With_SymmetricKey_Test(string path, string filename)
    {
        var key = new CryptoNetAes().ExportKey();

        var filePath = Path.Combine(path, filename);
        FileInfo fi = new FileInfo(filePath);

        byte[] pdfFileBytes = File.ReadAllBytes(filePath);
        var encrypt = new CryptoNetAes(key).EncryptFromBytes(pdfFileBytes);

        var decrypt = new CryptoNetAes(key).DecryptToBytes(encrypt);
        string pdfDecryptedFilePath = Path.Combine(path, $"{Path.GetFileNameWithoutExtension(fi.Name)}-decrypted{fi.Extension}");
        File.WriteAllBytes(pdfDecryptedFilePath, decrypt);

        var isIdenticalFile = CryptoNetExtensions.ByteArrayCompare(pdfFileBytes, decrypt);
        Debug.Assert(isIdenticalFile);
    }

    private static string UniqueKeyGenerator(string input)
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

// Copyright CryptoNet contributors.
//
// The MIT License is a permissive free software license.The original MIT License text is as follows:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.