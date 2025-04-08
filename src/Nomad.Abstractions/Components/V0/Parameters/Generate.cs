using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0.Parameters;

[ExcludeFromCodeCoverage]
public class Generate
{
    [JsonPropertyName("minLength")]
    public int MinLength { get; set; }
}
