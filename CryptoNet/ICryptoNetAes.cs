// <copyright file="ICryptoNetAes.cs" company="NextBix" year="2024">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System;
using System.IO;

namespace CryptoNet;

public interface ICryptoNetAes : ICryptoNet
{
    string GetKey();
    void SaveKey(FileInfo fileInfo);
    void SaveKey(string filename);
}