using System;
using System.IO;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomadify.Execution;
using Nomadify.Execution.Exceptions;
using Nomadify.Extensions;
using Nomadify.Interfaces;
using Spectre.Console;

namespace Nomadify.Services;

public sealed class ProjectPropertyService(IProcessRunner processRunner, IAnsiConsole console) : IProjectPropertyService
{
    public async Task<string?> GetProjectPropertiesAsync(string? projectPath, params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(projectPath);

        var fullPath = Path.GetFullPath(projectPath);
        var fullProjectPath = fullPath.NormalizePath();
        var propertyValues = await ExecuteDotnetMsBuildGetPropertyCommand(fullProjectPath, propertyNames);

        return propertyValues ?? null;
    }

    private async Task<string?> ExecuteDotnetMsBuildGetPropertyCommand(string projectPath, params string[] propertyNames)
    {
        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(DotNetSdkConstants.MsBuildArgument, string.Empty, quoteValue: false)
            .AppendArgument($"\"{projectPath}\"", string.Empty, quoteValue: false);

        foreach (var propertyName in propertyNames)
        {
            argumentsBuilder.AppendArgument(DotNetSdkConstants.GetPropertyArgument, propertyName, true);
        }

        var result = await processRunner.ExecuteCommand(new()
        {
            Command = DotNetSdkConstants.DotNetCommand,
            ArgumentsBuilder = argumentsBuilder,
            PropertyKeySeparator = ':',
        });

        if (!result.Success)
        {
            console.MarkupLine($"[red]Failed to get project properties for '{projectPath}'.[/]");
            ExitCodeException.ExitNow(result.ExitCode);
        }

        return result.Success ? result.Output : null;
    }
}
