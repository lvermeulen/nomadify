using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Nomad.Abstractions;
using Nomad.Abstractions.Components;
using Nomadify.Execution;
using Nomadify.Execution.Exceptions;
using Spectre.Console;

namespace Nomadify.Processors;

public abstract class BaseContainerProcessor(IAnsiConsole console, IProcessRunner processRunner) : BaseResourceProcessor(console)
{
    public IProcessRunner ProcessRunner { get; } = processRunner;

    public async Task CheckIfContainerBuilderIsRunning(string? builder)
    {
        ArgumentException.ThrowIfNullOrEmpty(builder);

        var builderAvailable = IsCommandAvailable(builder);
        if (!builderAvailable.IsAvailable)
        {
            Console.MarkupLine($"[red bold]{builder} is not available or found on your system.[/]");
            ExitCodeException.ExitNow();
        }

        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("info", string.Empty, quoteValue: false)
            .AppendArgument("--format", "json", quoteValue: false);

        var builderCheckResult = await ProcessRunner.ExecuteCommand(new()
        {
            Command = builderAvailable.FullPath,
            ArgumentsBuilder = argumentsBuilder,
        });

        ValidateBuilderOutput(builderCheckResult);
    }

    private void ValidateBuilderOutput(ProcessRunResult builderCheckResult)
    {
        if (builderCheckResult.Success)
        {
            return;
        }

        var builderInfo = builderCheckResult.Output.TryParseAsJsonDocument();
        if (builderInfo == null || !builderInfo.RootElement.TryGetProperty("ServerErrors", out var errorProperty))
        {
            return;
        }

        if (errorProperty.ValueKind == JsonValueKind.Array && errorProperty.GetArrayLength() == 0)
        {
            return;
        }

        var messages = string.Join(Environment.NewLine, errorProperty.EnumerateArray());
        Console.MarkupLine("[red][bold]The daemon server reported errors:[/][/]");
        Console.MarkupLine($"[red]{messages}[/]");
        ExitCodeException.ExitNow();
    }

    public async Task HandleBuildErrors(string command, ArgumentsBuilder argumentsBuilder, bool nonInteractive, string errors)
    {
        if (errors.Contains(DotNetSdkConstants.DuplicateFileOutputError, StringComparison.OrdinalIgnoreCase))
        {
            await HandleDuplicateFilesInOutput(argumentsBuilder, nonInteractive);
            return;
        }

        if (errors.Contains(DotNetSdkConstants.UnknownContainerRegistryAddress, StringComparison.OrdinalIgnoreCase))
        {
            Console.MarkupLine($"[red bold]{DotNetSdkConstants.UnknownContainerRegistryAddress}: Unknown container registry address, or container registry address not accessible.[/]");
            ExitCodeException.ExitNow(1013);
        }

        ExitCodeException.ExitNow();
    }

    private async Task HandleDuplicateFilesInOutput(ArgumentsBuilder argumentsBuilder, bool nonInteractive = false)
    {
        var shouldRetry = AskIfShouldRetryHandlingDuplicateFiles(nonInteractive);
        if (shouldRetry)
        {
            argumentsBuilder.AppendArgument(DotNetSdkConstants.ErrorOnDuplicatePublishOutputFilesArgument, "false");

            await ProcessRunner.ExecuteCommand(new()
            {
                Command = DotNetSdkConstants.DotNetCommand,
                ArgumentsBuilder = argumentsBuilder,
                NonInteractive = nonInteractive,
                OnFailed = HandleBuildErrors,
                ShowOutput = true
            });
            return;
        }

        ExitCodeException.ExitNow();
    }

    private bool AskIfShouldRetryHandlingDuplicateFiles(bool nonInteractive)
    {
        if (nonInteractive)
        {
            return true;
        }

        return Console.Confirm("[red bold]Implicitly, dotnet publish does not allow duplicate filenames to be output to the artefact directory at build time.Would you like to retry the build explicitly allowing them?[/]");
    }

    public static CommandAvailableResult IsCommandAvailable(string commandName)
    {
        try
        {
            var commandPath = FindFullPathFromPath(commandName);
            if (string.IsNullOrEmpty(commandPath) || commandName.Equals(commandPath, StringComparison.Ordinal))
            {
                return CommandAvailableResult.NotAvailable;
            }

            return string.IsNullOrEmpty(commandPath)
                ? CommandAvailableResult.NotAvailable
                : CommandAvailableResult.Available(commandPath);
        }
        catch (Exception)
        {
            return CommandAvailableResult.NotAvailable;
        }
    }

    private static string FindFullPathFromPath(string command)
    {
        var path = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator);
        foreach (var directory in path)
        {
            var fullPath = Path.Combine(directory, command + (OperatingSystem.IsWindows() ? ".exe" : ""));
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        return command;
    }
}
