using CryptoNetLib.helpers;

namespace CryptoNetLib
{
    public interface ICryptoNet
    {
        KeyHelper.KeyType GetKeyType();
        string ExportPublicKey();
        string ExportPrivateKey();
        byte[] EncryptFromString(string content);
        string DecryptToString(byte[] bytes);
        byte[] EncryptFromBytes(byte[] bytes);
        byte[] DecryptToBytes(byte[] bytes);
    }
}