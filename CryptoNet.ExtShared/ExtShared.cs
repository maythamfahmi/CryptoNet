// <copyright file="CryptoNetUtils.cs" company="itbackyard" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CryptoNet.Models;
using System.Text.Json;

namespace CryptoNet.ExtShared;

/// <summary>
/// Provides utility methods for cryptographic operations, certificate retrieval, 
/// and byte-array manipulation.
/// </summary>
public static class ExtShared
{
    /// <summary>
    /// Gets RSA parameters from a certificate for a specified key type.
    /// </summary>
    /// <param name="certificate">The certificate to retrieve the RSA key from.</param>
    /// <param name="keyType">The type of key to retrieve (public or private).</param>
    /// <returns>The RSA parameters of the specified key type.</returns>
    public static RSAParameters GetParameters(X509Certificate2? certificate, KeyType keyType)
    {
        return certificate!.GetRSAPrivateKey()!.ExportParameters(keyType == KeyType.PrivateKey);
    }

    /// <summary>
    /// Retrieves a certificate from the specified certificate store by name.
    /// </summary>
    /// <param name="storeName">The name of the certificate store.</param>
    /// <param name="storeLocation">The location of the certificate store.</param>
    /// <param name="certName">The name of the certificate.</param>
    /// <returns>The certificate if found; otherwise, <c>null</c>.</returns>
    public static X509Certificate2? GetCertificateFromStore(StoreName storeName, StoreLocation storeLocation, string certName)
    {
        X509Store store = new X509Store(storeName, storeLocation);
        return GetCertificateFromStore(store, certName);
    }

    /// <summary>
    /// Retrieves a certificate from a specified store name by name.
    /// </summary>
    /// <param name="storeName">The name of the certificate store.</param>
    /// <param name="certName">The name of the certificate.</param>
    /// <returns>The certificate if found; otherwise, <c>null</c>.</returns>
    public static X509Certificate2? GetCertificateFromStore(StoreName storeName, string certName)
    {
        X509Store store = new X509Store(storeName);
        return GetCertificateFromStore(store, certName);
    }

    /// <summary>
    /// Retrieves a certificate from a specified store location by name.
    /// </summary>
    /// <param name="storeLocation">The location of the certificate store.</param>
    /// <param name="certName">The name of the certificate.</param>
    /// <returns>The certificate if found; otherwise, <c>null</c>.</returns>
    public static X509Certificate2? GetCertificateFromStore(StoreLocation storeLocation, string certName)
    {
        X509Store store = new X509Store(storeLocation);
        return GetCertificateFromStore(store, certName);
    }

    /// <summary>
    /// Retrieves a certificate from the current user store by name.
    /// </summary>
    /// <param name="certName">The name of the certificate.</param>
    /// <returns>The certificate if found; otherwise, <c>null</c>.</returns>
    public static X509Certificate2? GetCertificateFromStore(string certName)
    {
        X509Store store = new X509Store(StoreLocation.CurrentUser);
        return GetCertificateFromStore(store, certName);
    }

    /// <summary>
    /// Helper method to retrieve a certificate from the specified store.
    /// </summary>
    /// <param name="store">The certificate store to search within.</param>
    /// <param name="certName">The name of the certificate.</param>
    /// <returns>The certificate if found; otherwise, <c>null</c>.</returns>
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

    /// <summary>
    /// Converts a byte array to an ASCII-encoded string.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>An ASCII-encoded string representation of the byte array.</returns>
    public static string BytesToString(byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }

    /// <summary>
    /// Converts an ASCII-encoded string to a byte array.
    /// </summary>
    /// <param name="content">The string to convert.</param>
    /// <returns>A byte array representing the ASCII-encoded string.</returns>
    public static byte[] StringToBytes(string content)
    {
        return Encoding.ASCII.GetBytes(content);
    }

