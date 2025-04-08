using System.Collections.Generic;

namespace Nomad.Abstractions;

public class NomadBasicServiceInformation
{
    public string ServiceName { get; set; } = string.Empty;
    public IEnumerable<object> Tags { get; set; } = [];

    public override string ToString() => ServiceName;
}
