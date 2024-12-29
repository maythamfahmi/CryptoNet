![Cryptonet](https://raw.githubusercontent.com/maythamfahmi/CryptoNet/main/img/CryptoNetLogo.svg)

[![GitHub](https://img.shields.io/github/license/maythamfahmi/cryptonet)](https://github.com/maythamfahmi/CryptoNet/blob/main/LICENSE)
[![CryptoNet NuGet version](https://img.shields.io/nuget/v/CryptoNet?color=blue)](https://www.nuget.org/packages/CryptoNet/)
[![Passing build workflow](https://github.com/maythamfahmi/CryptoNet/actions/workflows/1-ci.yml/badge.svg)](https://github.com/maythamfahmi/CryptoNet/actions/workflows/1-ci.yml)
[![Generic badge](https://img.shields.io/badge/support-.NET%20Standard%202.0-blue.svg)](https://github.com/maythamfahmi/CryptoNet)
![Code Coverage](./coverage-badge.svg)
[![Build status](https://ci.appveyor.com/api/projects/status/2bnos98bkfn18pko/branch/main?svg=true)](https://ci.appveyor.com/project/maythamfahmi/cryptonet/branch/main)

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

CryptoNet v3:

- RSA: Asymmetric cryptographic algorithm used for secure data transmission and digital signatures.
- AES: Symmetric cryptographic algorithm used for fast and secure data encryption.
- DSA: Asymmetric cryptographic algorithm used for digital signatures

CryptoNet Extensions v3:

- RSA PEM exporting and importing.
- Support for X509Certificate2.
- Ability to encrypt and decrypt text, and files like images, word, excel, and any byte content.
- **Cross-platform compatible** with Windows, Linux, and iOS.

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

or run preserved PowerShell:

```powershell
./DockerBuild.ps1
```

## How to release a new version?

Preview
```
.\Release.ps1 -VersionNumber 3.0.0 -IsPreview $true
```

Release
```
.\Release.ps1 -VersionNumber 3.0.0 -IsPreview $false
```

## Documentation Creation

There are static and dynamically generated documentation. Both are published automatically in the pipeline called `5-static.yml`.

To work with the documentation locally, the following might be relevant:

- **Static base documentation** is located in the `docs` folder.
- **Dynamically generated documentation** is created from code using a tool called **DocFX**.

### Running Documentation Creation Locally

To generate the documentation locally on your computer, run:

```powershell
.\run_docs.ps1
```

### Setup

1. Install the DocFX tool (only needs to be done once):

```
dotnet tool install -g docfx
```

2. The following step is already configured in this repository. However, if you need to start over, run the following to initialize and configure DocFX:

```
docfx init -y
```

## Code Coverage

Code coverage ensures that your tests adequately cover your codebase, improving overall quality, reliability, and maintainability. Follow these steps to set up and generate code coverage reports.

### Running Code Coverage Locally

To generate code coverage reports locally on your computer, run the following command in Windows:

```powershell
.\run_codecoverage.ps1
```

### Setup

If the required tools and packages are not set up locally, follow the steps below to configure them:

1. Navigate to your test project directory (e.g., `CryptoNet.UnitTests`):

```bash
cd .\CryptoNet.UnitTests\
```

2. Add the necessary coverage packages to your test project:

```bash
dotnet add package coverlet.collector
dotnet add package coverlet.msbuild
```

3. Install the report generator tool (only needs to be done once):

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
```

Once set up, you can use these tools to analyze and generate detailed code coverage reports to ensure thorough testing of your application.

## Contributing

You are more than welcome to contribute in one of the following ways:

1. Basic: Give input, and suggestions for improvement by creating an issue and labeling it https://github.com/maythamfahmi/CryptoNet/issues
2. Advance: if you have good knowledge of C# and Cryptography just grab one of the issues, or features, or create a new one and refactor and add a pull request.
3. Documentation: Add, update, or improve documentation, by making a pull request.

### How to contribute:

[Here](https://www.dataschool.io/how-to-contribute-on-github/) is a link to learn how to contribute if you are not aware of how to do it.
