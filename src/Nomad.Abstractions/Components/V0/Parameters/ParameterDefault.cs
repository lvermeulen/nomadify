using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0.Parameters;

public class ParameterDefault
{
    [JsonPropertyName("generate")]
    public Generate? Generate { get; set; }
}
