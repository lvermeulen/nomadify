using System.Collections.Generic;
using System.Text.Json;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Dapr;
using Spectre.Console;

namespace Nomadify.Processors;

public class DaprProcessor(IAnsiConsole console)
    : BaseResourceProcessor(console)
{
    public override Resource? Deserialize(ref Utf8JsonReader reader) => JsonSerializer.Deserialize<DaprResource>(ref reader);

    public override List<object> CreateNomadObjects() => [];
}
