using System.Text.Json.Nodes;

namespace Nomadify.Processors;

public interface IJsonExpressionProcessor
{
    void ResolveJsonExpressions(JsonNode? jsonNode, JsonNode? rootNode);
}
