![Cryptonet](https://raw.githubusercontent.com/maythamfahmi/CryptoNet/main/img/CryptoNetLogo.svg)

[![GitHub](https://img.shields.io/github/license/maythamfahmi/cryptonet)](https://github.com/maythamfahmi/CryptoNet/blob/main/LICENSE)
[![CryptoNet NuGet version](https://img.shields.io/nuget/v/CryptoNet?color=blue)](https://www.nuget.org/packages/CryptoNet/)
[![Passing build workflow](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml/badge.svg)](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml)
[![Generic badge](https://img.shields.io/badge/support-.NET%20Standard%202.0-blue.svg)](https://github.com/maythamfahmi/CryptoNet)
![Code Coverage](./coverage-badge.svg)

# Introduction
:rocket: CryptoNet is simple, fast, and a lightweight asymmetric and symmetric encryption NuGet library supporting .NET Standard 2.0 and C# 8.0 for cross platforms Windows, Linux, and iOS.
It is a 100% native C# implementation based on [Microsoft](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography?view=net-8.0) cryptography.
It does not depend on other libraries.

## Installation

You can install the CryptoNet NuGet package via [NuGet](https://www.nuget.org/packages/CryptoNet).

## Website

https://maythamfahmi.github.io/CryptoNet

## Versions

[![Nuget](https://img.shields.io/nuget/v/cryptonet?style=social)](https://www.nuget.org/packages/CryptoNet/) is latest version and are maintained. 

List of features:
- RSA asymmetric encryption.
- AES symmetric encryption.
- RSA PEM exporting and importing.
- Support for X509Certificate2.
- Ability to encrypt and decrypt text, and files like images, word, excel, etc.
- Cross-platform compatible with Windows, Linux, and iOS.

## Issues

Please report issues [here](https://github.com/maythamfahmi/CryptoNet/issues).

## How to use

### Short intro

The library can be used in 2 ways:

* Symmetrically encryption
* Asymmetrically encryption (public key encryption)

#### Symmetric encryption
You use the same key (any secret key) for encryption and decryption.

#### Asymmetric encryption
Asymmetrically, the library can use its own self-generated RSA key pairs (Private/Public key) to encrypt and decrypt content.

You can store the private key on one or more machines. The public key can easily be distributed to all clients.

> I'd like to point out that you don't distribute private keys publicly and keep them in a safe place. You need to reissue new keys if a private key mistakenly gets exposed. The content already encrypted with the private key, can not be decrypted back with the newly generated private key. So before updating the private key or deleting the old key ensure all your content is decrypted, otherwise, you lose the content.

It is also possible to use asymmetric keys of the X509 Certificate instead of generating your keys.

The main concept with asymmetric encryption is that you have a Private and Public key. You use the Public key to encrypt the content and use the Private key to decrypt the content back again.

Read more about asymmetric or public key encryption [here](https://www.cloudflare.com/learning/ssl/what-is-asymmetric-encryption/)

You find the complete and all examples for:

- RSA encryption [here](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNet.Cli/ExampleRsa.cs)
- AES encryption [here](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNet.Cli/ExampleAes.cs) 


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

## Build and Testing
You have different options to build and run the unit tests from:
 1. Visual Studio 2019/2022.
 2. Visual Studio Code.
 3. dotnet command line.
 4. dotnet commands are preserved in a PowerShell script ```build.ps1```.
 5. Docker, run the following command from the solution folder:

```
docker build . --file .\Dockerfile --tag cryptonet-service:latest
```

## Contributing

You are more than welcome to contribute in one of the following ways:

1. Basic: Give input, and suggestions for improvement by creating an issue and labeling it https://github.com/maythamfahmi/CryptoNet/issues
2. Advance: if you have good knowledge of C# and Cryptography just grab one of the issues, or features, or create a new one and refactor and add a pull request.
3. Documentation: Add, update, or improve documentation, by making a pull request.

### How to contribute:

[Here](https://www.dataschool.io/how-to-contribute-on-github/) is a link to learn how to contribute if you are not aware of how to do it.
