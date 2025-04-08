using System.Collections.Generic;
using Nomad.Abstractions.Components.V0.Parameters;

namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithInput : IResource
{
    Dictionary<string, ParameterInput>? Inputs { get; set; }
}