    /// <summary>
    /// Encodes a byte array to a Base64 string.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <returns>A Base64 string representation of the byte array.</returns>
    public static string Base64BytesToString(byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Decodes a Base64 string to a byte array.
    /// </summary>
    /// <param name="content">The Base64 string to decode.</param>
    /// <returns>A byte array decoded from the Base64 string.</returns>
    public static byte[] Base64StringToBytes(string content)
    {
        return Convert.FromBase64String(content);
    }

    /// <summary>
    /// Compares two byte arrays for equality.
    /// </summary>
    /// <param name="b1">The first byte array to compare.</param>
    /// <param name="b2">The second byte array to compare.</param>
    /// <returns><c>true</c> if the byte arrays are equal; otherwise, <c>false</c>.</returns>
    public static bool ByteArrayCompare(byte[] b1, byte[] b2)
    {
        if (b1.Length != b2.Length)
        {
            return false;
        }

        return b1.Length - b2.Length == 0 && b1.SequenceEqual(b2);
    }

    /// <summary>
    /// Loads the content of a specified file as a byte array.
    /// </summary>
    /// <param name="filename">The path of the file to load.</param>
    /// <returns>A byte array containing the file's content.</returns>
    public static byte[] LoadFileToBytes(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    /// <summary>
    /// Loads the content of a specified file and converts it to a string.
    /// </summary>
    /// <param name="filename">The path of the file to load.</param>
    /// <returns>A string representing the file's content.</returns>
    public static string LoadFileToString(string filename)
    {
        return BytesToString(LoadFileToBytes(filename));
    }

    /// <summary>
    /// Saves a byte array to a specified file.
    /// </summary>
    /// <param name="filename">The path of the file where the data will be saved.</param>
    /// <param name="bytes">The byte array containing the data to save.</param>
    public static void SaveKey(string filename, byte[] bytes)
    {
        using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        fs.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Converts a string to a byte array and saves it to a specified file.
    /// </summary>
    /// <param name="filename">The path of the file where the string data will be saved.</param>
    /// <param name="content">The string content to convert and save.</param>
    public static void SaveKey(string filename, string content)
    {
        var bytes = StringToBytes(content);
        SaveKey(filename, bytes);
    }

    /// <summary>
    /// Exports the AES key and IV to a JSON string for storage or sharing.
    /// </summary>
    /// <param name="aes">The AES instance containing the key and IV to export.</param>
    /// <returns>A JSON string representation of the AES key and IV.</returns>
    public static string ExportAndSaveAesKey(Aes aes)
    {
        AesKeyValue aesKeyValue = new AesKeyValue(aes.Key, aes.IV);
        return JsonSerializer.Serialize(aesKeyValue);
    }

    /// <summary>
    /// Imports an AES key and IV from a JSON string.
    /// </summary>
    /// <param name="aesJson">A JSON string representing the AES key and IV.</param>
    /// <returns>An <see cref="AesKeyValue"/> object containing the imported AES key and IV.</returns>
    public static AesKeyValue ImportAesKey(string aesJson)
    {
        return JsonSerializer.Deserialize<AesKeyValue>(aesJson)!;
    }

    /// <summary>
    /// Retrieves the description attribute of a specified <see cref="KeyType"/> enumeration value.
    /// </summary>
    /// <param name="value">The <see cref="KeyType"/> value for which to retrieve the description.</param>
    /// <returns>The description of the <see cref="KeyType"/> value, or the value's name if no description attribute is found.</returns>
    public static string GetDescription(KeyType value)
    {
        var fi = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }

    /// <summary>
    /// Determines whether the specified RSA key is a public or private key.
    /// </summary>
    /// <param name="rsa">The RSA instance to evaluate.</param>
    /// <returns>The <see cref="KeyType"/> value indicating whether the RSA key is public or private.</returns>
    public static KeyType GetKeyType(RSACryptoServiceProvider rsa)
    {
        return rsa.PublicOnly ? KeyType.PublicKey : KeyType.PrivateKey;
    }
}

