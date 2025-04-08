using System;
using System.Threading.Tasks;
using Nomadify.Commands.Options;
using Nomadify.Extensions;
using Nomadify.Interfaces;
using Spectre.Console;

namespace Nomadify.Actions;

public class LoadConfigurationAction(INomadifyConfigurationService configurationService, IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (PreviousStateWasRestored())
        {
            return Task.FromResult(true);
        }

        Logger.WriteRuler("[purple]Handling Configuration[/]");

        var nomadifySettings = configurationService.LoadConfigurationFile(CurrentState.ProjectPath);
        if (nomadifySettings is null)
        {
            return Task.FromResult(true);
        }

        if (string.IsNullOrEmpty(CurrentState.RawExecSettings.CompressionKind))
        {
            CurrentState.RawExecSettings.CompressionKind = CompressionKind.Zip;
        }

        if (string.IsNullOrEmpty(CurrentState.NomadUrl))
        {
            CurrentState.NomadUrl = "http://localhost:4646";
        }

        if (string.IsNullOrEmpty(CurrentState.OutputPath))
        {
            CurrentState.OutputPath = "./";
        }

        if (string.IsNullOrEmpty(CurrentState.ContainerRegistry))
        {
            CurrentState.ContainerRegistry = nomadifySettings.ContainerSettings?.Registry ?? null;
        }

        if (string.IsNullOrEmpty(CurrentState.ContainerBuilder))
        {
            CurrentState.ContainerBuilder = nomadifySettings.ContainerSettings?.Builder?.ToLower() ?? null;
        }

        if (string.IsNullOrEmpty(CurrentState.ContainerRepositoryPrefix))
        {
            CurrentState.ContainerRepositoryPrefix = nomadifySettings.ContainerSettings?.RepositoryPrefix ?? null;
        }

        CurrentState.ContainerImageTags ??= nomadifySettings.ContainerSettings?.Tags ?? null;

        Logger.MarkupLine($"[bold]Successfully loaded existing Nomadify bootstrap settings from [blue]'{CurrentState.ProjectPath}'[/].[/]");

        return Task.FromResult(true);
    }
}
