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

public interface ICryptoNet
{
    CryptoNetInfo Info { get; }
    byte[] EncryptFromString(string content);
    string DecryptToString(byte[] bytes);
    byte[] EncryptFromBytes(byte[] bytes);
    byte[] DecryptToBytes(byte[] bytes);
}