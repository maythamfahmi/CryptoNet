// <copyright file="Program.cs" company="NextBix" year="2022">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>6-2-2022 13:20:23</date>
// <summary>part of CryptoNetIaC project</summary>

using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV1s;

namespace CryptoNetIac;

public class Program
{
    public static void Main()
    {
        CiYamlGenerator();
    }
    
    public static void CiYamlGenerator(string workflowName = "ci-auto-generated-beta.yaml")
    {
        var adoClient = new ADotNetClient();

        var aspNetPipeline = new GithubPipeline()
        {
            Name = ".NET",
            
            OnEvents = new Events()
            {
                Push = new PushEvent()
                {
                    Branches = new[]
                    {
                        "main",
                        "feature/*",
                        "!feature/ci*"
                    }
                },
                PullRequest = new PullRequestEvent()
                {
                    Branches = new[]
                    {
                        "main"
                    }
                }
            },

            Jobs = new Jobs()
            {
                Build = new BuildJob()
                {
                    RunsOn = "ubuntu-latest",
                    TimeoutInMinutes = 15,
                    Steps = new List<GithubTask>()
                    {
                        new CheckoutTaskV2()
                        {
                            Name = "Checkout",
                            Uses = "actions/checkout@v2"
                        },
                        new SetupDotNetTaskV1()
                        {
                            Name = "Setup .NET",
                            Uses = "actions/setup-dotnet@v1",
                            TargetDotNetVersion = new TargetDotNetVersion()
                            {
                                DotNetVersion = "6.0.x"
                            }
                        },
                        new DotNetBuildTask()
                        {
                            Name = "Build",
                            Run = "dotnet build --configuration Release"
                        },
                        new TestTask()
                        {
                            Name = "Test",
                            Run = "dotnet test --configuration Release --no-build"
                        }
                    }
                }
            }
        };

        CreateWorkFlowFile(adoClient, aspNetPipeline, workflowName);
    }

    private static void CreateWorkFlowFile(ADotNetClient adoClient, GithubPipeline githubPipeline, string workflowName)
    {
        var solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
        var workflowPath = $"{solutionRoot}\\.github\\workflows";
        string workflowFile = Path.Combine(workflowPath, workflowName);
        adoClient.SerializeAndWriteToFile(githubPipeline, workflowFile);
    }


}
