using System.Text.Json.Serialization;

namespace Nomad.Abstractions;

public class NomadServiceInformation : NomadBasicServiceInformation
{
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("AllocID")]
    public string AllocId { get; set; } = string.Empty;

    public int CreateIndex { get; set; }
    public string Datacenter { get; set; } = string.Empty;

    [JsonPropertyName("ID")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("JobID")]
    public string JobId { get; set; } = string.Empty;
    
    public int ModifyIndex { get; set; }
    public string Namespace { get; set; } = string.Empty;

    [JsonPropertyName("NodeID")]
    public string NodeId { get; set; } = string.Empty;
    
    public int Port { get; set; }

    public override string ToString() => $"{ServiceName} ({Address}:{Port})";
}
