using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0.Dapr;

public sealed class DaprComponentResource : Resource
{
    [JsonPropertyName("daprComponent")]
    public InnerDaprComponent? DaprComponentProperty { get; set; }
}
