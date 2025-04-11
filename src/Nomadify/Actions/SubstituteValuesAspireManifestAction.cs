using System;
using System.Threading.Tasks;
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
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] {numberOfBindings} bindings have been inserted.");
        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] {numberOfSubstitutions} substitutions have been applied.");

        return Task.FromResult(true);
    }
}
