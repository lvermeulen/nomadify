using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions.Components;
using Nomadify.Commands.Options;
using Nomadify.Extensions;
using Nomadify.Processors;
using Spectre.Console;

namespace Nomadify.Actions;

public sealed class BuildAndPushContainersFromProjectsAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Projects[/]");

        if (CurrentState.NoBuild == true)
        {
            Logger.MarkupLine("[bold]Skipping build and push action as requested.[/]");
            return true;
        }

        if (NoSelectedDockerProjectComponents() && CurrentState.IncludeDashboard == false)
        {
            return true;
        }

        var projectProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(NomadifyConstants.Project) as ProjectProcessor;
        if (projectProcessor is null)
        {
            return false;
        }

        Logger.MarkupLine("[bold]Building all project resources, and pushing containers[/]");

        foreach (var resource in CurrentState.SelectedDockerProjectComponents.Where(x => x.Value is not null))
        {
            await projectProcessor.BuildAndPushProjectContainer(resource, new()
            {
                ContainerBuilder = (CurrentState.ContainerBuilder ?? ContainerBuilder.Docker).ToLower(),
                Prefix = CurrentState.ContainerRepositoryPrefix
            }, CurrentState.RuntimeIdentifier);
        }

        if (CurrentState.IncludeDashboard == true)
        {
            await projectProcessor.BuildAndPushDashboard(new()
            {
                ContainerBuilder = (CurrentState.ContainerBuilder ?? ContainerBuilder.Docker).ToLower(),
                Prefix = CurrentState.ContainerRepositoryPrefix
            }, CurrentState.RuntimeIdentifier);
        }

        Logger.MarkupLine("[bold]Building and push completed for all selected project components.[/]");

        return true;
    }

    private bool NoSelectedDockerProjectComponents()
    {
        if (CurrentState.AspireDockerComponentsToProcess.Count == 0)
        {
            Logger.MarkupLine("[bold]No docker project components selected. Skipping build and publish action.[/]");
            return true;
        }

        return false;
    }
}
