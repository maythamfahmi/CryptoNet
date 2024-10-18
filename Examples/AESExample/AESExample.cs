// Copyright and trademark notices at the end of this file.

// Demonstrates a sender (encryptor) and a receiver (decryptor),
// using a shared key.

using CryptoNet;

var confidentialMessage = "Watson, can you hear me?";

Console.WriteLine($"Original: {confidentialMessage}");

var sharedKey = CreateKey();

var cypher = SimulateEncryptor(sharedKey, confidentialMessage);

Console.WriteLine("Encrypted: " + BitConverter.ToString(cypher).Replace("-", ""));

var decryptedMessage = SimulateDecryptor(sharedKey, cypher);

Console.WriteLine($"Decrypted: {decryptedMessage}");

////////////////////////
//

// Demonstrates key creation.
string CreateKey()
{
    ICryptoNet encoder = new CryptoNetAes();

    return encoder.ExportKey();
}

// Demonstrates how to create a cypher with a key.
byte[] SimulateEncryptor(string key, string message)
{
    ICryptoNet encryptClient = new CryptoNetAes(key);
    
    return encryptClient.EncryptFromString(message);
}

// Demonstrates how to decrypt a cypher with a key.
string SimulateDecryptor(string key, byte[] encrypted)
{
    ICryptoNet decryptClient = new CryptoNetAes(key);
    
    return decryptClient.DecryptToString(encrypted);
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