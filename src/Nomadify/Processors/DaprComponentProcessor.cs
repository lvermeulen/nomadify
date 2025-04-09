using System.Text.Json;
using Nomad.Abstractions.Components.V0;
using Spectre.Console;

namespace Nomadify.Processors;

public class DaprComponentProcessor(IAnsiConsole console)
    : BaseResourceProcessor(console)
{
    public override Resource? Deserialize(ref Utf8JsonReader reader) => JsonSerializer.Deserialize<DaprComponentResource>(ref reader);
}
