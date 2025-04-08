using System.Collections.Generic;

namespace Nomad.Abstractions;

public class NomadServices
{
    public string Namespace { get; set; } = string.Empty;
    public IEnumerable<NomadBasicServiceInformation> Services { get; set; } = [];
}
