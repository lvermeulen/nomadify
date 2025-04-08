using System;
using System.Linq;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomadify.Commands.Options;
using Nomadify.Extensions;
using Nomadify.Interfaces;
using Nomadify.Models;
using Spectre.Console;

namespace Nomadify.Actions;

public class InitializeConfigurationAction(INomadifyConfigurationService configurationService, IServiceProvider serviceProvider)
    : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Initialization Defaults[/]");

        var nomadifySettings = PerformConfigurationBootstrapping();

        if (CurrentState.ProjectPath is not null)
        {
            configurationService.SaveConfigurationFile(nomadifySettings, CurrentState.ProjectPath);
        }

        return Task.FromResult(true);
    }

    private NomadifySettings PerformConfigurationBootstrapping()
    {
        var nomadifySettings = new NomadifySettings();

        HandleProjectPath(nomadifySettings);
        HandleNomadUrl(nomadifySettings);
        HandleRawExec(nomadifySettings);
        HandleCompressionKind(nomadifySettings);
        HandleContainerBuilder(nomadifySettings);
        HandleContainerRegistry(nomadifySettings);
        HandleContainerRepositoryPrefix(nomadifySettings);
        HandleContainerTag(nomadifySettings);

        return nomadifySettings;
    }

    private void HandleProjectPath(NomadifySettings nomadifySettings)
    {
        if (!string.IsNullOrEmpty(CurrentState.ProjectPath))
        {
            nomadifySettings.ProjectPath = CurrentState.ProjectPath;
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'project path'[/] to [blue]'{nomadifySettings.ProjectPath}'[/].");
        }
    }

    private void HandleNomadUrl(NomadifySettings nomadifySettings)
    {
        if (!string.IsNullOrEmpty(CurrentState.NomadUrl))
        {
            nomadifySettings.NomadUrl = CurrentState.NomadUrl;
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Nomad URL'[/] to [blue]'{nomadifySettings.NomadUrl}'[/].");
        }
    }

    private void HandleRawExec(NomadifySettings nomadifySettings)
    {
        nomadifySettings.RawExecSettings = CurrentState.RawExecSettings;
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Raw Exec Settings'.");
    }

    private void HandleCompressionKind(NomadifySettings nomadifySettings)
    {
        if (CurrentState.RawExecSettings is not null)
        {
            CurrentState.RawExecSettings.CompressionKind = CurrentState.RawExecSettings.CompressionKind?.ToLower();
            nomadifySettings.RawExecSettings!.CompressionKind = CurrentState.RawExecSettings.CompressionKind;
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Compression Kind'[/] to [blue]'{nomadifySettings.RawExecSettings.CompressionKind}'[/].");
            return;
        }

        Logger.MarkupLine("Nomadify supports [blue]Zip[/] and [blue]Tar[/] compression kinds.");
        var shouldSetBuilder = Logger.Confirm("Would you like to set a fall-back value for the container builder?", false);
        if (!shouldSetBuilder)
        {
            return;
        }

        var compressionKind = Logger.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the Compression Kind to use...")
                .HighlightStyle("blue")
                .PageSize(3)
                .AddChoices(ContainerBuilder.List.Select(x => x.Value).ToArray()));

        if (nomadifySettings.RawExecSettings is not null)
        {
            nomadifySettings.RawExecSettings.CompressionKind = compressionKind;
            Logger.MarkupLine(
                $"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Compression Kind'[/] to [blue]'{nomadifySettings.RawExecSettings.CompressionKind}'[/].");
        }
    }

    private void HandleContainerBuilder(NomadifySettings nomadifySettings)
    {
        if (!string.IsNullOrEmpty(CurrentState.ContainerBuilder) && nomadifySettings.ContainerSettings is not null)
        {
            CurrentState.ContainerBuilder = CurrentState.ContainerBuilder.ToLower();
            nomadifySettings.ContainerSettings.Builder = CurrentState.ContainerBuilder;
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container builder'[/] to [blue]'{nomadifySettings.ContainerSettings.Builder}'[/].");
            return;
        }

        Logger.MarkupLine("Nomadify supports [blue]Docker[/] and [blue]Podman[/] as container builders.");
        var shouldSetBuilder = Logger.Confirm("Would you like to set a fall-back value for the container builder?", false);

        if (!shouldSetBuilder)
        {
            return;
        }

        var builder = Logger.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the Container Builder to use...")
                .HighlightStyle("blue")
                .PageSize(3)
                .AddChoices(ContainerBuilder.List.Select(x => x.Value).ToArray()));

        if (nomadifySettings.ContainerSettings is not null)
        {
            nomadifySettings.ContainerSettings.Builder = builder;
            Logger.MarkupLine(
                $"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container Builder'[/] to [blue]'{nomadifySettings.ContainerSettings.Builder}'[/].");
        }
    }

    private void HandleContainerRegistry(NomadifySettings nomadifySettings)
    {
        if (!string.IsNullOrEmpty(CurrentState.ContainerRegistry))
        {
            nomadifySettings.ContainerSettings!.Registry = CurrentState.ContainerRegistry;
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container fallback registry'[/] to [blue]'{nomadifySettings.ContainerSettings.Registry}'[/].");
            return;
        }

        Logger.MarkupLine("Nomadify supports setting a fall-back value for projects that have not yet set a [blue]'ContainerRegistry'[/] in their csproj file.");
        var shouldSetContainerRegistry = Logger.Confirm("Would you like to set a fall-back value for the container registry?", false);

        if (!shouldSetContainerRegistry)
        {
            return;
        }

        var containerRegistry = Logger.Prompt(new TextPrompt<string>("Please enter the container registry to use as a fall-back value:").PromptStyle("blue"));
        nomadifySettings.ContainerSettings!.Registry = containerRegistry;
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container fallback registry'[/] to [blue]'{nomadifySettings.ContainerSettings.Registry}'[/].");
    }

    private void HandleContainerRepositoryPrefix(NomadifySettings nomadifySettings)
    {
        if (!string.IsNullOrEmpty(CurrentState.ContainerRepositoryPrefix))
        {
            nomadifySettings.ContainerSettings!.RepositoryPrefix = CurrentState.ContainerRepositoryPrefix;
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container repository prefix'[/] to [blue]'{nomadifySettings.ContainerSettings.RepositoryPrefix}'[/].");
            return;
        }

        Logger.MarkupLine("Nomadify supports setting a repository prefix for all for projects.");
        var shouldSetContainerRepositoryPrefix = Logger.Confirm("Would you like to set this value?", false);

        if (!shouldSetContainerRepositoryPrefix)
        {
            return;
        }

        var containerRepositoryPrefix = Logger.Prompt(new TextPrompt<string>("Please enter the container repository prefix to use as a fall-back value:").PromptStyle("blue"));
        nomadifySettings.ContainerSettings!.RepositoryPrefix = containerRepositoryPrefix;
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container repository prefix'[/] to [blue]'{nomadifySettings.ContainerSettings.RepositoryPrefix}'[/].");
    }

    private void HandleContainerTag(NomadifySettings nomadifySettings)
    {
        if (CurrentState.ContainerImageTags?.Count > 0)
        {
            nomadifySettings.ContainerSettings!.Tags = CurrentState.ContainerImageTags;
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container fallback tag'[/] to [blue]'{string.Join(';', nomadifySettings.ContainerSettings.Tags)}'[/].");
            return;
        }

        Logger.MarkupLine("Nomadify supports setting a fall-back value for projects that have not yet set a [blue]'ContainerTag'[/] in their csproj file.");
        var shouldSetContainerTag = Logger.Confirm("Would you like to set a fall-back value for the container tag?", false);

        if (!shouldSetContainerTag)
        {
            return;
        }

        var containerTag = Logger.Prompt(new TextPrompt<string>("Please enter the container tags to use as a fall-back value, you can enter multiple values split via semi-colon ';' :").PromptStyle("blue"));
        nomadifySettings.ContainerSettings!.Tags = containerTag.Split(';').ToList();
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Set [blue]'Container fallback tag'[/] to [blue]'{nomadifySettings.ContainerSettings.Tags}'[/].");
    }
}
