using System.Collections.Generic;

namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithArgs : IResource
{
    List<string>? Args { get; set; }
}
