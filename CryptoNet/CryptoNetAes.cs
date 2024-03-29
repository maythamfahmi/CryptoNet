﻿// <copyright file="CryptoNetAes.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using System.Security.Cryptography;
using CryptoNet.Models;
using CryptoNet.Utils;

namespace CryptoNet;

public class CryptoNetAes : ICryptoNet
{
    private Aes Aes { get; }
    public CryptoNetInfo Info { get; }

    public CryptoNetAes()
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        Aes.GenerateKey();
        Aes.GenerateIV();
        Info = CreateInfo(Aes.Key, Aes.IV);
        Aes.Key = Info.AesDetail!.AesKeyValue.Key;
        Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
    }

    public CryptoNetAes(string key)
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        var keyInfo = CryptoNetUtils.ImportAesKey(key);
        Info = CreateInfo(keyInfo.Key, keyInfo.Iv);
        Aes.Key = Info.AesDetail!.AesKeyValue.Key;
        Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
    }

    public CryptoNetAes(FileInfo fileInfo)
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        var keyInfo = CryptoNetUtils.ImportAesKey(CryptoNetUtils.LoadFileToString(fileInfo.FullName));
        Info = CreateInfo(keyInfo.Key, keyInfo.Iv);
        Aes.Key = Info.AesDetail!.AesKeyValue.Key;
        Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
    }

    public CryptoNetAes(byte[] key, byte[] iv)
    {
        Aes = Aes.Create();
        Aes.KeySize = 256;
        Info = CreateInfo(key, iv);
        Aes.Key = Info.AesDetail!.AesKeyValue.Key;
        Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
    }

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

    public string ExportKey(bool? privateKey = null)
    {
        return CryptoNetUtils.ExportAndSaveAesKey(Aes);
    }

    public void ExportKeyAndSave(FileInfo fileInfo, bool? privateKey = false)
    {
        var key = CryptoNetUtils.ExportAndSaveAesKey(Aes);
        CryptoNetUtils.SaveKey(fileInfo.FullName, key);
    }

    #region encryption logic
    public byte[] EncryptFromString(string content)
    {
        return EncryptContent(CryptoNetUtils.StringToBytes(content));

    }

    public byte[] EncryptFromBytes(byte[] bytes)
    {
        return EncryptContent(bytes);
    }

    public string DecryptToString(byte[] bytes)
    {
        return CryptoNetUtils.BytesToString(DecryptContent(bytes));
    }

    public byte[] DecryptToBytes(byte[] bytes)
    {
        return DecryptContent(bytes);
    }

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