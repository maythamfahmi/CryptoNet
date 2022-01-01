// <copyright file="CryptoNet.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNetLib project</summary>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CryptoNetLib.helpers;
using static CryptoNetLib.helpers.KeyHelper;

namespace CryptoNetLib
{
    public class CryptoNet : ICryptoNet
    {
        private readonly RSA _rsa;

        /// <summary>
        /// You can pass 2 type of keys, Private and Public key.
        /// Use Public key to encrypt with.
        /// Use Private key to decrypt with.
        /// You need to generate RSA key pair first
        /// use ExportPrivateKey method for generating Private key.
        /// use ExportPublicKey method for generating Public key.
        /// </summary>
        /// <param name="asymmetricKey"></param>
        public CryptoNet(string? asymmetricKey = null)
        {
            _rsa = RSA.Create();
            _rsa.KeySize = 2048;
            if (!string.IsNullOrEmpty(asymmetricKey))
            {
                _rsa.FromXmlString(asymmetricKey);
            }
        }

        /// <summary>
        /// Import certificate from your computer
        /// You can use helper method like:
        /// X509Certificate2 certificate = CryptoNetUtils.GetCertificateFromStore("CN=CERTIFICATE_NAME");
        /// </summary>
        /// <param name="certificate"></param>
        public CryptoNet(X509Certificate2? certificate, KeyHelper.KeyType keyType)
        {
            RSAParameters @params = CryptoNetUtils.GetParameters(certificate, keyType);
            _rsa = RSA.Create();
            _rsa.KeySize = 2048;
            _rsa.ImportParameters(@params);
        }

        /// <summary>
        /// Get Key Type that is  initialization in the constructor
        /// </summary>
        /// <returns></returns>
        public KeyType GetKeyType()
        {
            var privateKey = this.ExportPrivateKey();
            if (
                privateKey.Contains("RSAKeyValue") &&
                privateKey.Contains("Modulus") &&
                privateKey.Contains("Exponent") &&
                privateKey.Contains("<P>") &&
                privateKey.Contains("<DP>") &&
                privateKey.Contains("<DQ>") &&
                privateKey.Contains("<InverseQ>") &&
                privateKey.Contains("<D>")
            )
            {
                return KeyType.PrivateKey;
            }

            var publicKey = this.ExportPublicKey();
            if (
                publicKey.Contains("RSAKeyValue") &&
                publicKey.Contains("Modulus") &&
                publicKey.Contains("Exponent") &&
                !publicKey.Contains("<P>") &&
                !publicKey.Contains("<DP>") &&
                !publicKey.Contains("<DQ>") &&
                !publicKey.Contains("<InverseQ>") &&
                !publicKey.Contains("<D>")
            )
            {
                return KeyType.PublicKey;
            }

            return KeyType.NotSet;
        }

        /// <summary>
        /// Generate and Export Public Key
        /// </summary>
        /// <returns></returns>
        public string ExportPublicKey()
        {
            try
            {
                return _rsa.ToXmlString(false);
            }
            catch (CryptographicException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Generate and Export Private Key (RSA Pairs both Private and Public key)
        /// </summary>
        /// <returns></returns>
        public string ExportPrivateKey()
        {
            try
            {
                return _rsa.ToXmlString(true);
            }
            catch (CryptographicException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// This method is ideal to encrypt string or json payload
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public byte[] EncryptFromString(string content)
        {
            return EncryptContent(CryptoNetUtils.StringToBytes(content));
        }

        /// <summary>
        /// This method is ideal to encrypt documents and files like
        /// word, excel, image etc.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] EncryptFromBytes(byte[] bytes)
        {
            return EncryptContent(bytes);
        }

        private byte[] EncryptContent(byte[] content)
        {
            byte[] bytes;

            var rjndl = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC
            };
            var transform = rjndl.CreateEncryptor();

            var keyEncrypted = _rsa.Encrypt(rjndl.Key, RSAEncryptionPadding.OaepSHA1);

            var lKey = keyEncrypted.Length;
            var lenK = BitConverter.GetBytes(lKey);
            var lIv = rjndl.IV.Length;
            var lenIv = BitConverter.GetBytes(lIv);

            using (var msOut = new MemoryStream())
            {
                msOut.Write(lenK, 0, 4);
                msOut.Write(lenIv, 0, 4);
                msOut.Write(keyEncrypted, 0, lKey);
                msOut.Write(rjndl.IV, 0, lIv);

                using (var outStreamEncrypted = new CryptoStream(msOut, transform, CryptoStreamMode.Write))
                {
                    var blockSizeBytes = rjndl.BlockSize / 8;
                    var data = new byte[blockSizeBytes];

                    using (var msIn = new MemoryStream(content))
                    {
                        int count;
                        do
                        {
                            count = msIn.Read(data, 0, blockSizeBytes);
                            outStreamEncrypted.Write(data, 0, count);
                        } while (count > 0);

                        msIn.Close();
                    }

                    outStreamEncrypted.FlushFinalBlock();
                    outStreamEncrypted.Close();
                }

                bytes = msOut.ToArray();

                msOut.Close();
            }

            return bytes;
        }

        /// <summary>
        /// This method is ideal to decrypt string or json payload
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string DecryptToString(byte[] bytes)
        {
            return CryptoNetUtils.BytesToString(DecryptContent(bytes));
        }

        /// <summary>
        /// This method is ideal to decrypt documents and files like
        /// word, excel, image etc.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] DecryptToBytes(byte[] bytes)
        {
            return DecryptContent(bytes);
        }

        private byte[] DecryptContent(byte[] bytes)
        {
            byte[] content;

            var rjndl = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC
            };

            var lenKByte = new byte[4];
            var lenIvByte = new byte[4];

            using (var inMs = new MemoryStream(bytes))
            {
                inMs.Seek(0, SeekOrigin.Begin);
                inMs.Seek(0, SeekOrigin.Begin);
                inMs.Read(lenKByte, 0, 3);
                inMs.Seek(4, SeekOrigin.Begin);
                inMs.Read(lenIvByte, 0, 3);

                var lenK = BitConverter.ToInt32(lenKByte, 0);
                var lenIv = BitConverter.ToInt32(lenIvByte, 0);

                var startC = lenK + lenIv + 8;

                var keyEncrypted = new byte[lenK];
                var iv = new byte[lenIv];

                inMs.Seek(8, SeekOrigin.Begin);
                inMs.Read(keyEncrypted, 0, lenK);
                inMs.Seek(8 + lenK, SeekOrigin.Begin);
                inMs.Read(iv, 0, lenIv);

                var keyDecrypted = _rsa.Decrypt(keyEncrypted, RSAEncryptionPadding.OaepSHA1);

                var transform = rjndl.CreateDecryptor(keyDecrypted, iv);

                using (var outMs = new MemoryStream())
                {
                    var blockSizeBytes = rjndl.BlockSize / 8;
                    var data = new byte[blockSizeBytes];

                    inMs.Seek(startC, SeekOrigin.Begin);
                    using (var outStreamDecrypted = new CryptoStream(outMs, transform, CryptoStreamMode.Write))
                    {
                        int count;
                        do
                        {
                            count = inMs.Read(data, 0, blockSizeBytes);
                            outStreamDecrypted.Write(data, 0, count);
                        } while (count > 0);

                        outStreamDecrypted.FlushFinalBlock();
                        outStreamDecrypted.Close();
                    }

                    content = outMs.ToArray();

                    outMs.Close();
                }

                inMs.Close();
            }

            return content;
        }

    }

}
