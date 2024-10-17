// <copyright file="CryptoNetUtils.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CryptoNet.Models;

namespace CryptoNet.Extensions;

public static class CryptoNetExtensions
{
    #region External methods
    public static RSAParameters GetParameters(X509Certificate2? certificate, KeyType keyType)
    {
        return certificate!.GetRSAPrivateKey()!.ExportParameters(keyType == KeyType.PrivateKey);
    }

    public static X509Certificate2? GetCertificateFromStore(StoreName storeName, StoreLocation storeLocation, string certName)
    {
        X509Store store = new X509Store(storeName, storeLocation);
        return GetCertificateFromStore(store, certName);
    }

    public static X509Certificate2? GetCertificateFromStore(StoreName storeName, string certName)
    {
        X509Store store = new X509Store(storeName);
        return GetCertificateFromStore(store, certName);
    }

    public static X509Certificate2? GetCertificateFromStore(StoreLocation storeLocation, string certName)
    {
        X509Store store = new X509Store(storeLocation);
        return GetCertificateFromStore(store, certName);
    }

    public static X509Certificate2? GetCertificateFromStore(string certName)
    {
        X509Store store = new X509Store(StoreLocation.CurrentUser);
        return GetCertificateFromStore(store, certName);
    }

    private static X509Certificate2? GetCertificateFromStore(X509Store store, string certName)
    {
        try
        {
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates;
            X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);
            return signingCert.Count == 0 ? null : signingCert[0];
        }
        finally
        {
            store.Close();
        }
    }

    public static string BytesToString(byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }

    public static byte[] StringToBytes(string content)
    {
        return Encoding.ASCII.GetBytes(content);
    }

    public static string Base64BytesToString(byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }

    public static byte[] Base64StringToBytes(string content)
    {
        return Convert.FromBase64String(content);
    }

    public static bool ByteArrayCompare(byte[] b1, byte[] b2)
    {
        if (b1.Length != b2.Length)
        {
            return false;
        }

        return (b1.Length - b2.Length) == 0 && b1.SequenceEqual(b2);
    }
    #endregion
}
