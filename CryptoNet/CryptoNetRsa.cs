// <copyright file="CryptoNetRsa.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CryptoNet.Shared;
using CryptoNet.Utils;
using CryptoNet.Models;

namespace CryptoNet;

/// <summary>
/// Provides RSA cryptographic functionalities, including key management, encryption, and decryption.
/// </summary>
public class CryptoNetRsa : ICryptoNetRsa
{
    private RSA Rsa { get; }

    /// <summary>
    /// Gets information about the current cryptographic configuration and key details.
    /// </summary>
    /// <value>A <see cref="CryptoNetInfo"/> object containing details such as encryption type, key type, and cryptographic parameters.</value>
    public CryptoNetInfo Info { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetRsa"/> class with a specified key size.
    /// </summary>
    /// <param name="keySize">The size of the RSA key in bits. Default is 2048.</param>
    public CryptoNetRsa(int keySize = 2048)
    {
        Rsa = RSA.Create();
        Info = CreateInfo(Rsa, keySize);
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetRsa"/> class using a specified key in string format.
    /// </summary>
    /// <param name="key">The RSA key as a string.</param>
    /// <param name="keySize">The size of the RSA key in bits. Default is 2048.</param>
    public CryptoNetRsa(string key, int keySize = 2048)
    {
        Rsa = RSA.Create();
        Info = CreateInfo(Rsa, keySize);
        CreateAsymmetricKey(key);
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetRsa"/> class using a key loaded from a file.
    /// </summary>
    /// <param name="fileInfo">FileInfo object representing the file containing the RSA key.</param>
    /// <param name="keySize">The size of the RSA key in bits. Default is 2048.</param>
    public CryptoNetRsa(FileInfo fileInfo, int keySize = 2048)
    {
        Rsa = RSA.Create();
        Info = CreateInfo(Rsa, keySize);
        CreateAsymmetricKey(CryptoNetUtils.LoadFileToString(fileInfo.FullName));
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetRsa"/> class using an X509 certificate and key type.
    /// </summary>
    /// <param name="certificate">X509Certificate2 object representing the certificate.</param>
    /// <param name="keyType">The key type (public or private) associated with the certificate.</param>
    /// <param name="keySize">The size of the RSA key in bits. Default is 2048.</param>
    public CryptoNetRsa(X509Certificate2? certificate, KeyType keyType, int keySize = 2048)
    {
        Rsa = RSA.Create();
        Rsa.KeySize = keySize;
        Info = CreateInfo(Rsa, keySize);
        RSAParameters @params = Shared.ExtShared.GetParameters(certificate, keyType);
        Rsa.ImportParameters(@params);
        Info.KeyType = CheckKeyType();
        if (Info.RsaDetail != null)
        {
            Info.RsaDetail.PrivateKey = TryGetKey();
            Info.RsaDetail.PublicKey = TryGetKey(false);
        }
    }

    /// <summary>
    /// Retrieves the RSA key as a byte array.
    /// </summary>
    /// <param name="privateKey">True to retrieve the private key; false to retrieve the public key.</param>
    /// <returns>Byte array representing the RSA key.</returns>
    private byte[] TryGetKey(bool privateKey = true)
    {
        try
        {
            return privateKey
                ? Shared.ExtShared.StringToBytes(ExportKey(KeyType.PrivateKey))
                : Shared.ExtShared.StringToBytes(ExportKey(KeyType.PublicKey));
        }
        catch (Exception)
        {
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// Checks and returns the key type (public or private).
    /// </summary>
    /// <returns>The key type as a <see cref="KeyType"/> enum value.</returns>
    private KeyType CheckKeyType()
    {
        try
        {
            Rsa.ExportParameters(true);
            return KeyType.PrivateKey;
        }
        catch (CryptographicException)
        {
            return KeyType.PublicKey;
        }
        catch (Exception)
        {
            return KeyType.NotSet;
        }
    }

    /// <summary>
    /// Imports an asymmetric key from a string.
    /// </summary>
    /// <param name="key">The key as a string.</param>
    private void CreateAsymmetricKey(string? key = null)
    {
        if (!string.IsNullOrEmpty(key))
        {
            Rsa.FromXmlString(key);
        }
    }

    /// <summary>
    /// Creates and returns a new <see cref="CryptoNetInfo"/> object.
    /// </summary>
    /// <returns>A <see cref="CryptoNetInfo"/> instance with RSA details.</returns>
    private static CryptoNetInfo CreateInfo(RSA Rsa, int keySize)
    {
        Rsa.KeySize = keySize;

        return new CryptoNetInfo()
        {
            RsaDetail = new RsaDetail(Rsa),
            EncryptionType = EncryptionType.Rsa,
            KeyType = KeyType.NotSet
        };
    }

    /// <summary>
    /// Retrieves the RSA key as a string.
    /// </summary>
    /// <param name="privateKey">True to retrieve the private key; false to retrieve the public key.</param>
    /// <returns>The RSA key as a string.</returns>
    public string GetKey(bool privateKey = false)
    {
        return privateKey ? ExportKey(KeyType.PrivateKey) : ExportKey(KeyType.PublicKey);
    }

    /// <summary>
    /// Saves the RSA key to a specified file.
    /// </summary>
    /// <param name="fileInfo">FileInfo object representing the destination file.</param>
    /// <param name="privateKey">True to save the private key; false to save the public key.</param>
    public void SaveKey(FileInfo fileInfo, bool privateKey = false)
    {
        string key = privateKey ? ExportKey(KeyType.PrivateKey) : ExportKey(KeyType.PublicKey);
        if (!string.IsNullOrEmpty(key))
        {
            CryptoNetUtils.SaveKey(fileInfo.FullName, key);
        }
    }

    /// <summary>
    /// Saves the RSA key to a specified file.
    /// </summary>
    /// <param name="filename">The name of the file to save the key to.</param>
    /// <param name="privateKey">True to save the private key; false to save the public key.</param>
    public void SaveKey(string filename, bool privateKey = false)
    {
        SaveKey(new FileInfo(filename), privateKey);
    }

    /// <summary>
    /// Exports the RSA key as a string based on the specified key type.
    /// </summary>
    /// <param name="keyType">The type of key to export.</param>
    /// <returns>The RSA key as a string.</returns>
    private string ExportKey(KeyType keyType)
    {
        return keyType switch
        {
            KeyType.NotSet => string.Empty,
            KeyType.PublicKey => Rsa.ToXmlString(false),
            KeyType.PrivateKey => Rsa.ToXmlString(true),
            _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
        };
    }

    #region encryption logic
    /// <summary>
    /// Encrypts content from a string using RSA.
    /// </summary>
    /// <param name="content">The content to encrypt.</param>
    /// <returns>The encrypted content as a byte array.</returns>
    public byte[] EncryptFromString(string content)
    {
        return EncryptContent(Shared.ExtShared.StringToBytes(content));
    }

    /// <summary>
    /// Encrypts content from a byte array using RSA.
    /// </summary>
    /// <param name="bytes">The byte array to encrypt.</param>
    /// <returns>The encrypted byte array.</returns>
    public byte[] EncryptFromBytes(byte[] bytes)
    {
        return EncryptContent(bytes);
    }

    /// <summary>
    /// Decrypts encrypted content to a string.
    /// </summary>
    /// <param name="bytes">The byte array to decrypt.</param>
    /// <returns>The decrypted content as a string.</returns>
    public string DecryptToString(byte[] bytes)
    {
        return Shared.ExtShared.BytesToString(DecryptContent(bytes));
    }

    /// <summary>
    /// Decrypts a byte array to another byte array using AES.
    /// </summary>
    /// <param name="bytes">The encrypted byte array to decrypt.</param>
    /// <returns>The decrypted byte array.</returns>
    public byte[] DecryptToBytes(byte[] bytes)
    {
        return DecryptContent(bytes);
    }

    /// <summary>
    /// Encrypts a byte array using AES encryption.
    /// </summary>
    /// <param name="bytes">The byte array to encrypt.</param>
    /// <returns>The encrypted byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input byte array is null or empty.</exception>
    private byte[] EncryptContent(byte[] bytes)
    {
        if (bytes == null || bytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        byte[] result;

        var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;

        var encryptor = aes.CreateEncryptor();

        var keyEncrypted = Rsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA1);

        var lKey = keyEncrypted.Length;
        var lenK = BitConverter.GetBytes(lKey);
        var lIv = aes.IV.Length;
        var lenIv = BitConverter.GetBytes(lIv);

        using (var msOut = new MemoryStream())
        {
            msOut.Write(lenK, 0, 4);
            msOut.Write(lenIv, 0, 4);
            msOut.Write(keyEncrypted, 0, lKey);
            msOut.Write(aes.IV, 0, lIv);

            using (var csEncryptedOut = new CryptoStream(msOut, encryptor, CryptoStreamMode.Write))
            {
                var blockSizeBytes = aes.BlockSize / 8;
                var data = new byte[blockSizeBytes];

                using (var msIn = new MemoryStream(bytes))
                {
                    int count;
                    do
                    {
                        count = msIn.Read(data, 0, blockSizeBytes);
                        csEncryptedOut.Write(data, 0, count);
                    } while (count > 0);

                    msIn.Close();
                }

                csEncryptedOut.FlushFinalBlock();
                csEncryptedOut.Close();
            }

            result = msOut.ToArray();

            msOut.Close();
        }

        return result;
    }

    /// <summary>
    /// Decrypts a byte array that was encrypted using AES.
    /// </summary>
    /// <param name="bytes">The encrypted byte array to decrypt.</param>
    /// <returns>The decrypted byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input byte array is null or empty.</exception>
    private byte[] DecryptContent(byte[] bytes)
    {
        if (bytes == null || bytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        byte[] result;

        var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;

        var lenKByte = new byte[4];
        var lenIvByte = new byte[4];

        using (var inMs = new MemoryStream(bytes))
        {
            inMs.Seek(0, SeekOrigin.Begin);
            inMs.Seek(0, SeekOrigin.Begin);
            inMs.Read(lenKByte, 0, 3);
            inMs.Seek(4, SeekOrigin.Begin);
            inMs.Read(lenIvByte, 0, 3);

            var lenK = BitConverter.ToInt32(lenKByte, 0);
            var lenIv = BitConverter.ToInt32(lenIvByte, 0);

            var startC = lenK + lenIv + 8;

            var keyEncrypted = new byte[lenK];
            var iv = new byte[lenIv];

            inMs.Seek(8, SeekOrigin.Begin);
            inMs.Read(keyEncrypted, 0, lenK);
            inMs.Seek(8 + lenK, SeekOrigin.Begin);
            inMs.Read(iv, 0, lenIv);

            var keyDecrypted = Rsa.Decrypt(keyEncrypted, RSAEncryptionPadding.OaepSHA1);

            var decryptor = aes.CreateDecryptor(keyDecrypted, iv);

            using (var outMs = new MemoryStream())
            {
                var blockSizeBytes = aes.BlockSize / 8;
                var data = new byte[blockSizeBytes];

                inMs.Seek(startC, SeekOrigin.Begin);
                using (var csDecryptedOut = new CryptoStream(outMs, decryptor, CryptoStreamMode.Write))
                {
                    int count;
                    do
                    {
                        count = inMs.Read(data, 0, blockSizeBytes);
                        csDecryptedOut.Write(data, 0, count);
                    } while (count > 0);

                    csDecryptedOut.FlushFinalBlock();
                    csDecryptedOut.Close();
                }

                result = outMs.ToArray();

                outMs.Close();
            }

            inMs.Close();
        }

        return result;
    }
    #endregion
}
