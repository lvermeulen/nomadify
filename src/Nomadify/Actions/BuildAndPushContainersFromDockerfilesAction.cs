using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions.Components;
using Nomadify.Commands.Options;
using Nomadify.Extensions;
using Nomadify.Processors;
using Spectre.Console;

namespace Nomadify.Actions;

public sealed class BuildAndPushContainersFromDockerfilesAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Dockerfiles[/]");

        if (!HasSelectedDockerfileComponents())
        {
            return true;
        }

        var dockerfileProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(NomadifyConstants.Dockerfile) as DockerfileProcessor;

        CacheContainerDetails(dockerfileProcessor);

        if (CurrentState.NoBuild == true)
        {
            Logger.MarkupLine("[bold]Skipping build and push action as requested.[/]");
            return true;
        }

        await PerformBuildAndPushes(dockerfileProcessor);

        return true;
    }

    private void CacheContainerDetails(DockerfileProcessor? dockerfileProcessor)
    {
        if (dockerfileProcessor is null)
        {
            return;
        }

        foreach (var resource in CurrentState.SelectedDockerfileProjectComponents)
        {
            dockerfileProcessor.PopulateContainerImageCacheWithImage(resource, new()
            {
                Registry = CurrentState.ContainerRegistry,
                Prefix = CurrentState.ContainerRepositoryPrefix,
                Tags = CurrentState.ContainerImageTags,
            });
        }
    }

    private async Task PerformBuildAndPushes(DockerfileProcessor? dockerfileProcessor)
    {
        if (dockerfileProcessor is null)
        {
            return;
        }

        foreach (var resource in CurrentState.SelectedDockerfileProjectComponents)
        {
            await dockerfileProcessor.BuildAndPushContainerForDockerfile(resource, new()
            {
                ContainerBuilder = (CurrentState.ContainerBuilder ?? ContainerBuilder.Docker).ToLower(),
                ImageName = resource.Key,
                Registry = CurrentState.ContainerRegistry,
                Prefix = CurrentState.ContainerRepositoryPrefix,
                Tags = CurrentState.ContainerImageTags
            });
        }
    }

    private bool HasSelectedDockerfileComponents()
    {
        if (CurrentState.SelectedDockerfileProjectComponents.Count > 0)
        {
            return true;
        }

        Logger.MarkupLine("[bold]No Dockerfile components selected. Skipping build and publish action.[/]");
        return false;
    }
}
