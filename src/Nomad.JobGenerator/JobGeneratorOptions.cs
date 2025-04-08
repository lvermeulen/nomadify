using System.Collections.Generic;
using Nomad.Abstractions;

namespace Nomad.JobGenerator;

public record JobFileGeneratorOptions(string ServiceName, IDictionary<string, ITaskTemplate> TaskTemplates, IDictionary<string, object> DefaultValues, NomadifyState State) : IJobFileGeneratorOptions;
