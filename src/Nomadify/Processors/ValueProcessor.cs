﻿using System.Text.Json;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Parameters;
using Spectre.Console;

namespace Nomadify.Processors;

public class ValueProcessor(IAnsiConsole console) : BaseResourceProcessor(console)
{
    public override Resource? Deserialize(ref Utf8JsonReader reader) => JsonSerializer.Deserialize<ValueResource>(ref reader);
}
