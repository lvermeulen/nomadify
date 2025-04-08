using System.Collections.Generic;

namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithAnnotations : IResource
{
    Dictionary<string, string>? Annotations { get; set; }
}
