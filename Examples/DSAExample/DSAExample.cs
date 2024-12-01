// Copyright and trademark notices at the end of this file.

// Demonstrates a sender (encryptor) and a receiver (decryptor),
// using a shared key.

using CryptoNet;
using CryptoNet.ExtShared;

var confidentialMessage = "Watson, can you hear me?";

Console.WriteLine($"Original: {confidentialMessage}");

var sharedKey = CreateKey();

var signature = SimulateSignature(sharedKey, confidentialMessage);

Console.WriteLine("Encrypted: " + BitConverter.ToString(signature).Replace("-", ""));

var decryptedMessage = SimulateVerify(sharedKey, signature);

Console.WriteLine($"Decrypted: {decryptedMessage}");

////////////////////////
//

// Demonstrates key creation.
string CreateKey()
{
    ICryptoNetDsa client = new CryptoNetDsa();

    return client.GetKey(true);
}

// Demonstrates how to create a signature with a key.
byte[] SimulateSignature(string key, string message)
{
    ICryptoNetDsa signatureClient = new CryptoNetDsa(key);

    return signatureClient.CreateSignature(message);
}

// Demonstrates how to verfiy a signature with a key.
bool SimulateVerify(string key, byte[] signature)
{
    ICryptoNetDsa verifyClient = new CryptoNetDsa(key);

    return verifyClient.IsContentVerified(ExtShared.StringToBytes(confidentialMessage), signature);
}

// Copyright CryptoNet contributors.
//
// The MIT License is a permissive free software license. The original MIT License text is as follows:
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