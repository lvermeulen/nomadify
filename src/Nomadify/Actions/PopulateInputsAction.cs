using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Parameters;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Actions;

public sealed class PopulateInputsAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling inputs[/]");

        var parameterResources = CurrentState.LoadedAspireManifestResources?.Where(x => x.Value is ParameterResource).ToArray()
            .Select(item => (item.Key, item.Value))
            .ToList();
        if (parameterResources?.Count == 0)
        {
            Logger.WriteLine("No inputs need to be processed.");
            return Task.FromResult(true);
        }

        ApplyManualValues(parameterResources);

        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Input values have all been assigned.");

        return Task.FromResult(true);
    }

    private void ApplyManualValues(IEnumerable<(string Key, Resource Value)>? parameterResources)
    {
        if (parameterResources is null)
        {
            return;
        }

        foreach (var component in parameterResources)
        {
            var componentWithInput = component.Value as ParameterResource;
            var manualInputs = componentWithInput?.Inputs?.Where(x => x.Value.Default is null)
                .Select(item => (item.Key, item.Value));

            AssignManualValues(ref manualInputs, componentWithInput);
        }
    }

    private void AssignManualValues(ref IEnumerable<(string Key, ParameterInput Value)>? manualInputs, ParameterResource? parameterResource)
    {
        if (manualInputs is null)
        {
            return;
        }

        foreach (var input in manualInputs)
        {
            HandleSetInput(input, parameterResource);
        }
    }

    private void HandleSetInput((string Key, ParameterInput Value) input, ParameterResource? parameterResource)
    {
        if (parameterResource is null)
        {
            return;
        }

        var firstPrompt = new TextPrompt<string>($"Enter a value for resource [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/]: ").PromptStyle("yellow");
        var secondPrompt = new TextPrompt<string>("Please repeat the value: ").PromptStyle("yellow");

        var firstInput = Logger.Prompt(firstPrompt);
        var secondInput = Logger.Prompt(secondPrompt);

        if (firstInput.Equals(secondInput, StringComparison.Ordinal))
        {
            parameterResource.Value = firstInput;
            Logger.MarkupLine($"Successfully [green]assigned[/] a value for [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/]");
            return;
        }

        Logger.MarkupLine("[red]The values do not match. Please try again.[/]");
        HandleSetInput(input, parameterResource);
    }
}
