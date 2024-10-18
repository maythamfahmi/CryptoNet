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
        var tmpDirPrefix = $"{nameof(AESExampleTests)}.{nameof(AESExampleSmokeTest)}-{Guid.NewGuid().ToString("D")}";

        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        using (var tmpDir = new TempDirectory())
        {
            var result = await Cli.Wrap("AESExample.exe")
                .WithWorkingDirectory(tmpDir.DirectoryInfo.FullName)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync();
        }

        var stdOut = stdOutBuffer.ToString();
        var stdErr = stdErrBuffer.ToString();

        Console.WriteLine(stdOut);
        Console.Error.WriteLine(stdErr);

        Assert.IsNotEmpty(stdOut);
        Assert.IsEmpty(stdErr);

        Assert.Fail("AESExample is incomplete and so is this test.");
    }
}
