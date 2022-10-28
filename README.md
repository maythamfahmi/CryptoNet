![Cryptonet](https://raw.githubusercontent.com/maythamfahmi/CryptoNet/main/img/CryptoNetLogo.svg)

[![GitHub](https://img.shields.io/github/license/maythamfahmi/cryptonet)](https://github.com/maythamfahmi/CryptoNet/blob/main/LICENSE)
[![CryptoNet NuGet version](https://img.shields.io/nuget/v/CryptoNet?color=blue)](https://www.nuget.org/packages/CryptoNet/)
[![Passing build workflow](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml/badge.svg)](https://github.com/maythamfahmi/CryptoNet/actions/workflows/ci.yml)
[![Generic badge](https://img.shields.io/badge/support-.NET%20Standard%202.0-blue.svg)](https://github.com/maythamfahmi/CryptoNet)
[![BCH compliance](https://bettercodehub.com/edge/badge/maythamfahmi/CryptoNet?branch=main)](https://bettercodehub.com)


# Introdution
:rocket: CryptoNet is simple, fast and a lightweight asymmetric and symmetric encryption NuGet library supporting .NET Standard 2.0 and C# 8.0 for cross platforms Windows, Linux, iOS.
It is a 100% native C# implementation based on [Microsoft](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography?view=net-6.0) cryptography.
It does not depending on other library.

## Installation

You can download CryptoNet via [NuGet](https://www.nuget.org/packages/CryptoNet).

## Website

https://maythamfahmi.github.io/CryptoNet

## Versions

[![Nuget](https://img.shields.io/nuget/v/cryptonet?style=social)](https://www.nuget.org/packages/CryptoNet/) is latest version and are maintained. 

#### [![Nuget](https://img.shields.io/badge/nuget-v2.2.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/2.2.0) [![Release%20Code](https://img.shields.io/badge/release%20code-v2.2.0-blue?style=social)](https://github.com/maythamfahmi/CryptoNet/releases/tag/v2.2.0)
- Minor updates
- Corrected texts

#### [![Nuget](https://img.shields.io/badge/nuget-v2.1.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/2.1.0) [![Release%20Code](https://img.shields.io/badge/release%20code-v2.1.0-blue?style=social)](https://github.com/maythamfahmi/CryptoNet/releases/tag/v2.1.0)
- !!!Breaking change!!!
- Refactoring RSA asymmetric encryption.
- Introducing AES symmetric encryption.
- Adapt RSA PEM exporting and importing helpers with example.
- Windows symmetric encryption from v.1.6 is no longer avaible.

#### [![Nuget](https://img.shields.io/badge/nuget-v1.6.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/1.6.0) [![Release%20Code](https://img.shields.io/badge/release%20code-v1.6.0-blue?style=social)](https://github.com/maythamfahmi/CryptoNet/releases/tag/v1.6.0)
- Adapt RSA instance for customization.
- Adapt RSA customization example for PEM exporting and importing.

#### [![Nuget](https://img.shields.io/badge/nuget-v1.5.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/1.5.0) [![Release%20Code](https://img.shields.io/badge/release%20code-v1.5.0-blue?style=social)](https://github.com/maythamfahmi/CryptoNet/releases/tag/v1.5.0)
- Reintroducing symmetric encryption only for Windows OS.
- Adding Source Link, Deterministic and Compiler Flags to NuGet package.
- Readme enhancement.

#### [![Nuget](https://img.shields.io/badge/nuget-v1.2.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/1.2.0) [![Release%20Code](https://img.shields.io/badge/release%20code-v1.2.0-blue?style=social)](https://github.com/maythamfahmi/CryptoNet/releases/tag/v1.2.0)
- Change from RSACryptoServiceProvider to RSA factory that support cross platforms (Windows, Linux, iOS).
- No longer support for symmetric encryption from version 1.0.0.
- Console examples and Unit testing refactored.
- Support for X509Certificate2.

#### [![Nuget](https://img.shields.io/badge/nuget-v1.0.0-blue?style=social)](https://www.nuget.org/packages/CryptoNet/1.0.0) [![Release%20Code](https://img.shields.io/badge/release%20code-v1.0.0-blue?style=social)](https://github.com/maythamfahmi/CryptoNet/releases/tag/v1.0.0)

- Ability to encrypt and decrypt files like, images, word, excel etc.
- Improvement documentation

## Issues

Please report issues [here](https://github.com/maythamfahmi/CryptoNet/issues).

## How to use

### Short intro

The library can be used in 2 ways:

* Symmetric way
* Asymmetric way

#### Symmetric way
You use the same key (any secret key) for encryption and decryption.

#### Asymmetric way
With asymmetric way, the library can use its own self-generated RSA key pairs (Private/Public key) to encrypt and decrypt content.

You can store the private key on one or more machines. The public key can easily distribute to all clients.

> Note: Please be aware of not to distribute private key publicly and keep it in a safe place. If private key mistakenly gets exposed, you need to re-issue new keys. The content that is already encrypted with private key, can not be decrypted back with the new generated private key. So before updating private key or deleting the old key ensure all your content are decrypted, other wise you lose the content.

It is also possible to use asymmetric keys of X509 Certificate instead of generating your own keys.

The main concept with asymmetric encryption, is that you have a Private and Public key. You use Public key to encrypt the content with and use Private key to decrypt the content back again.

You find the comlete and all examples for:

- Rsa encryption [here](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNet.Cli/ExampleRsa.cs)
- Aes encryption [here](https://github.com/maythamfahmi/CryptoNet/blob/main/CryptoNet.Cli/ExampleAes.cs) 


Here is some of the examples:

### Examples

### Example: Encrypt and Decrypt Content With Symmetric Key
In this example CryptoNetAes generate random key and iv, hence we use the same instance we can both encrypt and decrypt.
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

### Example: Generate Asymmetric Rsa key pair, Export Private and Public, use Public key to encrypt with and Use Private key to decrypt with
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
You have different options to build and run unit test from:
 1. Visual Studio 2019/2022.
 2. dotnet command line.
 3. start Powershell, and run ```build.ps1``` from solution folder.
 4. Docker, run following command from solution folder:

```
docker build . --file .\Dockerfile --tag cryptonet-service:latest
```

## Contributing

You are more than welcome to contribute in one of the following ways:

1. Basic: Give input, suggestion for improvement by creating issue and lable it https://github.com/maythamfahmi/CryptoNet/issues
2. Advance: if you have good knowledge of C# and Cryptography just grab one of the issues, feature or refactoring and add a pull request.
3. Documentation: Add, update or improve documentation, by makeing pull request.

### How to contribute:

[Here](https://www.dataschool.io/how-to-contribute-on-github/) is a link to learn how to contribute if you are not a ware of how to do it.
