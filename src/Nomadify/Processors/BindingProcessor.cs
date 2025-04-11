using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;

namespace Nomadify.Processors;

public static class BindingProcessor
{
    private const int DefaultServicePort = 10000;
    private static int s_servicePort = DefaultServicePort;

    public static string HandleBindingReplacement(JsonNode? rootNode, IReadOnlyList<string> pathParts, string input, string jsonPath)
    {
        var resourceName = pathParts[0];
        var bindingName = pathParts[2];
        var bindingProperty = pathParts[3];

        var replacement = ParseBinding(resourceName, bindingName, bindingProperty, rootNode);

        return input.Replace($"{{{jsonPath}}}", replacement, StringComparison.OrdinalIgnoreCase);
    }

    private static string? ParseBinding(string resourceName, string bindingName, string bindingProperty, JsonNode? rootNode)
    {
        var bindingEntry = rootNode![resourceName]![NomadifyConstants.Bindings]![bindingName].Deserialize<Binding>();

        return bindingProperty switch
        {
            NomadifyConstants.Host => resourceName, // return the name of the resource for 'host'
            NomadifyConstants.Port => bindingEntry!.Port.GetValueOrDefault() != 0 ? bindingEntry.Port.ToString() : bindingEntry.TargetPort.ToString(),
            NomadifyConstants.TargetPort => bindingEntry!.TargetPort.ToString(),
            NomadifyConstants.Url => HandleUrlBinding(resourceName, bindingName, bindingEntry),
            _ => throw new InvalidOperationException($"Unknown property {bindingProperty}.")
        };
    }

    private static string HandleUrlBinding(string resourceName, string bindingName, Binding? binding) => bindingName switch
    {
        NomadifyConstants.Http => $"{NomadifyConstants.Http}://{resourceName}:{binding?.TargetPort}",
        NomadifyConstants.Https => string.Empty, // For now - disable https, only http is supported until we have a way to generate dev certs and inject into container for startup.
        _ => HandleCustomServicePortBinding(resourceName, binding),
    };

    private static string HandleCustomServicePortBinding(string resourceName, Binding? binding)
    {
        if (binding?.TargetPort == 0)
        {
            binding.TargetPort = s_servicePort;
            s_servicePort++;
        }

        var prefix = HandleServiceBindingPrefix(binding);

        return $"{prefix}{resourceName}:{binding?.TargetPort}";
    }

    private static string HandleServiceBindingPrefix(Binding? binding) => binding?.Protocol switch
    {
        NomadifyConstants.Http => $"{NomadifyConstants.Http}://",
        NomadifyConstants.Https => $"{NomadifyConstants.Https}://",
        _ => string.Empty,
    };
}
