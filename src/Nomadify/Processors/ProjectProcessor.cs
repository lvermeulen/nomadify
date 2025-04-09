using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flurl;
using Nomad.Abstractions;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using Nomad.JobGenerator;
using Nomadify.Commands.Options;
using Nomadify.Execution;
using Nomadify.Execution.Exceptions;
using Nomadify.Extensions;
using Nomadify.Interfaces;
using Spectre.Console;

namespace Nomadify.Processors;

public sealed class ProjectProcessor(IAnsiConsole console, IProcessRunner processRunner, ContainerProcessor containerProcessor, IContainerDetailsService containerDetailsService)
    : BaseResourceProcessor(console)
{
    private readonly Dictionary<string, MsBuildContainerProperties> _containerDetailsCache = [];

    public override Resource? Deserialize(ref Utf8JsonReader reader) => JsonSerializer.Deserialize<ProjectResource>(ref reader);

    public async Task BuildAndPushProjectContainer((string Key, Resource Value) resource, ContainerOptions options, string runtimeIdentifier)
    {
        var project = resource.Value as ProjectResource;

        if (!_containerDetailsCache.TryGetValue(resource.Key, out var containerDetails))
        {
            throw new InvalidOperationException($"Container details for project {resource.Key} not found.");
        }

        await containerProcessor.BuildAndPushContainerForProject(project, containerDetails, options, runtimeIdentifier);

        Console.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Building and Pushing container for project [blue]{resource.Key}[/]");
    }

    public async Task BuildAndPushDashboard(ContainerOptions options, string runtimeIdentifier)
    {
        await containerProcessor.BuildAndPushDashboard(options, runtimeIdentifier);

        Console.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Building and Pushing .NET Aspire dashboard");
    }

    public async Task<string> PublishProject((string Key, Resource Value) resource, NomadifyState state)
    {
        if (resource.Value is not ProjectResource projectResource)
        {
            return string.Empty;
        }

        var normalizedPath = projectResource.Path;
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            return string.Empty;
        }

        // get project path
        var projectDirectory = Path.GetDirectoryName(normalizedPath);

        //
        // dotnet publish . -r linux-arm --output bin\\linux-arm\\publish --no-self-contained
        //

        var outputPath = Path.Combine(projectDirectory!, @"bin\linux-arm\publish");
        EmptyDirectory(new DirectoryInfo(outputPath));

        // publish project
        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(DotNetSdkConstants.PublishArgument, string.Empty, quoteValue: false)
            .AppendArgument(normalizedPath, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkConstants.RuntimeIdentifierArgument, state.RuntimeIdentifier, quoteValue: false)
            .AppendArgument(DotNetSdkConstants.OutputArgument, outputPath);

        if (!await IsBlazorProject(normalizedPath))
        {
            argumentsBuilder.AppendArgument(DotNetSdkConstants.NoSelfContainedArgument, string.Empty, quoteValue: false);
        }

        var publishResult = await processRunner.ExecuteCommand(new()
        {
            Command = DotNetSdkConstants.DotNetCommand,
            PreCommandMessage = $"Publishing {resource.Key}...",
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = false,
        });

        if (!string.IsNullOrEmpty(publishResult.Error))
        {
            Console.MarkupLine($"[red]Error: {publishResult.Error.EscapeMarkup()}[/]");
            Console.MarkupLine($"[red]Could not publish {resource.Key}.[/]");
            ExitCodeException.ExitNow();
        }

        return outputPath;
    }

    public static async Task HandleCompression((string Key, Resource Value) resource, NomadifyState state, string publishFolder)
    {
        var settings = state.RawExecSettings;
        if (settings.CompressionKind is null || resource.Value is not ProjectResource)
        {
            return;
        }

        var outputPath = state.OutputPath!.NormalizePath();
        var outputFile = Path.Combine(outputPath, $"{resource.Key}.{state.RawExecSettings.CompressionKind}");
        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        if (state.RawExecSettings.CompressionKind!.Equals("zip", StringComparison.OrdinalIgnoreCase))
        {
            ZipFile.CreateFromDirectory(publishFolder, outputFile);
        }
        else if (state.RawExecSettings.CompressionKind.Equals("tar", StringComparison.OrdinalIgnoreCase))
        {
            await TarFile.CreateFromDirectoryAsync(publishFolder, outputFile, false);
        }
    }

    private static async Task<bool> IsBlazorProject(string projectFileName)
    {
        var text = await File.ReadAllTextAsync(projectFileName);
        var doc = XDocument.Parse(text);
        var result = doc.Descendants().Any(x => x.Attributes("Include").Any(a => a.Value.Contains("Microsoft.AspNetCore.Components.WebAssembly")));

        return result;
    }

    public static void EmptyDirectory(DirectoryInfo directory)
    {
        if (!Directory.Exists(directory.Name))
        {
            return;
        }

        foreach (var file in directory.EnumerateFiles())
        {
            file.Delete();
        }

        foreach (var subDirectory in directory.EnumerateDirectories())
        {
            subDirectory.Delete(true);
        }
    }

    public async Task PopulateContainerDetailsCacheForProject((string Key, Resource Value) resource, ContainerOptions options)
    {
        var project = resource.Value as ProjectResource;

        var details = await containerDetailsService.GetContainerDetails(resource.Key, project, options);
        var success = _containerDetailsCache.TryAdd(resource.Key, details);
        if (!success)
        {
            throw new InvalidOperationException($"Failed to add container details for project {resource.Key} to cache.");
        }

        Console.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Populated container details cache for project [blue]{resource.Key}[/]");
    }

    public static TaskTemplate CreateNomadRawExecTaskTemplate((string Key, Resource? Value) resource, NomadifyState state)
    {
        if (resource.Value is ProjectResource projectResource)
        {
            var source = Url.Combine(state.FileServerUrl, $"{resource.Key}.{state.RawExecSettings.CompressionKind}");

            var result = new TaskTemplate
            {
                ServiceName = resource.Key,
                ServiceNameLiteral = Path.ChangeExtension(Path.GetFileName(projectResource!.Path), ".dll")!,
                Driver = "raw_exec",
                ArtifactSourceUrl = source,
                StartCommand = string.Empty,
                TraefikPrefixName = string.Empty,
                EnvironmentVariables = projectResource.Env!
            };

            return result;
        }

        throw new InvalidOperationException($"The resource {resource.Value.Name} is not a project resource.");
    }

    public static TaskTemplate CreateNomadDockerTaskTemplate((string Key, Resource? Value) resource, NomadifyState state)
    {
        var projectResource = resource.Value as ProjectResource;

        var containerRepositoryPrefix = state.ContainerRepositoryPrefix;
        var prefix = containerRepositoryPrefix is null
            ? ""
            : $"{containerRepositoryPrefix}/";

        var containerRegistry = state.ContainerRegistry ?? null;
        if (containerRegistry is not null && !containerRegistry.EndsWith('/'))
        {
            containerRegistry += '/';
        }

        var result = new TaskTemplate
        {
            ServiceName = resource.Key,
            ServiceNameLiteral = Path.ChangeExtension(Path.GetFileName(projectResource!.Path), ".dll")!,
            Driver = "docker",
            ArtifactSourceUrl = $"{containerRegistry ?? ""}{prefix}{projectResource.Name.ToLower()}:{string.Join(",", state.ContainerImageTags!)}",
            StartCommand = string.Empty,
            TraefikPrefixName = string.Empty,
            EnvironmentVariables = projectResource.Env!
        };

        return result;
    }
}
