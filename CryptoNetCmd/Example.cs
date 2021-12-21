using CryptoNetLib;
using CryptoNetLib.helpers;

namespace CryptoNetCmd;

public class Example
{
    private const string AsymmetricKey = "any-secret-key-that-should-be-the-same-for-encrypting-and-decrypting";
    private const string ConfidentialDummyData = @"Some Secret Data";

    private static readonly string BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string? Root = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
    private static readonly string RsaKeyPair = Path.Combine(Root ?? string.Empty, "test.certificate");

    internal static string PrivateKeyFile = Path.Combine(BaseFolder, "privateKey");
    internal static string PublicKeyFile = Path.Combine(BaseFolder, "publicKey.pub");

    public static void Main(string[] args)
    {
        Example_1_With_AsymmetricKey();
        Example_2_With_SelfGenerated_RasKeyPair();
        Example_3_SelfGenerated_RasKeyPair();
        Example_4_With_Encrypt_With_Public_Key_Decrypt_With_SelfGenerated_RasKeyPair();
    }

    public static void Example_1_With_AsymmetricKey()
    {
        ICryptoNet encryptClient = new CryptoNet(AsymmetricKey);
        Console.WriteLine("1- We will encrypt following:");
        Console.WriteLine(ConfidentialDummyData);

        var encrypted = encryptClient.EncryptFromString(ConfidentialDummyData);
        Console.WriteLine("2- To:");
        Console.WriteLine(CryptoNetUtils.BytesToString(encrypted));

        ICryptoNet decryptClient = new CryptoNet(AsymmetricKey);
        var decrypted = decryptClient.DecryptToString(encrypted);
        Console.WriteLine("3- And we will decrypt it back with correct key:");
        Console.WriteLine(decrypted);

        ICryptoNet decryptClientWithWrongKey = new CryptoNet("wrong key");
        var decryptWithWrongKey = decryptClientWithWrongKey.DecryptToString(encrypted);
        Console.WriteLine("4- And we will not be able decrypt it back with wrong key:");
        Console.WriteLine(decryptWithWrongKey);
    }

    public static void Example_2_With_SelfGenerated_RasKeyPair()
    {
        ICryptoNet cryptoNet = new CryptoNet(CryptoNetUtils.LoadFileToString(RsaKeyPair), true);
        var encryptClient = cryptoNet.EncryptFromString(ConfidentialDummyData);
        Console.WriteLine("1- This time we use a certificate to encrypt");
        Console.WriteLine(CryptoNetUtils.BytesToString(encryptClient));

        ICryptoNet decryptClient = new CryptoNet(CryptoNetUtils.LoadFileToString(RsaKeyPair), true);
        var encrypted4 = decryptClient.DecryptToString(encryptClient);
        Console.WriteLine("6- And use the same certificate to decrypt");
        Console.WriteLine(encrypted4);
    }

    public static void Example_3_SelfGenerated_RasKeyPair()
    {
        ICryptoNet cryptoNet = new CryptoNet("My-Secret-Key");

        CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
        CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());

        var certificate = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
        var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);

        Console.WriteLine(certificate);
        Console.WriteLine();
        Console.WriteLine(publicKey);
    }

    public static void Example_4_With_Encrypt_With_Public_Key_Decrypt_With_SelfGenerated_RasKeyPair()
    {
        var certificate = CryptoNetUtils.LoadFileToString(RsaKeyPair);
        // Export public key
        ICryptoNet cryptoNet = new CryptoNet(certificate, true);
        var publicKey = cryptoNet.ExportPublicKey();
        CryptoNetUtils.SaveKey(PublicKeyFile, publicKey);

        // Import public key and encrypt
        var importPublicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
        ICryptoNet cryptoNetEncryptWithPublicKey = new CryptoNet(importPublicKey, true);
        var encryptWithPublicKey = cryptoNetEncryptWithPublicKey.EncryptFromString(ConfidentialDummyData);
        Console.WriteLine("1- This time we use a certificate public key to encrypt");
        Console.WriteLine(CryptoNetUtils.BytesToString(encryptWithPublicKey));

        ICryptoNet cryptoNetDecryptWithPublicKey = new CryptoNet(certificate, true);
        var decryptWithPrivateKey = cryptoNetDecryptWithPublicKey.DecryptToString(encryptWithPublicKey);
        Console.WriteLine("6- And use the same certificate to decrypt");
        Console.WriteLine(decryptWithPrivateKey);
    }

}
