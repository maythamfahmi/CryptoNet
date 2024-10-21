// Copyright and trademark notices at the end of this file.

using NUnit.Framework.Legacy;

namespace CryptoNet.Examples.UnitTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class RSAExampleTests : ExampleTestBase
{
    private const string RSAExampleExeName = "RSAExample.exe";

    //[Ignore("temp")]
    [Test]
    public async Task RSAExampleSmokeTest()
    {
        var tmpDirPrefix = $"{nameof(AESExampleTests)}.{nameof(RSAExampleSmokeTest)}-{Guid.NewGuid().ToString("D")}";

        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        using (var tmpDir = new TempDirectory())
        {
            ClassicAssert.IsTrue(Directory.Exists(tmpDir.DirectoryInfo.FullName));
            ClassicAssert.IsTrue(File.Exists(RSAExampleExeName));

            ShowAvailableExecutables();

            var result = await Cli.Wrap(RSAExampleExeName)
                .WithWorkingDirectory(tmpDir.DirectoryInfo.FullName)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync();
        }

        var stdOut = stdOutBuffer.ToString().Trim();
        var stdErr = stdErrBuffer.ToString();

        Console.WriteLine(stdOut);
        Console.Error.WriteLine(stdErr);

        ClassicAssert.IsNotEmpty(stdOut);
        ClassicAssert.IsEmpty(stdErr);

        ClassicAssert.IsTrue(stdOut.StartsWith("Original: Watson, can you hear me?"));
        ClassicAssert.IsTrue(stdOut.Contains("Encrypted:"));
        ClassicAssert.IsTrue(stdOut.EndsWith("Decrypted: Watson, can you hear me?"));
    }
}

// Copyright CryptoNet contributors.
//
// The MIT License is a permissive free software license. The original MIT License text is as follows:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
