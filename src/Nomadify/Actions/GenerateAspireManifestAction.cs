using System;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomadify.Execution.Exceptions;
using Nomadify.Extensions;
using Nomadify.Services;
using Spectre.Console;

namespace Nomadify.Actions;

public sealed class GenerateAspireManifestAction(AspireManifestCompositionService manifestCompositionService, IServiceProvider serviceProvider)
    : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Aspire Manifest[/]");

        if (!string.IsNullOrEmpty(CurrentState.AspireManifest))
        {
            Logger.MarkupLine($"[bold]Aspire Manifest supplied at path: [blue]{CurrentState.AspireManifest}[/].[/]");
            Logger.MarkupLine("[bold]Skipping Aspire Manifest generation.[/]");
            return true;
        }

        Logger.MarkupLine("[bold]Generating Aspire Manifest for supplied App Host[/]");

        var (success, fullPath) = await manifestCompositionService.BuildManifestForProject(CurrentState.ProjectPath);
        if (success)
        {
            CurrentState.AspireManifest = fullPath;

            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Created Aspire Manifest at path: [blue]{CurrentState.AspireManifest}[/]");

            return true;
        }

        Logger.MarkupLine($"[red]Failed to generate Aspire Manifest at: {CurrentState.AspireManifest}[/]");
        ExitCodeException.ExitNow();
        return false;
    }
}
