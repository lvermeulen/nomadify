using System;
using System.Threading.Tasks;
using Nomadify.Actions;
using Nomadify.Commands.Base;

namespace Nomadify.Commands.Generate;

public sealed class GenerateCommandHandler(IServiceProvider serviceProvider)
    : BaseCommandOptionsHandler<GenerateOptions>(serviceProvider)
{
    //dotnet run --project AspireApp.AppHost\AspireApp.AppHost.csproj `
    // -- `
    // --publisher manifest `
    // --output-path ../aspire-manifest.json

    public override Task<int> HandleAsync(GenerateOptions options) => ActionExecutor
        .QueueAction(nameof(LoadConfigurationAction))
        .QueueAction(nameof(GenerateAspireManifestAction))
        .QueueAction(nameof(LoadAspireManifestAction))
        .QueueAction(nameof(IncludeAspireDashboardAction))
        .QueueAction(nameof(PopulateInputsAction))
        .QueueAction(nameof(SubstituteValuesAspireManifestAction))
        .QueueAction(nameof(ApplyDaprAnnotationsAction))
        .QueueAction(nameof(BuildAndPublishRawExecAction))
        .QueueAction(nameof(PopulateContainerDetailsForProjectsAction))
        .QueueAction(nameof(BuildAndPushContainersFromProjectsAction))
        .QueueAction(nameof(BuildAndPushContainersFromDockerfilesAction))
        .QueueAction(nameof(ApplyJobToClusterAction))
        .ExecuteCommandsAsync();
}
