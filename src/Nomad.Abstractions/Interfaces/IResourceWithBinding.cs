using System.Collections.Generic;
using Nomad.Abstractions.Components.V0;

namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithBinding : IResource
{
    Dictionary<string, Binding>? Bindings { get; set; }
}
