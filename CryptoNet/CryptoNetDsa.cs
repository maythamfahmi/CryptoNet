using System;
using System.IO;
using System.Security.Cryptography;
using CryptoNet.Models;

namespace CryptoNet
{
    /// <summary>
    /// Provides functionality for performing cryptographic operations using the Digital Signature Algorithm (DSA).
    /// </summary>
    public class CryptoNetDsa : ICryptoNetDsa
    {
        /// <summary>
        /// Gets the DSA cryptographic service provider.
        /// </summary>
        private DSA Dsa { get; }

        /// <summary>
        /// Gets information about the current cryptographic configuration and key details.
        /// </summary>
        /// <value>A <see cref="CryptoNetInfo"/> object containing details such as encryption type, key type, and cryptographic parameters.</value>
        public CryptoNetInfo Info { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoNetDsa"/> class with a specified key size.
        /// </summary>
        /// <param name="keySize">The size of the DSA key in bits. Default is 2048.</param>
        public CryptoNetDsa(int keySize = 1024)
        {
            Dsa = DSA.Create();
            Info = CreateInfo(Dsa, keySize);
            Info.KeyType = CheckKeyType();
            if (Info.DsaDetail != null)
            {
                Info.DsaDetail.PrivateKey = TryGetKey();
                Info.DsaDetail.PublicKey = TryGetKey(false);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoNetDsa"/> class using a specified key in string format.
        /// </summary>
        /// <param name="key">The DSA key as a string.</param>
        /// <param name="keySize">The size of the DSA key in bits. Default is 2048.</param>
        public CryptoNetDsa(string key, int keySize = 1024)
        {
            Dsa = DSA.Create();
            Info = CreateInfo(Dsa, keySize);
            CreateAsymmetricKey(key);
            Info.KeyType = CheckKeyType();
            if (Info.DsaDetail != null)
            {
                Info.DsaDetail.PrivateKey = TryGetKey();
                Info.DsaDetail.PublicKey = TryGetKey(false);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoNetDsa"/> class using a key loaded from a file.
        /// </summary>
        /// <param name="fileInfo">A <see cref="FileInfo"/> object representing the file containing the DSA key.</param>
        /// <param name="keySize">The size of the DSA key in bits. Default is 2048.</param>
        public CryptoNetDsa(FileInfo fileInfo, int keySize = 1024)
        {
            Dsa = DSA.Create();
            Info = CreateInfo(Dsa, keySize);
            CreateAsymmetricKey(ExtShared.ExtShared.LoadFileToString(fileInfo.FullName));
            Info.KeyType = CheckKeyType();
            if (Info.DsaDetail != null)
            {
                Info.DsaDetail.PrivateKey = TryGetKey();
                Info.DsaDetail.PublicKey = TryGetKey(false);
            }
        }

        /// <summary>
        /// Retrieves the DSA key as a byte array.
        /// </summary>
        /// <param name="privateKey">True to retrieve the private key; false to retrieve the public key.</param>
        /// <returns>A byte array representing the DSA key.</returns>
        private byte[] TryGetKey(bool privateKey = true)
        {
            try
            {
                return privateKey
                    ? ExtShared.ExtShared.StringToBytes(ExportKey(KeyType.PrivateKey))
                    : ExtShared.ExtShared.StringToBytes(ExportKey(KeyType.PublicKey));
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Checks and returns the key type (public or private).
        /// </summary>
        /// <returns>The key type as a <see cref="KeyType"/> enum value.</returns>
        private KeyType CheckKeyType()
        {
            try
            {
                Dsa.ExportParameters(true);
                return KeyType.PrivateKey;
            }
            catch (CryptographicException)
            {
                return KeyType.PublicKey;
            }
            catch (Exception)
            {
                return KeyType.NotSet;
            }
        }

        /// <summary>
        /// Imports an asymmetric key from a string.
        /// </summary>
        /// <param name="key">The key as a string.</param>
        private void CreateAsymmetricKey(string? key = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Dsa.FromXmlString(key);
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="CryptoNetInfo"/> object.
        /// </summary>
        /// <param name="DSA">The DSA cryptographic service provider.</param>
        /// <param name="keySize">The size of the DSA key in bits.</param>
        /// <returns>A <see cref="CryptoNetInfo"/> instance with DSA details.</returns>
        private static CryptoNetInfo CreateInfo(DSA DSA, int keySize)
        {
            DSA.KeySize = keySize;

            return new CryptoNetInfo()
            {
                DsaDetail = new DsaDetail(DSA),
                EncryptionType = EncryptionType.Dsa,
                KeyType = KeyType.NotSet
            };
        }

        /// <summary>
        /// Retrieves the DSA key as a string.
        /// </summary>
        /// <param name="privateKey">True to retrieve the private key; false to retrieve the public key.</param>
        /// <returns>The DSA key as a string.</returns>
        public string GetKey(bool privateKey = false)
        {
            return privateKey ? ExportKey(KeyType.PrivateKey) : ExportKey(KeyType.PublicKey);
        }

        /// <summary>
        /// Saves the DSA key to a specified file.
        /// </summary>
        /// <param name="fileInfo">A <see cref="FileInfo"/> object representing the destination file.</param>
        /// <param name="privateKey">True to save the private key; false to save the public key.</param>
        public void SaveKey(FileInfo fileInfo, bool privateKey = false)
        {
            string key = privateKey ? ExportKey(KeyType.PrivateKey) : ExportKey(KeyType.PublicKey);
            if (!string.IsNullOrEmpty(key))
            {
                ExtShared.ExtShared.SaveKey(fileInfo.FullName, key);
            }
        }

        /// <summary>
        /// Saves the DSA key to a specified file.
        /// </summary>
        /// <param name="filename">The name of the file to save the key to.</param>
        /// <param name="privateKey">True to save the private key; false to save the public key.</param>
        public void SaveKey(string filename, bool privateKey = false)
        {
            SaveKey(new FileInfo(filename), privateKey);
        }

        /// <summary>
        /// Exports the DSA key as a string based on the specified key type.
        /// </summary>
        /// <param name="keyType">The type of key to export.</param>
        /// <returns>The DSA key as a string.</returns>
        private string ExportKey(KeyType keyType)
        {
            return keyType switch
            {
                KeyType.NotSet => string.Empty,
                KeyType.PublicKey => Dsa.ToXmlString(false),
                KeyType.PrivateKey => Dsa.ToXmlString(true),
                _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
            };
        }

        #region Signature and Verification Logic

        /// <summary>
        /// Creates a digital signature for a given byte array.
        /// </summary>
        /// <param name="messageBytes">The byte array to sign.</param>
        /// <returns>The generated digital signature as a byte array.</returns>
        public byte[] CreateSignature(byte[] messageBytes)
        {
            if (messageBytes == null || messageBytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(messageBytes));
            }

            return Dsa.CreateSignature(messageBytes);
        }

        /// <summary>
        /// Creates a digital signature for a given string.
        /// </summary>
        /// <param name="content">The string content to sign.</param>
        /// <returns>The generated digital signature as a byte array.</returns>
        public byte[] CreateSignature(string content)
        {
            return CreateSignature(ExtShared.ExtShared.StringToBytes(content));
        }

        /// <summary>
        /// Verifies a digital signature for a given byte array.
        /// </summary>
        /// <param name="messageBytes">The byte array containing the original message.</param>
        /// <param name="signature">The digital signature to verify.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool IsContentVerified(byte[] messageBytes, byte[] signature)
        {
            if (messageBytes == null || messageBytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(messageBytes));
            }

            return Dsa.VerifySignature(messageBytes, signature);
        }

        /// <summary>
        /// Verifies a digital signature for a given string.
        /// </summary>
        /// <param name="message">The string containing the original message.</param>
        /// <param name="signature">The digital signature to verify.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        public bool IsContentVerified(string message, byte[] signature)
        {
            return IsContentVerified(ExtShared.ExtShared.StringToBytes(message), signature);
        }
        #endregion
    }

}
