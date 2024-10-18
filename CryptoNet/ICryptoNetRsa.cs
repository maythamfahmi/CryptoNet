﻿// <copyright file="ICryptoNetRsa.cs" company="NextBix" year="2024">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

namespace CryptoNet;

public interface ICryptoNetRsa : ICryptoNet
{
    string ExportKey(bool privateKey = false);
    void ExportKeyAndSave(FileInfo fileInfo, bool privateKey = false);
}