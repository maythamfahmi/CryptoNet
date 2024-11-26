// <copyright file="ICryptoNetDsa.cs" company="NextBix" year="2024">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>Part of CryptoNet project</summary>

using CryptoNet.Models;
using System;
using System.IO;

namespace CryptoNet;

/// <summary>
/// Defines DSA-specific cryptographic operations, such as creating signatures, verifying content, 
/// and managing keys for digital signature functionality.
/// </summary>
public interface ICryptoNetDsa
{
    /// <summary>
    /// Gets the cryptographic information associated with this instance, including algorithm details and configuration.
    /// </summary>
    /// <value>A <see cref="CryptoNetInfo"/> object containing encryption type, key type, and additional details.</value>
    CryptoNetInfo Info { get; }

    /// <summary>
    /// Creates a digital signature for the specified string content.
    /// </summary>
    /// <param name="content">The plaintext string content to be signed.</param>
    /// <returns>A byte array representing the digital signature.</returns>
    byte[] CreateSignature(string content);

    /// <summary>
    /// Creates a digital signature for the specified byte array content.
    /// </summary>
    /// <param name="messageBytes">The byte array containing the content to be signed.</param>
    /// <returns>A byte array representing the digital signature.</returns>
    byte[] CreateSignature(byte[] messageBytes);

    /// <summary>
    /// Verifies the digital signature for the given string content.
    /// </summary>
    /// <param name="message">The plaintext string content to verify.</param>
    /// <param name="signature">The byte array representing the digital signature to verify against.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    bool IsContentVerified(string message, byte[] signature);

    /// <summary>
    /// Verifies the digital signature for the given byte array content.
    /// </summary>
    /// <param name="messageBytes">The byte array containing the content to verify.</param>
    /// <param name="signature">The byte array representing the digital signature to verify against.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    bool IsContentVerified(byte[] messageBytes, byte[] signature);

    /// <summary>
    /// Retrieves the DSA key as a string.
    /// </summary>
    /// <param name="privateKey">
    /// If set to <c>true</c>, retrieves the private key; otherwise, retrieves the public key.
    /// </param>
    /// <returns>A string representation of the DSA key.</returns>
    string GetKey(bool privateKey = false);

    /// <summary>
    /// Saves the DSA key to a specified file.
    /// </summary>
    /// <param name="fileInfo">
    /// A <see cref="FileInfo"/> object representing the file where the key will be saved.
    /// </param>
    /// <param name="privateKey">
    /// If set to <c>true</c>, saves the private key; otherwise, saves the public key.
    /// </param>
    void SaveKey(FileInfo fileInfo, bool privateKey = false);

    /// <summary>
    /// Saves the DSA key to a specified file path.
    /// </summary>
    /// <param name="filename">The file path where the key will be saved.</param>
    /// <param name="privateKey">
    /// If set to <c>true</c>, saves the private key; otherwise, saves the public key.
    /// </param>
    void SaveKey(string filename, bool privateKey = false);
}
