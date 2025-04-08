using System.Collections.Generic;
using System.Linq;

namespace Nomad.Abstractions;

public interface ITaskTemplate
{
    string ServiceName { get; set; }
    string ServiceNameLiteral { get; set; }
    string Driver { get; set; }
    string ArtifactSourceUrl { get; set; }
    string StartCommand { get; set; }
    string TraefikPrefixName { get; set; }
    IDictionary<string, string> EnvironmentVariables { get; set; }
}

public static class TaskTemplateExtensions
{
    public static IDictionary<string, object> ToKeyValues(this ITaskTemplate taskTemplate) => new Dictionary<string, object>
    {
        ["GroupServiceName"] = taskTemplate.ServiceName,
        ["GroupServiceNameLiteral"] = taskTemplate.ServiceNameLiteral,
        ["GroupTaskDriver"] = taskTemplate.Driver,
        ["GroupTaskArtifactSourceUrl"] = taskTemplate.ArtifactSourceUrl,
        ["GroupTaskConfigStartCommand"] = taskTemplate.StartCommand,
        ["GroupTraefikPrefixName"] = taskTemplate.TraefikPrefixName,
        ["GroupTaskEnv"] = taskTemplate.EnvironmentVariables.Select(x =>
                x.Value.IsPrimitive()
                    ? $"{x.Key}={x.Value}"
                    : $"{x.Key}={x.Value.DoubleQuoted()}")
            .ToList()
    };
}
