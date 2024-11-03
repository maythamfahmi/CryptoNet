// <copyright file="CryptoNetUtils.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Xml.Serialization;
using CryptoNet.Models;
using CryptoNet.Extensions;

namespace CryptoNet.Utils;

/// <summary>
/// Provides utility methods for cryptographic operations, including file handling, key serialization, and type description retrieval.
/// </summary>
public static class CryptoNetUtils
{
    /// <summary>
    /// Loads the content of a specified file as a byte array.
    /// </summary>
    /// <param name="filename">The path of the file to load.</param>
    /// <returns>A byte array containing the file's content.</returns>
    internal static byte[] LoadFileToBytes(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    /// <summary>
    /// Loads the content of a specified file and converts it to a string.
    /// </summary>
    /// <param name="filename">The path of the file to load.</param>
    /// <returns>A string representing the file's content.</returns>
    internal static string LoadFileToString(string filename)
    {
        return CryptoNetExtensions.BytesToString(LoadFileToBytes(filename));
    }

    /// <summary>
    /// Saves a byte array to a specified file.
    /// </summary>
    /// <param name="filename">The path of the file where the data will be saved.</param>
    /// <param name="bytes">The byte array containing the data to save.</param>
    internal static void SaveKey(string filename, byte[] bytes)
    {
        using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        fs.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Converts a string to a byte array and saves it to a specified file.
    /// </summary>
    /// <param name="filename">The path of the file where the string data will be saved.</param>
    /// <param name="content">The string content to convert and save.</param>
    internal static void SaveKey(string filename, string content)
    {
        var bytes = CryptoNetExtensions.StringToBytes(content);
        SaveKey(filename, bytes);
    }

    /// <summary>
    /// Exports the AES key and IV to a JSON string for storage or sharing.
    /// </summary>
    /// <param name="aes">The AES instance containing the key and IV to export.</param>
    /// <returns>A JSON string representation of the AES key and IV.</returns>
    internal static string ExportAndSaveAesKey(Aes aes)
    {
        AesKeyValue aesKeyValue = new AesKeyValue { Key = aes.Key, Iv = aes.IV };
        return JsonSerializer.Serialize(aesKeyValue);
    }

    /// <summary>
    /// Imports an AES key and IV from a JSON string.
    /// </summary>
    /// <param name="aesJson">A JSON string representing the AES key and IV.</param>
    /// <returns>An <see cref="AesKeyValue"/> object containing the imported AES key and IV.</returns>
    internal static AesKeyValue ImportAesKey(string aesJson)
    {
        return JsonSerializer.Deserialize<AesKeyValue>(aesJson)!;
    }

    /// <summary>
    /// Retrieves the description attribute of a specified <see cref="KeyType"/> enumeration value.
    /// </summary>
    /// <param name="value">The <see cref="KeyType"/> value for which to retrieve the description.</param>
    /// <returns>The description of the <see cref="KeyType"/> value, or the value's name if no description attribute is found.</returns>
    internal static string GetDescription(KeyType value)
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
    internal static KeyType GetKeyType(RSACryptoServiceProvider rsa)
    {
        return rsa.PublicOnly ? KeyType.PublicKey : KeyType.PrivateKey;
    }
}

