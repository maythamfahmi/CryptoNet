// <copyright file="ICryptoNet.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNetLib project</summary>

using System.Security.Cryptography;
using CryptoNetLib.helpers;

namespace CryptoNetLib
{
    public interface ICryptoNet
    {
        KeyHelper.KeyType GetKeyType();
        string ExportPublicKey();
        string ExportPrivateKey();
        byte[] EncryptFromString(string content);
        string DecryptToString(byte[] bytes);
        byte[] EncryptFromBytes(byte[] bytes);
        byte[] DecryptToBytes(byte[] bytes);
        public RSA Rsa { get; }
    }
}
