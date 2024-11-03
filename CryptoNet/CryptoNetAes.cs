// <copyright file="CryptoNetAes.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using System.Security.Cryptography;
using CryptoNet.Models;
using CryptoNet.Shared;
using CryptoNet.Utils;

namespace CryptoNet;

/// <summary>
/// Provides AES cryptographic functionalities, including key management, encryption, and decryption.
/// </summary>
public class CryptoNetAes : ICryptoNetAes
{
    private Aes Aes { get; }

    /// <summary>
    /// Gets information about the current cryptographic configuration and key details.
    /// </summary>
    /// <value>A <see cref="CryptoNetInfo"/> object containing details such as encryption type, key type, and cryptographic parameters.</value>
    public CryptoNetInfo Info { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetAes"/> class and generates a new AES key and IV.
    /// </summary>
    public CryptoNetAes()
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        Aes.GenerateKey();
        Aes.GenerateIV();
        Info = CreateInfo(Aes.Key, Aes.IV);
        Aes.Key = Info.AesDetail?.AesKeyValue.Key;
        Aes.IV = Info.AesDetail?.AesKeyValue.Iv;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetAes"/> class using a specified AES key in string format.
    /// </summary>
    /// <param name="key">The AES key as a string.</param>
    public CryptoNetAes(string key)
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        var keyInfo = CryptoNetUtils.ImportAesKey(key);
        Info = CreateInfo(keyInfo.Key, keyInfo.Iv);
        Aes.Key = Info.AesDetail?.AesKeyValue.Key;
        Aes.IV = Info.AesDetail?.AesKeyValue.Iv;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetAes"/> class using an AES key loaded from a file.
    /// </summary>
    /// <param name="fileInfo">FileInfo object representing the file containing the AES key.</param>
    public CryptoNetAes(FileInfo fileInfo)
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        var keyInfo = CryptoNetUtils.ImportAesKey(CryptoNetUtils.LoadFileToString(fileInfo.FullName));
        Info = CreateInfo(keyInfo.Key, keyInfo.Iv);
        Aes.Key = Info.AesDetail?.AesKeyValue.Key;
        Aes.IV = Info.AesDetail?.AesKeyValue.Iv;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoNetAes"/> class using specified AES key and IV.
    /// </summary>
    /// <param name="key">The AES key as a byte array.</param>
    /// <param name="iv">The initialization vector (IV) as a byte array.</param>
    public CryptoNetAes(byte[] key, byte[] iv)
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        Info = CreateInfo(key, iv);
        Aes.Key = Info.AesDetail?.AesKeyValue.Key;
        Aes.IV = Info.AesDetail?.AesKeyValue.Iv;
    }

    /// <summary>
    /// Creates and returns a new <see cref="CryptoNetInfo"/> object with AES key details.
    /// </summary>
    /// <param name="key">The AES key as a byte array.</param>
    /// <param name="iv">The initialization vector (IV) as a byte array.</param>
    /// <returns>A <see cref="CryptoNetInfo"/> instance containing AES details.</returns>
    private CryptoNetInfo CreateInfo(byte[] key, byte[] iv)
    {
        return new CryptoNetInfo()
        {
            AesDetail = new AesDetail(key, iv)
            {
                Aes = Aes
            },
            EncryptionType = EncryptionType.Aes,
            KeyType = KeyType.SymmetricKey
        };
    }

    /// <summary>
    /// Retrieves the AES key as a string.
    /// </summary>
    /// <returns>The AES key as a string.</returns>
    public string GetKey()
    {
        return CryptoNetUtils.ExportAndSaveAesKey(Aes);
    }

    /// <summary>
    /// Saves the AES key to a specified file.
    /// </summary>
    /// <param name="fileInfo">FileInfo object representing the destination file.</param>
    public void SaveKey(FileInfo fileInfo)
    {
        var key = CryptoNetUtils.ExportAndSaveAesKey(Aes);
        CryptoNetUtils.SaveKey(fileInfo.FullName, key);
    }

    /// <summary>
    /// Saves the AES key to a specified file.
    /// </summary>
    /// <param name="filename">The name of the file to save the key to.</param>
    public void SaveKey(string filename)
    {
        SaveKey(new FileInfo(filename));
    }

    #region encryption logic

    /// <summary>
    /// Encrypts content from a string using AES.
    /// </summary>
    /// <param name="content">The content to encrypt.</param>
    /// <returns>The encrypted content as a byte array.</returns>
    public byte[] EncryptFromString(string content)
    {
        return EncryptContent(CryptoNetExtensions.StringToBytes(content));
    }

    /// <summary>
    /// Encrypts content from a byte array using AES.
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
        return CryptoNetExtensions.BytesToString(DecryptContent(bytes));
    }

    /// <summary>
    /// Decrypts encrypted content to a byte array.
    /// </summary>
    /// <param name="bytes">The byte array to decrypt.</param>
    /// <returns>The decrypted content as a byte array.</returns>
    public byte[] DecryptToBytes(byte[] bytes)
    {
        return DecryptContent(bytes);
    }

    /// <summary>
    /// Encrypts a byte array using AES.
    /// </summary>
    /// <param name="bytes">The byte array to encrypt.</param>
    /// <returns>The encrypted byte array.</returns>
    private byte[] EncryptContent(byte[] bytes)
    {
        if (bytes == null || bytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        byte[] result;

        var encryptor = Aes.CreateEncryptor(Aes.Key, Aes.IV);

        using (var msOut = new MemoryStream())
        {
            using (var csEncryptedOut = new CryptoStream(msOut, encryptor, CryptoStreamMode.Write))
            {
                var blockSizeBytes = Aes.BlockSize / 8;
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
    /// Decrypts a byte array using AES.
    /// </summary>
    /// <param name="bytes">The byte array to decrypt.</param>
    /// <returns>The decrypted byte array.</returns>
    private byte[] DecryptContent(byte[] bytes)
    {
        if (bytes == null || bytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        byte[] result;

        using (var inMs = new MemoryStream(bytes))
        {
            var decryptor = Aes.CreateDecryptor(Aes.Key, Aes.IV);

            using (var outMs = new MemoryStream())
            {
                var blockSizeBytes = Aes.BlockSize / 8;
                var data = new byte[blockSizeBytes];

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
