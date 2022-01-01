// <copyright file="KeyHelper.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of helpers project</summary>

using System.ComponentModel;
using System.Security.Cryptography;

namespace CryptoNetLib.helpers
{
    public static class KeyHelper
    {
        public enum KeyType
        {
            [Description("Key does not exist.")]
            NotSet,

            [Description("Public key is set.")]
            PublicKey,

            [Description("Both public and private are set.")]
            PrivateKey
        }

        public static string GetDescription(this KeyType value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static KeyType GetKeyType(this RSACryptoServiceProvider rsa)
        {
            return rsa.PublicOnly ? KeyType.PublicKey : KeyType.PrivateKey;
        }
    }
}
