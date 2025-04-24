using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions.Components;
using Nomadify.Execution;
using Nomadify.Execution.Exceptions;
using Nomadify.Extensions;
using Nomadify.Processors;
using Spectre.Console;

namespace Nomadify.Actions;

public class BuildAndPublishRawExecAction(IServiceProvider serviceProvider, IAnsiConsole console) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Gathering Information about deployables[/]");

        if (CurrentState.NoBuild == true)
        {
            Logger.MarkupLine("[bold]Skipping build and publish action as requested.[/]");
            return true;
        }

        if (NoSelectedRawExecProjectComponents())
        {
            Logger.MarkupLine("[bold]No project components selected. Skipping execution of raw exec action.[/]");
            return true;
        }

        await HandleProjects();
        await HandlePostBuildActions();

        Logger.MarkupLine("[bold]Gathering Tasks Completed - Cache Populated.[/]");

        return true;
    }

    private async Task HandleProjects()
    {
        if (CurrentState.FileServerUrl is null)
        {
            return;
        }

        var projectProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(NomadifyConstants.Project) as ProjectProcessor;
        if (projectProcessor is null)
        {
            throw new InvalidOperationException("Project processor could not be resolved.");
        }

        if (CurrentState.HasDapr())
        {
            Logger.MarkupLine("[bold]Generating files for Dapr sidecars[/]");
            foreach (var resourceValue in CurrentState.DaprComponents.Where(x => x.Value is not null).Select(x => x.Value))
            {
                await DaprComponentProcessor.CreateDaprdTemplateAsync(resourceValue, CurrentState.OutputPath!);
                Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Generated {CurrentState.OutputPath}/dapr/{resourceValue!.Name}.yaml");
            }
        }

        Logger.MarkupLine("[bold]Building and publishing raw exec projects for selected components[/]");
        foreach (var resource in CurrentState.SelectedRawExecProjectComponents)
        {
            var publishFolder = await projectProcessor.PublishProject(resource!, CurrentState);
            await ProjectProcessor.HandleCompression(resource!, CurrentState, publishFolder);

            console.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Building and publishing project [blue]{resource.Key}[/]");
        }
    }

    public Task HandlePostBuildActions()
    {
        var postBuildActions = CurrentState.RawExecSettings.PostBuildActions;
        if (postBuildActions is null)
        {
            return Task.CompletedTask;
        }

        if (CurrentState.OutputPath is null)
        {
            return Task.CompletedTask;
        }

        var processRunner = Services.GetRequiredService<IProcessRunner>();

        WithCurrentDirectory(CurrentState.OutputPath, async () =>
        {
            var result = new ProcessRunResult(false, string.Empty, string.Empty, 0);
            var parameter = string.Empty;
            try
            {
                foreach (var parameters in postBuildActions
                             .Where(x => x.Kind.Equals("shell", StringComparison.OrdinalIgnoreCase))
                             .Select(x => x.Parameters))
                {
                    parameter = parameters;
                    result = await processRunner.ExecuteCommand(new()
                    {
                        Command = parameters,
                        PreCommandMessage = $"Executing post-build action '{parameters}'...",
                        ArgumentsBuilder = new(),
                        ShowOutput = true,
                    });
                }
            }
            catch
            {
                // ignore exceptions
            }

            if (result.ExitCode != 0 || !string.IsNullOrEmpty(result.Error))
            {
                Logger.MarkupLine($"[red]Error: {result.Error}[/]");
                Logger.MarkupLine($"[red]Could not execute post-build action '{parameter}'. Exiting...[/]");
                ExitCodeException.ExitNow();
            }
        });

        return Task.CompletedTask;
    }

    private static void WithCurrentDirectory(string newDirectory, Action? action)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(newDirectory);
            action?.Invoke();
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDirectory);
        }
    }

    private bool NoSelectedRawExecProjectComponents()
    {
        if (CurrentState.AspireRawExecComponentsToProcess.Count == 0)
        {
            Logger.MarkupLine("[bold]No project components selected. Skipping execution of raw exec action.[/]");
            return true;
        }

        return false;
    }
}