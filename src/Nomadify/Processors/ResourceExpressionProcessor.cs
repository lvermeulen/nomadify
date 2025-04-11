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

public sealed class ResourceExpressionProcessor(JsonExpressionProcessor jsonExpressionProcessor)
{
    public (int numberOfBindings, int numberOfSubstitutions) ProcessEvaluations(Dictionary<string, Resource>? resources)
    {
        // bindings
        var numberOfBindings = resources?.EnsureBindingsHavePorts();

        // substitutions
        var jsonDocument = resources?.Where(r => r.Value is not UnsupportedResource)
            .ToDictionary(p => p.Key, p => p.Value)
            .TryParseAsJsonNode();
        var rootNode = jsonDocument?.Root;
        jsonExpressionProcessor.ResolveJsonExpressions(rootNode, rootNode);
        var numberOfSubstitutions = HandleSubstitutions(resources, rootNode);

        return (numberOfBindings.GetValueOrDefault(), numberOfSubstitutions);
    }

    private static int HandleSubstitutions(Dictionary<string, Resource>? resources, JsonNode? rootNode)
    {
        if (resources is null || rootNode is null)
        {
            return 0;
        }

        var counter = 0;

        foreach (var (key, value) in resources)
        {
            switch (value)
            {
                case IResourceWithConnectionString resourceWithConnectionString when !string.IsNullOrEmpty(resourceWithConnectionString.ConnectionString):
                    resourceWithConnectionString.ConnectionString = rootNode[key]![NomadifyConstants.ConnectionString]!.ToString();
                    counter++;
                    break;
                case ValueResource valueResource:
                {
                    foreach (var resourceValue in valueResource.Values.Select(x => x.Key))
                    {
                        valueResource.Values[resourceValue] = rootNode[key]![resourceValue]!.ToString();
                        counter++;
                    }

                    break;
                }
            }
        }

        return counter;
    }
}
