using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0;

public sealed class DaprComponentResource : Resource
{
    [JsonPropertyName("daprComponent")]
    public InnerDaprComponent? DaprComponentProperty { get; set; }
}
