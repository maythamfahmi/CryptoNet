// <copyright file="ICryptoNetAes.cs" company="NextBix" year="2024">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;

namespace CryptoNet;

/// <summary>
/// Defines AES-specific cryptographic operations, such as retrieving and saving the AES key.
/// </summary>
public interface ICryptoNetAes : ICryptoNet
{
    /// <summary>
    /// Retrieves the AES key as a string.
    /// </summary>
    /// <returns>A string representation of the AES key.</returns>
    string GetKey();

    /// <summary>
    /// Saves the AES key to a specified file.
    /// </summary>
    /// <param name="fileInfo">A <see cref="FileInfo"/> object representing the file where the key will be saved.</param>
    void SaveKey(FileInfo fileInfo);

    /// <summary>
    /// Saves the AES key to a specified file path.
    /// </summary>
    /// <param name="filename">The path of the file where the key will be saved.</param>
    void SaveKey(string filename);
}
