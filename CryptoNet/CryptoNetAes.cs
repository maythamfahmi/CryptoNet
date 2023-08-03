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
using CryptoNet.Utils;

namespace CryptoNet
{
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
            Info = CreateDetails(Aes.Key, Aes.IV);
            Aes.Key = Info.AesDetail!.AesKeyValue.Key;
            Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
        }

        public CryptoNetAes(string key)
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;
            var keyInfo = CryptoNetUtils.ImportAesKey(key);
            Info = CreateDetails(keyInfo.Key, keyInfo.Iv);
            Aes.Key = Info.AesDetail!.AesKeyValue.Key;
            Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
        }

        public CryptoNetAes(FileInfo fileInfo)
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;
            var keyInfo = CryptoNetUtils.ImportAesKey(CryptoNetUtils.LoadFileToString(fileInfo.FullName));
            Info = CreateDetails(keyInfo.Key, keyInfo.Iv);
            Aes.Key = Info.AesDetail!.AesKeyValue.Key;
            Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
        }

        public CryptoNetAes(byte[] key, byte[] iv)
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;
            Info = CreateDetails(key, iv);
            Aes.Key = Info.AesDetail!.AesKeyValue.Key;
            Aes.IV = Info.AesDetail!.AesKeyValue.Iv;
        }

        private CryptoNetInfo CreateDetails(byte[] key, byte[] iv)
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

        public void ExportKeyAndSave(FileInfo fileInfo, bool? privateKey = null)
        {
            var key = CryptoNetUtils.ExportAndSaveAesKey(Aes);
            CryptoNetUtils.SaveKey(fileInfo.FullName, key);
        }

        #region encryption logic
        public byte[] EncryptFromString(string bytes)
        {
            return EncryptContent(bytes);
        }

        public byte[] EncryptFromBytes(byte[] bytes)
        {
            return EncryptContent(bytes);
        }

        public string DecryptToString(byte[] bytes)
        {
            return DecryptContentToString(bytes);
        }

        public byte[] DecryptToBytes(byte[] bytes)
        {
            return DecryptContent(bytes);
        }

        private byte[] EncryptContent(string content)
        {
            if (content == null || content.Length <= 0)
            {
                throw new ArgumentNullException("content");
            }

            byte[] encrypted;

            ICryptoTransform encryptor = Aes.CreateEncryptor(Aes.Key, Aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(content);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        private byte[] EncryptContent(byte[] content)
        {
            if (content == null || content.Length <= 0)
            {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] encrypted;
            ICryptoTransform encryptor = Aes.CreateEncryptor(Aes.Key, Aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(content, 0, content.Length);
                }
                encrypted = msEncrypt.ToArray();
            }

            return encrypted;
        }

        private string DecryptContentToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                throw new ArgumentNullException("bytes");
            }

            string? plaintext;

            ICryptoTransform decryptor = Aes.CreateDecryptor(Aes.Key, Aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(bytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }

        private byte[] DecryptContent(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            byte[] decrypted;
            ICryptoTransform decryptor = Aes.CreateDecryptor(Aes.Key, Aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(bytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    var temp = new byte[bytes.Length];
                    var readBytes = csDecrypt.Read(temp, 0, temp.Length);
                    decrypted = new byte[readBytes];
                    Array.Copy(temp, decrypted, readBytes);
                }
            }

            return decrypted;
        }

        #endregion
    }
}