// <copyright file="ICryptoNet.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;
using CryptoNet.Models;

namespace CryptoNet;

/// <summary>
/// Defines generic cryptographic operations for encryption and decryption of data, with options to handle strings and byte arrays.
/// </summary>
public interface ICryptoNet
{
    /// <summary>
    /// Gets the cryptographic information associated with this instance, including algorithm details and configuration.
    /// </summary>
    CryptoNetInfo Info { get; }

    /// <summary>
    /// Encrypts the specified string content and returns the encrypted data as a byte array.
    /// </summary>
    /// <param name="content">The plaintext string content to encrypt.</param>
    /// <returns>A byte array representing the encrypted content.</returns>
    byte[] EncryptFromString(string content);

    /// <summary>
    /// Decrypts the specified byte array and returns the decrypted data as a string.
    /// </summary>
    /// <param name="bytes">The byte array containing encrypted data.</param>
    /// <returns>The decrypted string content.</returns>
    string DecryptToString(byte[] bytes);

    /// <summary>
    /// Encrypts the specified byte array and returns the encrypted data as a new byte array.
    /// </summary>
    /// <param name="bytes">The plaintext byte array to encrypt.</param>
    /// <returns>A byte array representing the encrypted content.</returns>
    byte[] EncryptFromBytes(byte[] bytes);

    /// <summary>
    /// Decrypts the specified byte array and returns the decrypted data as a new byte array.
    /// </summary>
    /// <param name="bytes">The byte array containing encrypted data.</param>
    /// <returns>A byte array representing the decrypted content.</returns>
    byte[] DecryptToBytes(byte[] bytes);
}
