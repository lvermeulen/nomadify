using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using Nomadify.Commands.Options;
using Nomadify.Execution;
using Nomadify.Execution.Exceptions;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Processors;

public class DockerfileProcessor(IAnsiConsole console, IProcessRunner processRunner) 
    : BaseContainerProcessor(console, processRunner)
{
    private readonly Dictionary<string, List<string>> _containerImageCache = [];

    public override Resource? Deserialize(ref Utf8JsonReader reader) => JsonSerializer.Deserialize<DockerfileResource>(ref reader);

    public override List<object> CreateNomadObjects()
    {
        return [];
    }

    private Task<ProcessRunResult> BuildContainer(DockerfileResource dockerfileResource, string? builder, List<string> tags, string dockerfilePath)
    {
        var argumentBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerConstants.BuildCommand, string.Empty, quoteValue: false);

        foreach (var tag in tags)
        {
            argumentBuilder.AppendArgument(DockerConstants.TagArgument, tag.ToLower(), allowDuplicates: true);
        }

        if (dockerfileResource.Env is not null)
        {
            AddDockerBuildArgs(argumentBuilder, dockerfileResource.Env);
        }

        if (dockerfileResource.BuildArgs is not null)
        {
            AddDockerBuildArgs(argumentBuilder, dockerfileResource.BuildArgs);
        }

        if (string.IsNullOrEmpty(dockerfileResource.Context))
        {
            dockerfileResource.Context = dockerfilePath;
        }

        argumentBuilder
            //.AppendArgument(DockerConstants.FileArgument, Path.GetFileName(fullDockerfilePath))
            .AppendArgument(dockerfileResource.Context, string.Empty, quoteValue: false);

        return ProcessRunner.ExecuteCommand(new()
        {
            Command = builder,
            ArgumentsBuilder = argumentBuilder,
            NonInteractive = true,
            ShowOutput = true,
        });
    }

    private async Task<ProcessRunResult> PushContainer(string? builder, string? registry, List<string> fullImages)
    {
        if (!string.IsNullOrEmpty(registry))
        {
            ProcessRunResult? result = null;

            foreach (var fullImage in fullImages)
            {
                var argumentBuilder = ArgumentsBuilder
                    .Create()
                    .AppendArgument(DockerConstants.PushCommand, string.Empty, quoteValue: false)
                    .AppendArgument(fullImage.ToLower(), string.Empty, quoteValue: false, allowDuplicates: true);

                result = await ProcessRunner.ExecuteCommand(new()
                {
                    Command = builder,
                    ArgumentsBuilder = argumentBuilder,
                    NonInteractive = true,
                    ShowOutput = true,
                });

                if (!result.Success)
                {
                    break;
                }
            }

            return result!;
        }

        return new ProcessRunResult(true, string.Empty, string.Empty, 0);
    }

    public async Task BuildAndPushContainerForDockerfile((string Key, Resource Value) resource, ContainerOptions options)
    {
        var dockerfileResource = resource.Value as DockerfileResource;

        ArgumentNullException.ThrowIfNull(dockerfileResource);
        ArgumentNullException.ThrowIfNull(options);

        await CheckIfContainerBuilderIsRunning(options.ContainerBuilder);

        var dockerfilePath = Path.GetDirectoryName(dockerfileResource.Path)!.GetFullPath();
        var fullImages = options.ToImageNames(dockerfileResource.Name);

        var result = await BuildContainer(dockerfileResource, options.ContainerBuilder, fullImages, dockerfilePath);
        if (result.ExitCode != 0)
        {
            ExitCodeException.ExitNow(result.ExitCode);
        }

        result = await PushContainer(options.ContainerBuilder, options.Registry, fullImages);
        if (result.ExitCode != 0)
        {
            ExitCodeException.ExitNow(result.ExitCode);
        }

        Console.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Building and Pushing container for Dockerfile [blue]{resource.Key}[/]");
    }

    public void PopulateContainerImageCacheWithImage((string Key, Resource Value) resource, ContainerOptions options)
    {
        _containerImageCache.Add(resource.Key, options.ToImageNames(resource.Key));

        Console.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Setting container details for Dockerfile [blue]{resource.Key}[/]");
    }

    private static void AddDockerBuildArgs(ArgumentsBuilder argumentsBuilder, Dictionary<string, string> dockerfileEnv)
    {
        foreach (var (key, value) in dockerfileEnv)
        {
            argumentsBuilder.AppendArgument(DockerConstants.BuildArgArgument, $"{key}=\"{value}\"", quoteValue: false, allowDuplicates: true);
        }
    }
}
