// <copyright file="CryptoNetInfo.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace CryptoNet.Models
{
    public class CryptoNetInfo
    {
        public EncryptionType EncryptionType { get; set; }
        public KeyType KeyType { get; set; }
        public RsaDetail? RsaDetail { get; set; }
        public AesDetail? AesDetail { get; set; }
    }

    public class RsaDetail
    {
        public RSA? Rsa { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
    }

    public class AesDetail
    {
        public AesDetail(byte[] key, byte[] iv)
        {
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            KeyInfo = new KeyInfo()
            {
                Key = key,
                Iv = iv
            };
        }

        public Aes? Aes { get; set; }
        public KeyInfo KeyInfo { get; set; }
    }

    [Serializable()]
    public class KeyInfo
    {
        [System.Xml.Serialization.XmlElement("key")]
        public byte[] Key { get; set; }

        [System.Xml.Serialization.XmlElement("iv")]
        public byte[] Iv { get; set; }

    }

    public enum KeyType
    {
        [Description("Key does not exist.")]
        NotSet,

        [Description("Symmetric key is set.")]
        SymmetricKey,

        [Description("Public key is set.")]
        PublicKey,

        [Description("Asymmertric key (both public and private) are set.")]
        PrivateKey
    }

    public enum EncryptionType
    {
        [Description("Rsa encryption.")]
        Rsa,

        [Description("Aes encryption.")]
        Aes
    }
}
