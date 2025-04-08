using System;
using System.Linq;
using System.Threading.Tasks;
using Nomad.Abstractions;
using Nomad.Abstractions.Components;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Actions;

public sealed class RemoveJobFromClusterAction(IServiceProvider serviceProvider, INomadExecutionService nomadExecutionService) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Removal from Cluster[/]");

        try
        {
            var lastTaskValue = CurrentState.LoadedAspireManifestResources?.LastOrDefault().Value;
            var serviceName = lastTaskValue?.Name ?? "lastTaskValueName";
            await nomadExecutionService.RemoveJobAsync(serviceName, CurrentState.NomadUrl ?? "http://localhost:4646");
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Job removed from cluster.");

            return true;
        }
        catch (Exception e)
        {
            Logger.MarkupLine("[red](!)[/] Failed to remove job from cluster.");
            Logger.MarkupLine($"[red](!)[/] Error: {e.Message}");
            return false;
        }
    }
}
