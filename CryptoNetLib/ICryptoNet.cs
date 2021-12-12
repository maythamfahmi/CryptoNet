using CryptoNetLib.helpers;

namespace CryptoNetLib
{
    public interface ICryptoNet
    {
        KeyHelper.KeyType GetKeyType();
        string ExportPublicKey();
        string ExportPrivateKey();
        byte[] Encrypt(string content);
        string Decrypt(byte[] bytes);
    }
}