using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Container;
using Spectre.Console;
using Nomadify.Execution;
using Nomadify.Extensions;
using Nomadify.Commands.Options;

namespace Nomadify.Processors;

public class ContainerProcessor(IAnsiConsole console, IProcessRunner processRunner)
    : BaseContainerProcessor(console, processRunner)
{
    public override Resource? Deserialize(ref Utf8JsonReader reader) => JsonSerializer.Deserialize<ContainerResource>(ref reader);

    public override List<object> CreateNomadObjects()
    {
        return [];
    }

    public async Task<bool> BuildAndPushContainerForProject(ProjectResource? projectResource, MsBuildContainerProperties containerDetails, ContainerOptions options, string runtimeIdentifier)
    {
        await CheckIfContainerBuilderIsRunning(options.ContainerBuilder);

        var fullProjectPath = projectResource?.Path?.NormalizePath();

        if (!string.IsNullOrEmpty(options.Prefix))
        {
            containerDetails.ContainerRepository = $"{options.Prefix}/{containerDetails.ContainerRepository}";
        }

        var argumentsBuilder = ArgumentsBuilder.Create();
        AddProjectPublishArguments(argumentsBuilder, fullProjectPath!, runtimeIdentifier);
        AddContainerDetailsToArguments(argumentsBuilder, containerDetails);

        await ProcessRunner.ExecuteCommand(new()
        {
            Command = DotNetSdkConstants.DotNetCommand,
            ArgumentsBuilder = argumentsBuilder,
            NonInteractive = true,
            OnFailed = HandleBuildErrors,
            ShowOutput = true,
        });

        return true;
    }

    public async Task<bool> BuildAndPushDashboard(ContainerOptions options, string runtimeIdentifier)
    {
        await CheckIfContainerBuilderIsRunning(options.ContainerBuilder);

        //var argumentsBuilder = ArgumentsBuilder.Create()
        //    .AppendArgument(DockerConstants.PullCommand, );

        //await ProcessRunner.ExecuteCommand(new()
        //{
        //    Command = DockerConstants.DockerCommand,
        //    ArgumentsBuilder = argumentsBuilder,
        //    NonInteractive = true,
        //    ShowOutput = true,
        //});

        return true;
    }

    private static void AddProjectPublishArguments(ArgumentsBuilder argumentsBuilder, string fullProjectPath, string runtimeIdentifier)
    {
        argumentsBuilder
            .AppendArgument(DotNetSdkConstants.PublishArgument, fullProjectPath)
            .AppendArgument(DotNetSdkConstants.ContainerTargetArgument, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkConstants.VerbosityArgument, DotNetSdkConstants.DefaultVerbosity)
            .AppendArgument(DotNetSdkConstants.NoLogoArgument, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkConstants.RuntimeIdentifierArgument, runtimeIdentifier);
    }

    private static void AddContainerDetailsToArguments(ArgumentsBuilder argumentsBuilder, MsBuildContainerProperties containerDetails)
    {
        if (!string.IsNullOrEmpty(containerDetails.ContainerRegistry))
        {
            argumentsBuilder.AppendArgument(DotNetSdkConstants.ContainerRegistryArgument, containerDetails.ContainerRegistry);
        }

        if (!string.IsNullOrEmpty(containerDetails.ContainerRepository))
        {
            argumentsBuilder.AppendArgument(DotNetSdkConstants.ContainerRepositoryArgument, containerDetails.ContainerRepository);
        }

        if (!string.IsNullOrEmpty(containerDetails.ContainerImageName))
        {
            argumentsBuilder.AppendArgument(DotNetSdkConstants.ContainerImageNameArgument, containerDetails.ContainerImageName);
        }

        if (containerDetails.ContainerImageTag is not null && containerDetails.ContainerImageTag.Contains(';'))
        {
            argumentsBuilder.AppendArgument(DotNetSdkConstants.ContainerImageTagArguments,
                $"\\\"{containerDetails.ContainerImageTag}\\\"");
            return;
        }

        argumentsBuilder.AppendArgument(DotNetSdkConstants.ContainerImageTagArgument, containerDetails.ContainerImageTag);
    }
}
