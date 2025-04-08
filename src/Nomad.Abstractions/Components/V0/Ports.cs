using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0;

public class Ports
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("internalPort")]
    public int InternalPort { get; set; }

    [JsonPropertyName("externalPort")]
    public int ExternalPort { get; set; }
}
