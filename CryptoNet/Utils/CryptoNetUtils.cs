// <copyright file="CryptoNetUtils.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using CryptoNet.Models;

namespace CryptoNet.Utils
{
    public static class CryptoNetUtils
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
        #endregion

        #region Internal methods
        internal static byte[] LoadFileToBytes(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        internal static string LoadFileToString(string filename)
        {
            return BytesToString(LoadFileToBytes(filename));
        }

        internal static void SaveKey(string filename, byte[] bytes)
        {
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
        }

        internal static void SaveKey(string filename, string content)
        {
            var bytes = StringToBytes(content);
            SaveKey(filename, bytes);
        }

        internal static string ExportAndSaveAesKey(Aes aes)
        {
            KeyInfo keyInfo = new KeyInfo { Key = aes.Key, Iv = aes.IV };
            XmlSerializer serializer = new XmlSerializer(typeof(KeyInfo));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, keyInfo);
            writer.Close();
            return writer.ToString();
        }

        internal static KeyInfo ImportAesKey(string aes)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(KeyInfo));
            StringReader reader = new StringReader(aes);
            return (KeyInfo)deserializer.Deserialize(reader)!;
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
        #endregion
    }

}
