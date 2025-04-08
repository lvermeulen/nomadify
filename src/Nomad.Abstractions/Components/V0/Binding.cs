using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0;

[ExcludeFromCodeCoverage]
public class Binding
{
    [JsonPropertyName("scheme")]
    public string? Scheme { get; set; } = "http";

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; } = "tcp";

    [JsonPropertyName("transport")]
    public string? Transport { get; set; }

    [JsonPropertyName("port")]
    public int? Port { get; set; }

    [JsonPropertyName("targetPort")]
    public int? TargetPort { get; set; }

    [JsonPropertyName("external")]
    public bool External { get; set; }
}
