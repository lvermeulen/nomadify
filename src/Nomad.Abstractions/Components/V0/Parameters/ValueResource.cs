using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0.Parameters;

public class ValueResource : Resource
{
    [JsonExtensionData]
    public Dictionary<string, object> Values { get; init; } = [];
}
