using System;
using System.IO;
using System.Security.Cryptography;
using CryptoNetLib.helpers;
using static CryptoNetLib.helpers.KeyHelper;

namespace CryptoNetLib
{
    public class CryptoNet : ICryptoNet
    {
        private readonly RSACryptoServiceProvider _rsa;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asymmetricKeyOrRsaCertificate"></param>
        /// <param name="certificate"></param>
        public CryptoNet(string? asymmetricKeyOrRsaCertificate = null, bool certificate = false)
        {

            if (string.IsNullOrWhiteSpace(asymmetricKeyOrRsaCertificate))
            {
                throw new Exception("Missing Asymmetric Key Or RsaCertificate");
            }

            var parameters = new CspParameters();
            if (!certificate)
            {
                parameters.KeyContainerName = asymmetricKeyOrRsaCertificate;
            }

            _rsa = new RSACryptoServiceProvider(parameters)
            {
                PersistKeyInCsp = true
            };
            if (certificate)
            {
                _rsa.FromXmlString(asymmetricKeyOrRsaCertificate);
            }
        }

        public KeyType GetKeyType()
        {
            return _rsa.GetKeyType();
        }

        public string ExportPublicKey()
        {
            return _rsa.ToXmlString(false);
        }

        public string ExportPrivateKey()
        {
            return _rsa.ToXmlString(true);
        }

        public byte[] EncryptString(string content)
        {
            return EncryptContent(CryptoNetUtils.StringToBytes(content));
        }

        public byte[] EncryptBytes(byte[] bytes)
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

            var keyEncrypted = _rsa.Encrypt(rjndl.Key, false);

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


        public string DecryptString(byte[] bytes)
        {
            try
            {
                return CryptoNetUtils.BytesToString(DecryptContent(bytes));
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public byte[] DecryptBytes(byte[] bytes)
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

                var keyDecrypted = _rsa.Decrypt(keyEncrypted, false);

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
