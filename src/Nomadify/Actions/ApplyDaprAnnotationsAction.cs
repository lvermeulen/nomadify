using System;
using System.Linq;
using System.Threading.Tasks;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Container;
using Nomad.Abstractions.Components.V0.Dapr;
using Nomad.Abstractions.Interfaces;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Actions;

public class ApplyDaprAnnotationsAction(IServiceProvider serviceProvider, IAnsiConsole console) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Dapr Components[/]");

        var daprComponents = CurrentState.DaprRawExecProjectComponents.ToList();
        if (daprComponents.Count == 0)
        {
            console.WriteLine("No components selected, skipping Dapr annotations.");
            return Task.FromResult(true);
        }

        foreach (var daprComponent in daprComponents)
        {
            if (daprComponent.Value is DaprResource daprResource)
            {
                Logger.WriteLine($"Applying Dapr annotations for {daprResource.Name}");

                var serviceForSidecar = CurrentState.LoadedAspireManifestResources?[daprResource.Metadata.Application];
                ApplyDaprAnnotationsToTargetService(serviceForSidecar, daprResource);
            }
        }

        return Task.FromResult(true);
    }

    private static void ApplyDaprAnnotationsToTargetService(Resource serviceForSidecar, DaprResource resource)
    {
        if (serviceForSidecar is not IResourceWithAnnotations service)
        {
            return;
        }

        service.Annotations ??= [];

        service.Annotations.Add("dapr.io/enabled", "'true'");
        service.Annotations.Add("dapr.io/config", "tracing");
        service.Annotations.Add("dapr.io/app-id", resource.Metadata.AppId);
        service.Annotations.Add("dapr.io/enable-api-logging", "'true'");

        HandleContainerPort(serviceForSidecar);
    }

    private static void HandleContainerPort(Resource serviceForSidecar)
    {
        if (serviceForSidecar is not ContainerResource container)
        {
            return;
        }

        if (!container.Bindings.TryGetValue("tcp", out var binding))
        {
            return;
        }

        container.Annotations.Add("dapr.io/app-port", binding.TargetPort.ToString());
    }
}
