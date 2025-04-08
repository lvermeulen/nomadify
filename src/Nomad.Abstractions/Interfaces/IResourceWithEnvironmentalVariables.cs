using System.Collections.Generic;

namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithEnvironmentalVariables : IResource
{
    Dictionary<string, string>? Env { get; set; }
}
