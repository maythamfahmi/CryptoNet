![Cryptonet](https://raw.githubusercontent.com/maythamfahmi/CryptoNet/main/img/CryptoNetLogo.svg)

[![CryptoNet NuGet version](https://img.shields.io/nuget/v/CryptoNet?color=blue)](https://www.nuget.org/packages/CryptoNet/)
[![Passing build workflow](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml/badge.svg)](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f)](https://github.com/hassanhabib/The-Standard)
[![BCH compliance](https://bettercodehub.com/edge/badge/maythamfahmi/CryptoNet?branch=main)](https://bettercodehub.com/)
[![GitHub](https://img.shields.io/github/license/maythamfahmi/cryptonet)](https://github.com/maythamfahmi/CryptoNet/blob/main/LICENSE)

# Introdution
CryptoNet is a simple and a lightweight asymmetric encryption NuGet library. 
It is a 100% native C# implementation based on RSA factory class.

## Installation

You can download CryptoNet via [NuGet](https://www.nuget.org/packages/CryptoNet/).

## Versions

[![Nuget](https://img.shields.io/nuget/v/cryptonet?style=social)](https://www.nuget.org/packages/CryptoNet/) is latest version and are maintained. 

#### v1.2.0 (Breaking changes)
- Change from RSACryptoServiceProvider to RSA factory.
- No longer support for symmertic encryption.
- Console examples and Unit testing refactored.
- Support for X509Certificate2.

#### v1.0.0:
- Ability to encrypt and decrypt files like, images, word, excel etc.
- Improvement documentation

## Issues

Please report issues [here](https://github.com/maythamfahmi/CryptoNet/issues).

## Using / Documentation

### Short intro

The are 2 ways to encrypt and decrypt content:
 1. Asymmetric RSA key pair (self generated keys).
 2. Asymmetric X509 Certificate (use own certificate).

Both ways:
 - Have a Private and Public key.
 - Use Public key to encrypt.
 - Use Private key to decrypt.
 - For RSA way, you need to generate RSA key pair first (Private/Public key).

You find the comlete and all [examples](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNetCmd/Example.cs) here.

Here is some of the examples:

### Example: Encrypt and Decrypt Content With SelfGenerated AsymmetricKey
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
Console.WriteLine("6- And use the same certificate to decrypt");
Console.WriteLine(decryptWithPrivateKey);
```

## Build and Testing
Build and run unit test from 
 1. Visual Studio
 2. dotnet commands
 3. Powershell, run build.ps1
 4. Docker, you can also run dockerized build and test.
 From solution folder run:

```dockerfile
docker build . --file .\Dockerfile --tag cryptonet-service:latest
```

## Contributing

I need your help, so if you have good knowledge of C# and Cryptography just grab one of the issues and add a pull request.
The same is valid, if you have idea for improvement or adding new feature.

### How to contribute:

[Here](https://www.dataschool.io/how-to-contribute-on-github/) is a link to learn how to contribute if you are not a ware of how to do it.
