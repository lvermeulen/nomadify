using System.Collections.Generic;
using Nomad.Abstractions;

namespace Nomad.JobGenerator;

public class TaskTemplate : ITaskTemplate
{
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceNameLiteral { get; set; } = string.Empty;
    public string Driver { get; set; } = string.Empty;
    public string ArtifactSourceUrl { get; set; } = string.Empty;
    public string StartCommand { get; set; } = string.Empty;
    public string TraefikPrefixName { get; set; } = string.Empty;
    public IDictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();
}
