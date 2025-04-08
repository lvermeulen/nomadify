using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Nomad.Abstractions;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Parameters;
using Nomad.Abstractions.Interfaces;
using Nomadify.Extensions;

namespace Nomadify.Processors;

public sealed class ResourceExpressionProcessor(IJsonExpressionProcessor jsonExpressionProcessor) : IResourceExpressionProcessor
{
    public void ProcessEvaluations(Dictionary<string, Resource>? resources)
    {
        resources?.EnsureBindingsHavePorts();

        var jsonDocument = resources?.Where(r => r.Value is not UnsupportedResource)
            .ToDictionary(p => p.Key, p => p.Value)
            .TryParseAsJsonNode();

        var rootNode = jsonDocument?.Root;

        jsonExpressionProcessor.ResolveJsonExpressions(rootNode, rootNode);

        HandleSubstitutions(resources, rootNode);
    }

    private static void HandleSubstitutions(Dictionary<string, Resource>? resources, JsonNode? rootNode)
    {
        if (resources is null || rootNode is null)
        {
            return;
        }

        foreach (var (key, value) in resources)
        {
            switch (value)
            {
                case IResourceWithConnectionString resourceWithConnectionString when !string.IsNullOrEmpty(resourceWithConnectionString.ConnectionString):
                    resourceWithConnectionString.ConnectionString = rootNode[key]![NomadifyConstants.ConnectionString]!.ToString();
                    break;
                case ValueResource valueResource:
                {
                    foreach (var resourceValue in valueResource.Values.Select(x => x.Key))
                    {
                        valueResource.Values[resourceValue] = rootNode[key]![resourceValue]!.ToString();
                    }

                    break;
                }
            }
        }
    }
}
