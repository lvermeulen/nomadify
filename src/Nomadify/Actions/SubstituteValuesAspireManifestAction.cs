using System;
using System.Threading.Tasks;
using Humanizer;
using Nomad.Abstractions.Components;
using Nomadify.Extensions;
using Nomadify.Processors;
using Spectre.Console;

namespace Nomadify.Actions;

public class SubstituteValuesAspireManifestAction(IServiceProvider serviceProvider, ResourceExpressionProcessor transformer) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle value and parameter substitution[/]");

        var (numberOfBindings, numberOfSubstitutions) = transformer.ProcessEvaluations(CurrentState.LoadedAspireManifestResources);
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] {numberOfBindings} {"binding".Pluralize()} inserted.");
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] {numberOfSubstitutions} {"substitution".Pluralize()} applied.");

        return Task.FromResult(true);
    }
}
