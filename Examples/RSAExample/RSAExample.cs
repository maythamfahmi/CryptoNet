// Copyright and trademark notices at the end of this file.

using CryptoNet;

// Demonstrates a sender (encryptor) and a receiver (decryptor),
// using asymetric keys, stored in seperate files.

var privateKeyFile = "RSAExample.Private.KeyFile";
var publicKeyFile = "RSAExample.Public.KeyFile";
var confidentialMessage = "Watson, can you hear me?";

Console.WriteLine($"Original: {confidentialMessage}");

SimulateKeyManagement();

var cypher = SimulateEncryptor(confidentialMessage);

Console.WriteLine("Encrypted: " + BitConverter.ToString(cypher).Replace("-", ""));

var decryptedMessage = SimulateDecryptor(cypher);

Console.WriteLine($"Decrypted: {decryptedMessage}");

////////////////////////
//

void SimulateKeyManagement()
{
    // Create keys.
    ICryptoNetRsa cryptoNet = new CryptoNetRsa();

    // Secure the private key and distribute the public key.
    cryptoNet.SaveKey(new FileInfo(privateKeyFile), true);
    cryptoNet.SaveKey(new FileInfo(publicKeyFile), false);
}

byte[] SimulateEncryptor(string confidentialData)
{
    // Sender retrieves public key from key store.
    ICryptoNetRsa cryptoNetPubKey = new CryptoNetRsa(new FileInfo(publicKeyFile));
    
    // Public key is used to encrypt the message.
    return cryptoNetPubKey.EncryptFromString(confidentialData);
}

string SimulateDecryptor(byte[] cypher)
{
    // Receiver retrieves private key from key store.
    ICryptoNetRsa cryptoNetPriKey = new CryptoNetRsa(new FileInfo(privateKeyFile));

    return cryptoNetPriKey.DecryptToString(cypher);
}

// Copyright CryptoNet contributors.
//
// The MIT License is a permissive free software license.The original MIT License text is as follows:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.