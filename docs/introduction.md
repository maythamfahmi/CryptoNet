# Introduction

### Short Intro

The library can be used in two ways:

- **Symmetric encryption**
- **Asymmetric encryption** (public key encryption)

#### Symmetric Encryption

You use the same key (any secret key) for both encryption and decryption.

#### Asymmetric Encryption

With asymmetric encryption, the library can use its own self-generated RSA key pairs (Private/Public keys) to encrypt and decrypt content.

You can store the private key on one or more machines, while the public key can be easily distributed to all clients.

> **Important:** Do not distribute private keys publicly; keep them in a safe place. If a private key is mistakenly exposed, you need to reissue new keys. Content already encrypted with the compromised private key cannot be decrypted with a newly generated private key. Before updating or deleting the old private key, ensure all encrypted content is decrypted, or you risk losing access to that content.

Additionally, it is possible to use asymmetric keys from an X.509 certificate instead of generating your own keys.

The main concept of asymmetric encryption is the use of a Private key and a Public key:
- Use the **Public key** to encrypt content.
- Use the **Private key** to decrypt the content.

Read more about asymmetric (public key) encryption [here](https://www.cloudflare.com/learning/ssl/what-is-asymmetric-encryption/).

You can find complete examples for:

- [RSA asymmetric cryptographic algorithm](https://github.com/maythamfahmi/CryptoNet/blob/main/Examples/RSAExample/RSAExample.cs)
- [AES symmetric cryptographic algorithm](https://github.com/maythamfahmi/CryptoNet/blob/main/Examples/AESExample/AESExample.cs)
- [DSA asymmetric cryptographic algorithm](https://github.com/maythamfahmi/CryptoNet/blob/main/Examples/DSAExample/DSAExample.cs)
