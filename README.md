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
Ref to docs.

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
