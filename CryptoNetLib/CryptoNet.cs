using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CryptoNetLib.helpers;

namespace CryptoNetLib
{
    public class CryptoNet : ICryptoNet
    {
        private readonly CspParameters _parameters;
        private RSACryptoServiceProvider _rsa;
        private readonly string _key;

        public CryptoNet(string key)
        {
            _key = key;
            _parameters = new CspParameters
            {
                KeyContainerName = _key
            };
            _rsa = new RSACryptoServiceProvider(_parameters);
        }

        public KeyHelper.KeyType InitAsymmetricKeys()
        {
            _rsa.PersistKeyInCsp = true;
            return _rsa.GetKeyType();
        }

        public KeyHelper.KeyType ImportKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                _rsa.FromXmlString(key);
            }

            return InitAsymmetricKeys();
        }

        public string ExportPublicKey()
        {
            return _rsa.ToXmlString(false);
        }

        public string ExportPrivateKey()
        {
            return _rsa.ToXmlString(true);
        }

        public byte[] Encrypt(string content)
        {
            if (_rsa == null)
                return StringToBytes(KeyHelper.KeyType.NotSet.ToString());
            return content == null ? StringToBytes("") : EncryptContent(content);
        }

        public string Decrypt(byte[] bytes)
        {
            if (_rsa == null)
                return KeyHelper.KeyType.NotSet.ToString();
            return bytes == null ? "" : DecryptContent(bytes);
        }

        public void Save(string filename, byte[] bytes)
        {
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public void SaveKey(string filename, string content)
        {
            var bytes = StringToBytes(content);
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public byte[] Load(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public string LoadKey(string filename)
        {
            return BytesToString(File.ReadAllBytes(filename));
        }

        private byte[] EncryptContent(string text)
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

                    using (var msIn = new MemoryStream(StringToBytes(text)))
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

        private string DecryptContent(byte[] bytes)
        {
            string text;

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

                    text = BytesToString(outMs.ToArray());

                    outMs.Close();
                }

                inMs.Close();
            }

            return text;
        }

        private static string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        private static byte[] StringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

    }

}
