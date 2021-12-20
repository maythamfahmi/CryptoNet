![Cryptonet](https://itbackyard.com/wp-content/uploads/2021/12/CryptoNetLogo.png)

[![CryptoNet NuGet version](https://img.shields.io/nuget/v/CryptoNet)](https://www.nuget.org/packages/CryptoNet/)
[![CryptoNet NuGet pre-release version](https://img.shields.io/nuget/vpre/CryptoNet)](https://www.nuget.org/packages/CryptoNet/)

<b>CryptoNet</b> is simple and lightweight symmetric and asymmetric encryption library. 
It is a 100% native C# implementation based on RSACryptoServiceProvider.

## Installation

You can download CryptoNet via [NuGet](https://www.nuget.org/packages/CryptoNet/).

## Versions

Currently version are maintained 

## Using / Documentation

Here is a simple example:

Check more [examples](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNetCmd/Example.cs).

### Example(1) Encrypt and Decrypt with Asymmetric Key
```csharp
var encryptClient = new CryptoNet(AsymmetricKey);
Console.WriteLine("1- We will encrypt following:");
Console.WriteLine(ConfidentialDummyData);

var encrypted = encryptClient.Encrypt(ConfidentialDummyData);
Console.WriteLine("2- To:");
Console.WriteLine(CryptoNetUtils.BytesToString(encrypted));

var decryptClient = new CryptoNet(AsymmetricKey);
var decrypted = decryptClient.Decrypt(encrypted);
Console.WriteLine("3- And we will decrypt it back with correct key:");
Console.WriteLine(decrypted);

var decryptClientWithWrongKey = new CryptoNet("wrong key");
var decryptWithWrongKey = decryptClientWithWrongKey.Decrypt(encrypted);
Console.WriteLine("4- And we will not be able decrypt it back with wrong key:");
Console.WriteLine(decryptWithWrongKey);
```

### Example(3) SelfGenerated Private Key (RasKeyPair)
```csharp
CryptoNet cryptoNet = new CryptoNet("My-Secret-Key");

CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());

var certificate = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
```

### Example(4) Encrypt with Public Key and later Decrypt with Private Key
```csharp
var certificate = CryptoNetUtils.LoadFileToString(RsaKeyPair);
// Export public key
ICryptoNet cryptoNet = new CryptoNet(certificate, true);
var publicKey = cryptoNet.ExportPublicKey();
CryptoNetUtils.SaveKey(PublicKeyFile, publicKey);

// Import public key and encrypt
var importPublicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
ICryptoNet cryptoNetEncryptWithPublicKey = new CryptoNet(importPublicKey, true);
var encryptWithPublicKey = cryptoNetEncryptWithPublicKey.Encrypt(ConfidentialDummyData);
Console.WriteLine("1- This time we use a certificate public key to encrypt");
Console.WriteLine(CryptoNetUtils.BytesToString(encryptWithPublicKey));

ICryptoNet cryptoNetDecryptWithPublicKey = new CryptoNet(certificate, true);
var decryptWithPrivateKey = cryptoNetDecryptWithPublicKey.Decrypt(encryptWithPublicKey);
Console.WriteLine("6- And use the same certificate to decrypt");
Console.WriteLine(decryptWithPrivateKey);
```


## Contributing

We need help, so if you have good knowledge of C# and Cryptography just grab one of the issues and add a pull request.
The same is valid, if you have idea for improvement or adding new feature.

<!--Regarding coding standards, we are using C# coding styles, to be a little more specific: we are using `camelCase` for variables and fields (with `m_` prefix for instance members and `s_` for static fields) and `PascalCase` for methods, classes and constants. Make sure you are using 'Insert Spaces' and 4 for tab and indent size.-->
