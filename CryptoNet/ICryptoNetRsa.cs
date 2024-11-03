// <copyright file="ICryptoNetRsa.cs" company="NextBix" year="2024">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;

namespace CryptoNet;

/// <summary>
/// Defines RSA-specific cryptographic operations, such as retrieving and saving RSA keys.
/// </summary>
public interface ICryptoNetRsa : ICryptoNet
{
    /// <summary>
    /// Retrieves the RSA key as a string, either the private or public key.
    /// </summary>
    /// <param name="privateKey">If set to <c>true</c>, retrieves the private key; otherwise, retrieves the public key.</param>
    /// <returns>A string representation of the RSA key.</returns>
    string GetKey(bool privateKey = false);

    /// <summary>
    /// Saves the RSA key to a specified file, either the private or public key.
    /// </summary>
    /// <param name="fileInfo">A <see cref="FileInfo"/> object representing the file where the key will be saved.</param>
    /// <param name="privateKey">If set to <c>true</c>, saves the private key; otherwise, saves the public key.</param>
    void SaveKey(FileInfo fileInfo, bool privateKey = false);

    /// <summary>
    /// Saves the RSA key to a specified file path, either the private or public key.
    /// </summary>
    /// <param name="filename">The path of the file where the key will be saved.</param>
    /// <param name="privateKey">If set to <c>true</c>, saves the private key; otherwise, saves the public key.</param>
    void SaveKey(string filename, bool privateKey = false);
}
