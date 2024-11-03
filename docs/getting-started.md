# Getting Started

Here are some examples:

### Examples

### Example: Encrypt and Decrypt Content With Symmetric Key
In this example CryptoNetAes generates a random key and IV, hence we use the same instance we can both encrypt and decrypt.
```csharp
ICryptoNet cryptoNet = new CryptoNetAes();
var key = cryptoNet.ExportKey();

ICryptoNet encryptClient = new CryptoNetAes(key);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);

ICryptoNet decryptClient = new CryptoNetAes(key);
var decrypt = decryptClient.DecryptToString(encrypt);

Debug.Assert(ConfidentialDummyData == decrypt);
```

### Example: Encrypt and Decrypt Content With Export and Import Self-Generated Symmetric Key
```csharp
ICryptoNet cryptoNet = new CryptoNetAes();
var file = new FileInfo(SymmetricKeyFile);
cryptoNet.ExportKeyAndSave(file);

Debug.Assert(File.Exists(file.FullName));

var encrypt = cryptoNet.EncryptFromString(ConfidentialDummyData);
        
ICryptoNet cryptoNetKeyImport = new CryptoNetAes(file);
var decrypt = cryptoNetKeyImport.DecryptToString(encrypt);

Debug.Assert(ConfidentialDummyData == decrypt);
```

### Example: Generate Asymmetric RSA key pair, Export Private and Public, use Public key to encrypt with and Use Private key to decrypt with
```csharp
ICryptoNet cryptoNet = new CryptoNetRsa();

cryptoNet.ExportKeyAndSave(new FileInfo(PrivateKeyFile), true);
cryptoNet.ExportKeyAndSave(new FileInfo(PublicKeyFile), false);

Debug.Assert(File.Exists(new FileInfo(PrivateKeyFile).FullName));
Debug.Assert(File.Exists(new FileInfo(PublicKeyFile).FullName));

ICryptoNet cryptoNetPubKey = new CryptoNetRsa(new FileInfo(PublicKeyFile));
var encrypt = cryptoNetPubKey.EncryptFromString(ConfidentialDummyData);

ICryptoNet cryptoNetPriKey = new CryptoNetRsa(new FileInfo(PrivateKeyFile));
var decrypt = cryptoNetPriKey.DecryptToString(encrypt);

Debug.Assert(ConfidentialDummyData == decrypt);
```

### Example: Use X509 certificate to Encrypt with Public Key and later Decrypt with Private Key
```csharp
// Find and replace CN=Maytham with your own certificate
X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

ICryptoNet cryptoNetWithPublicKey = new CryptoNetRsa(certificate, KeyType.PublicKey);
var encryptWithPublicKey = cryptoNetWithPublicKey.EncryptFromString(ConfidentialDummyData);

ICryptoNet cryptoNetWithPrivateKey = new CryptoNetRsa(certificate, KeyType.PrivateKey);
var decryptWithPrivateKey = cryptoNetWithPrivateKey.DecryptToString(encryptWithPublicKey);

Debug.Assert(ConfidentialDummyData == decryptWithPrivateKey);
```
