using CryptoNetLib.helpers;

namespace CryptoNetLib
{
    public interface ICryptoNet
    {
        KeyHelper.KeyType GetKeyType();
        string ExportPublicKey();
        string ExportPrivateKey();
        byte[] EncryptString(string content);
        string DecryptString(byte[] bytes);
        byte[] EncryptBytes(byte[] bytes);
        byte[] DecryptBytes(byte[] bytes);
    }
}