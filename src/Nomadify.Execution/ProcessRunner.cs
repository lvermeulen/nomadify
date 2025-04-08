using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using FluentResults;
using Nomadify.Execution.Exceptions;
using Spectre.Console;

namespace Nomadify.Execution;

public class ProcessRunner(IAnsiConsole console) : IProcessRunner
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();
    private bool _showOutput;

    public async Task<ProcessRunResult> ExecuteCommand(ProcessRunOptions options)
    {
        _showOutput = options.ShowOutput;
        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var arguments = options.ArgumentsBuilder!.RenderArguments(propertyKeySeparator: options.PropertyKeySeparator);

        if (options.ShowOutput)
        {
            console.WriteLine();
            console.MarkupLine(string.IsNullOrEmpty(options.PreCommandMessage) ? $"[cyan]Executing: {options.Command} {arguments}[/]" : options.PreCommandMessage);
        }

        var executionDirectory = string.IsNullOrEmpty(options.WorkingDirectory)
            ? Directory.GetCurrentDirectory()
            : options.WorkingDirectory;

        var result = await Cli.Wrap(options.Command!)
            .WithWorkingDirectory(executionDirectory)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(WriteInfo))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(WriteError))
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0)
        {
            await HandleExitCode(options.Command!, options.ArgumentsBuilder, options.NonInteractive, options.OnFailed, options.FailureCommandMessage, result, options.ExitWithExitCode);
        }

        if (result.ExitCode == 0 && !string.IsNullOrEmpty(options.SuccessCommandMessage))
        {
            console.MarkupLine(options.SuccessCommandMessage);
        }

        return new(result.ExitCode == 0, result.StandardOutput, result.StandardError, result.ExitCode);
    }

    public async Task<bool> ExecuteCommandWithEnvironmentNoOutput(string command, ArgumentsBuilder argumentsBuilder, IReadOnlyDictionary<string, string?> environmentVariables)
    {
        var output = new StringBuilder();
        var errors = new StringBuilder();

        try
        {
            var arguments = argumentsBuilder.RenderArguments();

            var executionCommand = Cli.Wrap(command)
                .WithArguments(arguments)
                .WithEnvironmentVariables(environmentVariables);

            var commandResult = await executionCommand
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(output))
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(errors))
                .ExecuteAsync();

            return commandResult.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IResult<string> IsCommandAvailable(string commandName)
    {
        var errorMessage = $"{commandName} was not found on the path.";

        try
        {
            var commandPath = FindFullPathFromPath(commandName);
            if (string.IsNullOrEmpty(commandPath) || commandName.Equals(commandPath, StringComparison.Ordinal))
            {
                return Result.Fail<string>(errorMessage);
            }

            return string.IsNullOrEmpty(commandPath)
                ? Result.Fail<string>(errorMessage)
                : Result.Ok(commandPath);
        }
        catch (Exception ex)
        {
            return Result.Fail<string>(ex.Message);
        }
    }

    private Task HandleExitCode(string command,
        ArgumentsBuilder argumentsBuilder,
        bool nonInteractive,
        Func<string, ArgumentsBuilder, bool, string, Task>? onFailed,
        string? failureCommandMessage, CommandResult result,
        bool exitWithExitCode)
    {
        if (onFailed != null)
        {
            return onFailed.Invoke(
                command,
                argumentsBuilder,
                nonInteractive,
                _stdErrBuffer.Append(_stdOutBuffer).ToString());
        }

        if (!string.IsNullOrEmpty(failureCommandMessage))
        {
            console.MarkupLine(failureCommandMessage);
        }

        if (exitWithExitCode)
        {
            ExitCodeException.ExitNow(result.ExitCode);
        }

        return Task.CompletedTask;
    }

    private void WriteError(string message)
    {
        if (_showOutput)
        {
            console.MarkupLine("[red]{0}[/]", message.EscapeMarkup());
        }

        _stdErrBuffer.AppendLine(message);
    }

    private void WriteInfo(string message)
    {
        if (_showOutput)
        {
            console.WriteLine(message);
        }

        _stdOutBuffer.AppendLine(message);
    }

    private static string FindFullPathFromPath(string command)
    {
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator))
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
