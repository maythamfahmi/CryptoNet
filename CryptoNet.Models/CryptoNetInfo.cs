// <copyright file="CryptoNetInfo.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CryptoNet.Models;

public class CryptoNetInfo
{
    public EncryptionType EncryptionType { get; set; }
    public KeyType KeyType { get; set; }
    public RsaDetail? RsaDetail { get; set; }
    public DsaDetail? DsaDetail { get; set; }
    public AesDetail? AesDetail { get; set; }
}

public class RsaDetail
{
    public RSA? Rsa { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] PrivateKey { get; set; }

    public RsaDetail(RSA rsa)
    {
        Rsa = rsa ?? throw new ArgumentNullException(nameof(rsa));
        PublicKey = Array.Empty<byte>();
        PrivateKey = Array.Empty<byte>();
    }

    public RsaDetail(byte[] publicKey, byte[] privateKey)
    {
        if (publicKey == null || publicKey.Length <= 0)
        {
            throw new ArgumentNullException(nameof(publicKey));
        }

        if (privateKey == null || privateKey.Length <= 0)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        PublicKey = publicKey;
        PrivateKey = privateKey;
    }
}

public class DsaDetail
{
    public DSA? Dsa { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] PrivateKey { get; set; }

    public DsaDetail(DSA dsa)
    {
        Dsa = dsa ?? throw new ArgumentNullException(nameof(dsa));
        PublicKey = Array.Empty<byte>();
        PrivateKey = Array.Empty<byte>();
    }

    public DsaDetail(byte[] publicKey, byte[] privateKey)
    {
        if (publicKey == null || publicKey.Length <= 0)
        {
            throw new ArgumentNullException(nameof(publicKey));
        }

        if (privateKey == null || privateKey.Length <= 0)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        PublicKey = publicKey;
        PrivateKey = privateKey;
    }
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

        AesKeyValue = new AesKeyValue(key, iv);
    }

    public Aes? Aes { get; set; }
    public AesKeyValue AesKeyValue { get; set; }
}

public class AesKeyValue
{
    public byte[] Key { get; }
    public byte[] Iv { get; }

    public AesKeyValue(byte[] key, byte[] iv)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Iv = iv ?? throw new ArgumentNullException(nameof(iv));
    }
}

public enum KeyType
{
    [Description("Key does not exist.")]
    NotSet,

    [Description("Symmetric key is set.")]
    SymmetricKey,

    [Description("Public key is set.")]
    PublicKey,

    [Description("Asymmetric key (both public and private) are set.")]
    PrivateKey
}

public enum EncryptionType
{
    [Description("Rsa encryption.")]
    Rsa,
    [Description("Aes encryption.")]
    Aes,
    [Description("Dsa encryption.")]
    Dsa
}
