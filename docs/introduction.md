# Introduction

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
