using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0.Dapr;

public sealed class DaprResource : Resource
{
    [JsonPropertyName("dapr")]
    public Metadata? Metadata { get; set; }
}
