using System.Collections.Generic;
using Nomad.Abstractions.Components.V0;

namespace Nomadify.Processors;

public interface IResourceExpressionProcessor
{
    void ProcessEvaluations(Dictionary<string, Resource>? resources);
}
