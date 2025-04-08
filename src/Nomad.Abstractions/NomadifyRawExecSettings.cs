using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nomad.Abstractions;

public record NomadifyRawExecSettings
{
    [JsonPropertyName("compressionKind")]
    public string? CompressionKind { get; set; }

    [JsonPropertyName("postBuildActions")]
    public IEnumerable<PostBuildAction>? PostBuildActions { get; set; }
}