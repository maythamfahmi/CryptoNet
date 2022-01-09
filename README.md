![Cryptonet](https://raw.githubusercontent.com/maythamfahmi/CryptoNet/main/img/CryptoNetLogo.svg)

[![GitHub](https://img.shields.io/github/license/maythamfahmi/cryptonet)](https://github.com/maythamfahmi/CryptoNet/blob/main/LICENSE)
[![CryptoNet NuGet version](https://img.shields.io/nuget/v/CryptoNet?color=blue)](https://www.nuget.org/packages/CryptoNet/)
[![Passing build workflow](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml/badge.svg)](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml)
[![Generic badge](https://img.shields.io/badge/support-.NET%20Standard%202.0-blue.svg)](https://github.com/bezzad/Downloader)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f)](https://github.com/hassanhabib/The-Standard)
[![BCH compliance](https://bettercodehub.com/edge/badge/maythamfahmi/CryptoNet?branch=main)](https://bettercodehub.com/)


# Introdution
:rocket: CryptoNet is simple, fast and a lightweight asymmetric and symmetric (*) encryption NuGet library supporting .NET Standard 2.0 for cross platforms Windows, Linux, iOS.
It is a 100% native C# implementation based on [RSA](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsa?view=net-6.0) factory class.
It does not depending on other library.

> * symmetric encryption is only supported in Windows OS.

## Installation

You can download CryptoNet via [NuGet](https://www.nuget.org/packages/CryptoNet/).

## Versions

[![Nuget](https://img.shields.io/nuget/v/cryptonet?style=social)](https://www.nuget.org/packages/CryptoNet/) is latest version and are maintained. 

#### [![Nuget](https://img.shields.io/badge/nuget-v1.5.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/1.5.0) - [Release code 1.5.0](https://github.com/maythamfahmi/CryptoNet/releases/tag/v1.5.0)
- Reintroducing symmertic encryption only for Windows OS.
- Adding Source Link, Deterministic and Compiler Flags to NuGet package.
- Readme enhancement.

#### [![Nuget](https://img.shields.io/badge/nuget-v1.2.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/1.2.0) - [Release code 1.2.0](https://github.com/maythamfahmi/CryptoNet/releases/tag/v1.2.0)
- Change from RSACryptoServiceProvider to RSA factory that support cross platforms (Windows, Linux, iOS).
- No longer support for symmertic encryption from version 1.0.0.
- Console examples and Unit testing refactored.
- Support for X509Certificate2.

#### [![Nuget](https://img.shields.io/badge/nuget-v1.0.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/1.0.0) - [Release code 1.0.0](https://github.com/maythamfahmi/CryptoNet/releases/tag/v1.0.0)
- Ability to encrypt and decrypt files like, images, word, excel etc.
- Improvement documentation

## Issues

Please report issues [here](https://github.com/maythamfahmi/CryptoNet/issues).

## How to use

### Short intro

The library can be used in 2 ways:

* Symmetric way (Only supported in Windows)
* Asymmetric way

#### Symmetric way
You use the same key (any secret key) for encryption and decryption.

#### Asymmetric way
With asymmetric way, the library can use its own self-generated RSA key pairs (Private/Public key) to encrypt and decrypt content.

You can store the private key on one or more machines. The public key can easily distribute to all clients.

> Note: Please be aware of not to distribute private key publicly and keep it in a safe place. If private key mistakenly gets exposed, you need to re-issue new keys. The content that is already encrypted with private key, can not be decrypted back with the new generated private key. So before updating private key or deleting the old key ensure all your content are decrypted, other wise you lose the content.

It is also possible to use asymmetric keys of X509 Certificate instead of generating your own keys.

The main concept with asymmetric encryption, is that you have a Private and Public key. You use Public key to encrypt the content with and use Private key to decrypt the content back again.

You find the comlete and all examples of both ways [here](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNetCmd/Example.cs).

Here is some of the examples:

### Examples

### Example: Encrypt and Decrypt Content With Symmetric Key (Only windows)
```csharp
var symmetricKey = "AnySecretKey";

ICryptoNet encryptClient = new CryptoNet(symmetricKey, true);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);
Console.WriteLine($"1- We will encrypt following text:\n{ConfidentialDummyData}\n");
Console.WriteLine($"2- To:\n{CryptoNetUtils.BytesToString(encrypt)}\n");

ICryptoNet decryptClient = new CryptoNet(symmetricKey, true);
var decrypt = decryptClient.DecryptToString(encrypt);
Console.WriteLine($"3- And we will decrypt it back to:\n{decrypt}\n");
```

### Example: Encrypt and Decrypt Content With Self-Generated Asymmetric Key
```csharp
ICryptoNet cryptoNet = new CryptoNet();

var privateKey = cryptoNet.ExportPrivateKey();
var publicKey = cryptoNet.ExportPublicKey();

ICryptoNet encryptClient = new CryptoNet(publicKey);
var encrypt = encryptClient.EncryptFromString(ConfidentialDummyData);
Console.WriteLine($"1- We will encrypt following text:\n{ConfidentialDummyData}\n");
Console.WriteLine($"2- To:\n{CryptoNetUtils.BytesToString(encrypt)}\n");

ICryptoNet decryptClient = new CryptoNet(privateKey);
var decrypt = decryptClient.DecryptToString(encrypt);
Console.WriteLine($"3- And we will decrypt it back to:\n{decrypt}\n");
```

### Example: Generate and Export Asymmetric Key (Private/Public) Key (RasKeyPair)
```csharp
ICryptoNet cryptoNet = new CryptoNet();

CryptoNetUtils.SaveKey(PrivateKeyFile, cryptoNet.ExportPrivateKey());
CryptoNetUtils.SaveKey(PublicKeyFile, cryptoNet.ExportPublicKey());

var privateKey = CryptoNetUtils.LoadFileToString(PrivateKeyFile);
Console.WriteLine($"The private key generated and saved to file {PrivateKeyFile}");
Console.WriteLine(privateKey);

var publicKey = CryptoNetUtils.LoadFileToString(PublicKeyFile);
Console.WriteLine($"\nThe public key generated and saved to file {PublicKeyFile}");
Console.WriteLine(publicKey);
```

### Example: Encrypt with Public Key and later Decrypt with Private Key
```csharp
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
Console.WriteLine("2- And use the same certificate to decrypt");
Console.WriteLine(decryptWithPrivateKey);
```

### Example: Use X509 certificate to Encrypt with Public Key and later Decrypt with Private Key
```csharp
// Find and replace CN=Maytham with your own certificate
X509Certificate2? certificate = CryptoNetUtils.GetCertificateFromStore("CN=Maytham");

ICryptoNet cryptoNetWithPublicKey = new CryptoNet(certificate, KeyHelper.KeyType.PublicKey);
var encryptWithPublicKey = cryptoNetWithPublicKey.EncryptFromString(ConfidentialDummyData);
Console.WriteLine($"1- We get public key from Certificate to encrypt following text:\n{ConfidentialDummyData}\n");
Console.WriteLine($"2- To:\n{CryptoNetUtils.BytesToString(encryptWithPublicKey)}\n");

ICryptoNet cryptoNetWithPrivateKey = new CryptoNet(certificate, KeyHelper.KeyType.PrivateKey);
var decryptWithPrivateKey = cryptoNetWithPrivateKey.DecryptToString(encryptWithPublicKey);
Console.WriteLine($"3- And we get private key from Certificate to decrypt it back to:\n{decryptWithPrivateKey}");
```

## Build and Testing
You have different options to build and run unit test from:
 1. Visual Studio 2019/2022.
 2. dotnet command line.
 3. Powershell, run build.ps1 from solution folder.
 4. Docker, run following command from solution folder:

```
docker build . --file .\Dockerfile --tag cryptonet-service:latest
```

## Contributing

I need your help, so if you have good knowledge of C# and Cryptography just grab one of the issues and add a pull request.
The same is valid, if you have idea for improvement, adding new feature or even documentation improvement and enhancemnet, you are more than welcome to contribute.

### How to contribute:

[Here](https://www.dataschool.io/how-to-contribute-on-github/) is a link to learn how to contribute if you are not a ware of how to do it.
