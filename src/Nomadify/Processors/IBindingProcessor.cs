using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Nomadify.Processors;

public interface IBindingProcessor
{
    string HandleBindingReplacement(JsonNode? rootNode, IReadOnlyList<string> pathParts, string input, string jsonPath);
}
