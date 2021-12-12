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

            [Description("Asymmetric key is set.")]
            AsymmetricKey,

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