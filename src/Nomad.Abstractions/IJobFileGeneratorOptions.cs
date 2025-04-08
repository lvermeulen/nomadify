using System.Collections.Generic;

namespace Nomad.Abstractions;

public interface IJobFileGeneratorOptions
{
    string ServiceName { get; init; }
    IDictionary<string, ITaskTemplate> TaskTemplates { get; init; }
    IDictionary<string, object> DefaultValues { get; init; }
    NomadifyState State { get; init; }

    bool HasDapr() => State.HasDapr();
}

public static class JobFileGeneratorExtensions
{
    public static IDictionary<string, object> ToKeyValues(this IJobFileGeneratorOptions options)
    {
        var result = new Dictionary<string, object>
        {
            ["JobServiceName"] = options.ServiceName
        };

        return result;
    }
}
