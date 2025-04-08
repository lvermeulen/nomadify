using System.Text.Json.Serialization;

namespace Nomad.Abstractions;

public struct PostBuildAction
{
    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    [JsonPropertyName("parameters")]
    public string Parameters { get; set; }
}
