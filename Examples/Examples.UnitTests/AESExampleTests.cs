// <copyright file="AESExampleTests.cs"">
// Copyright (c) All Rights Reserved
// </copyright>
// <author>Maytham Fahmi and contributors</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

namespace CryptoNet.Examples.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AESExampleTests
{
    [Test]
    public async Task AESExampleSmokeTest()
    {
        // This provides a human readable temporary directory name prefix.
        // If you see a lot of these laying around your temp directory, it's
        // probably due to some failures in this test.
        var tmpDirPrefix = $"{nameof(AESExampleTests)}.{nameof(AESExampleSmokeTest)}-";

        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        using (var tmpDir = new TempDirectory(tmpDirPrefix))
        {
            var result = await Cli.Wrap("AESExample.exe")
                .WithWorkingDirectory(tmpDir.DirectoryInfo.FullName)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync();
        }

        var stdOut = stdOutBuffer.ToString().Trim();
        var stdErr = stdErrBuffer.ToString();

        Console.WriteLine(stdOut);
        Console.Error.WriteLine(stdErr);

        Assert.IsNotEmpty(stdOut);
        Assert.IsEmpty(stdErr);

        Assert.IsTrue(stdOut.StartsWith("Original: Watson, can you hear me?"));
        Assert.IsTrue(stdOut.Contains("Encrypted:"));
        Assert.IsTrue(stdOut.EndsWith("Decrypted: Watson, can you hear me?"));
    }
}
