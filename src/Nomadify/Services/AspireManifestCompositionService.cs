using System.IO;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomadify.Execution;
using Nomadify.Execution.Exceptions;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Services;

public class AspireManifestCompositionService(IAnsiConsole console, IProcessRunner processRunner)
{
    public async Task<(bool Success, string FullPath)> BuildManifestForProject(string? appHostProject)
    {
        var normalizedPath = appHostProject?.NormalizePath();
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            return (false, string.Empty);
        }

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(DotNetSdkConstants.RunArgument, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkConstants.ProjectArgument, normalizedPath)
            .AppendArgument(DotNetSdkConstants.ArgumentDelimiter, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkConstants.PublisherArgument, NomadifyConstants.ManifestPublisherArgument, quoteValue: false)
            .AppendArgument(DotNetSdkConstants.OutputPathArgument, NomadifyConstants.DefaultManifestFile, quoteValue: false);

        var outputFile = Path.Combine(normalizedPath, NomadifyConstants.DefaultManifestFile);

        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        var newManifestFile = await processRunner.ExecuteCommand(new()
        {
            Command = DotNetSdkConstants.DotNetCommand,
            PreCommandMessage = "Generating Aspire Manifest...",
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = false,
        });

        if (!string.IsNullOrEmpty(newManifestFile.Error))
        {
            console.MarkupLine($"[red]Error: {newManifestFile.Error.EscapeMarkup()}[/]");
            console.MarkupLine("[red]Could not build the manifest for the supplied details. Exiting rather than building default manifest file.[/]");
            ExitCodeException.ExitNow();
        }

        return (newManifestFile.Success, outputFile);
    }
}
