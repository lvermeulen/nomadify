using System.Text.Json.Serialization;

namespace Nomadify.Models;

public class DockerAuth
{
    [JsonPropertyName("auth")]
    public string? Auth { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
