using System.Collections.Generic;
using System.Text.Json;
using Nomad.Abstractions.Components.V0;
using Spectre.Console;

namespace Nomadify.Processors;

public abstract class BaseResourceProcessor(IAnsiConsole console) : IResourceProcessor
{
    public IAnsiConsole Console { get; } = console;

    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    public abstract List<object> CreateNomadObjects();
}
