using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions.Components;
using Nomadify.Extensions;
using Nomadify.Processors;
using Spectre.Console;

namespace Nomadify.Actions;

public sealed class PopulateContainerDetailsForProjectsAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Gathering Information about deployables[/]");

        if (NoSelectedDockerProjectComponents())
        {
            return true;
        }

        await HandleProjects();

        Logger.MarkupLine("[bold]Gathering Tasks Completed - Cache Populated.[/]");

        return true;
    }

    private async Task HandleProjects()
    {
        if (CurrentState.ContainerRegistry is null)
        {
            return;
        }

        var projectProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(NomadifyConstants.Project) as ProjectProcessor;
        if (projectProcessor is null)
        {
            throw new InvalidOperationException("Project processor could not be resolved.");
        }

        Logger.MarkupLine("[bold]Gathering container details for each project in selected components[/]");

        foreach (var resource in CurrentState.SelectedDockerProjectComponents.Where(x => x.Value is not null))
        {
            await projectProcessor.PopulateContainerDetailsCacheForProject(resource!, new()
            {
                Registry = CurrentState.ContainerRegistry,
                Prefix = CurrentState.ContainerRepositoryPrefix,
                Tags = CurrentState.ContainerImageTags,
            });
        }
    }

    private bool NoSelectedDockerProjectComponents()
    {
        if (CurrentState.SelectedDockerProjectComponents.Count == 0)
        {
            Logger.MarkupLine("[bold]No project components selected. Skipping execution of container detail gathering for them.[/]");
            return true;
        }

        return false;
    }
}