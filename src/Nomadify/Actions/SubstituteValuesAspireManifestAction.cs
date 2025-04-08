using System;
using System.Threading.Tasks;
using Nomadify.Extensions;
using Nomadify.Processors;

namespace Nomadify.Actions;

public class SubstituteValuesAspireManifestAction(IServiceProvider serviceProvider, IResourceExpressionProcessor transformer) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle value and parameter substitution[/]");

        transformer.ProcessEvaluations(CurrentState.LoadedAspireManifestResources);

        return Task.FromResult(true);
    }
}