using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nomadify.Extensions;
using Nomadify.Interfaces;
using Spectre.Console;

namespace Nomadify.Actions;

public class LoadAspireManifestAction(IManifestFileParserService manifestFileParserService, IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Selecting Required Components[/]");

        CurrentState.LoadedAspireManifestResources = manifestFileParserService.LoadAndParseAspireManifest(CurrentState.AspireManifest);

        SelectManifestItemsToProcess();

        return Task.FromResult(true);
    }

    private void SelectManifestItemsToProcess()
    {
        Logger.MarkupLine("[blue]Processing components in the loaded manifest file.[/]");
        CurrentState.AspireRawExecComponentsToProcess = CurrentState.LoadedAspireManifestResources!.Keys.ToList();

        if (CurrentState.NoBuild == true)
        {
            Logger.WriteRuler("No-build option is active; continuing without selection of components.");
            return;
        }

        var daprComponents = CurrentState.DaprComponents.Select(x => x.Key).ToList();
        if (daprComponents.Count > 0)
        {
            Logger.MarkupLine("Dapr sidecars are present.");
        }

        var rawExecComponents = SelectRawExecItems(daprComponents).Union(CurrentState.DaprProjectComponents.Select(x => x.Key)).ToList();
        if (rawExecComponents.Count == CurrentState.LoadedAspireManifestResources.Count)
        {
            Logger.MarkupLine("Raw exec components are all components in the loaded file. Skipping selection of docker and dockerfile components...");
            return;
        }

        var dockerComponents = SelectDockerItems(rawExecComponents);
        if (rawExecComponents.Count + dockerComponents.Count == CurrentState.LoadedAspireManifestResources.Count)
        {
            Logger.MarkupLine("Raw exec and docker components are all components in the loaded file. Skipping selection of dockerfile components...");
            return;
        }

        _ = SelectDockerfileItems(rawExecComponents.Union(dockerComponents).ToList());
    }

    private IList<string> SelectRawExecItems(IList<string> alreadySelectedComponents)
    {
        var rawExecComponentsToProcess = Logger.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]raw exec components[/] to process from the loaded file")
                .PageSize(10)
                .NotRequired()
                .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a component, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("All Components", CurrentState.LoadedAspireManifestResources!.Keys.Where(x => alreadySelectedComponents.All(s => !s.Equals(x, StringComparison.OrdinalIgnoreCase))).ToList()));

        CurrentState.AspireRawExecComponentsToProcess = rawExecComponentsToProcess;
        return rawExecComponentsToProcess;
    }

    private IList<string> SelectDockerItems(IList<string> alreadySelectedComponents)
    {
        var dockerComponentsToProcess = Logger.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]docker components[/] to process from the loaded file")
                .PageSize(10)
                .NotRequired()
                .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a component, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("All Components", CurrentState.LoadedAspireManifestResources!.Keys.Where(x => alreadySelectedComponents.All(s => !s.Equals(x, StringComparison.OrdinalIgnoreCase))).ToList()));

        CurrentState.AspireDockerComponentsToProcess = dockerComponentsToProcess;
        return dockerComponentsToProcess;
    }

    private IList<string> SelectDockerfileItems(IList<string> alreadySelectedComponents)
    {
        var dockerfileComponentsToProcess = Logger.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]dockerfile components[/] to process from the loaded file")
                .PageSize(10)
                .NotRequired()
                .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a component, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("All Components", CurrentState.LoadedAspireManifestResources!.Keys.Where(x => alreadySelectedComponents.All(s => !s.Equals(x, StringComparison.OrdinalIgnoreCase))).ToList()));

        CurrentState.AspireDockerfileComponentsToProcess = dockerfileComponentsToProcess;
        return dockerfileComponentsToProcess;
    }
}
