// <copyright file="CryptoNetUtils.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using CryptoNet.Models;
using CryptoNet.Helpers;

namespace CryptoNet.Utils;

public static class CryptoNetUtils
{
    internal static byte[] LoadFileToBytes(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    internal static string LoadFileToString(string filename)
    {
        return CryptoNetHelpers.BytesToString(LoadFileToBytes(filename));
    }

    internal static void SaveKey(string filename, byte[] bytes)
    {
        using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        fs.Write(bytes, 0, bytes.Length);
    }

    internal static void SaveKey(string filename, string content)
    {
        var bytes = CryptoNetHelpers.StringToBytes(content);
        SaveKey(filename, bytes);
    }

    internal static string ExportAndSaveAesKey(Aes aes)
    {
        AesKeyValue aesKeyValue = new AesKeyValue { Key = aes.Key, Iv = aes.IV };
        XmlSerializer serializer = new XmlSerializer(typeof(AesKeyValue));
        StringWriter writer = new StringWriter();
        serializer.Serialize(writer, aesKeyValue);
        writer.Close();
        return writer.ToString();
    }

    internal static AesKeyValue ImportAesKey(string aes)
    {
        XmlSerializer deserializer = new XmlSerializer(typeof(AesKeyValue));
        StringReader reader = new StringReader(aes);
        return (AesKeyValue)deserializer.Deserialize(reader)!;
    }

    internal static string GetDescription(KeyType value)
    {
        var fi = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }

    internal static KeyType GetKeyType(RSACryptoServiceProvider rsa)
    {
        return rsa.PublicOnly ? KeyType.PublicKey : KeyType.PrivateKey;
    }
}
