namespace CryptoNet.Examples.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class RSAExampleTests
{
    [Test]
    public async Task RSAExampleSmokeTest()
    {
        var tmpDirPrefix = $"{nameof(AESExampleTests)}.{nameof(RSAExampleSmokeTest)}-{Guid.NewGuid().ToString("D")}";

        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        using (var tmpDir = new TempDirectory())
        {
            var result = await Cli.Wrap("RSAExample.exe")
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

        Assert.Fail("RSAExample is incomplete and so is this test.");
    }
}
