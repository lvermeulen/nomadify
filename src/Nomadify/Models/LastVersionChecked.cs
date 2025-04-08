using System;
using System.Text.Json.Serialization;

namespace Nomadify.Models;

public sealed class LastVersionChecked
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("lastChecked")]
    public DateTime LastChecked { get; set; }
}
