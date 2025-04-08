using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Nomad.Abstractions.Interfaces;

namespace Nomad.Abstractions.Components.V0;

[ExcludeFromCodeCoverage]
public class BicepResource : Resource, IResourceWithConnectionString, IResourceWithAnnotations, IResourceWithEnvironmentalVariables, IResourceWithArgs
{
    [JsonPropertyName("connectionString")]
    public string? ConnectionString { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("annotations")]
    public Dictionary<string, string>? Annotations { get; set; } = [];

    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; set; } = [];

    [JsonPropertyName("args")]
    public List<string>? Args { get; set; } = [];

    [JsonPropertyName("params")]
    public Dictionary<string, string>? Inputs { get; set; }
}
