using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nomadify.Models;

public class DockerConfigJson
{
    [JsonPropertyName("auths")]
    public Dictionary<string, DockerAuth>? Auths { get; set; }
}
