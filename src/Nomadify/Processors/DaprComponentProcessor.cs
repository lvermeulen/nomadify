﻿using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Dapr;
using Nomad.JobGenerator;
using Spectre.Console;
using Stubble.Core.Builders;

namespace Nomadify.Processors;

public class DaprComponentProcessor(IAnsiConsole console)
    : BaseResourceProcessor(console)
{
    public override Resource? Deserialize(ref Utf8JsonReader reader) => JsonSerializer.Deserialize<DaprComponentResource>(ref reader);

    public static async Task<bool> CreateDaprTemplateAsync(Resource? resource, string outputPath)
    {
        var daprComponentResource = resource as DaprComponentResource;
        if (daprComponentResource?.DaprComponentProperty is null)
        {
            return false;
        }

        var templateData = new DaprComponentTemplateData()
            .WithName(daprComponentResource.Name)
            .WithType(daprComponentResource.DaprComponentProperty.Type)
            .WithVersion(daprComponentResource.DaprComponentProperty.Version)
            .WithMetadata(daprComponentResource.DaprComponentProperty.Metadata);

        await CreateDaprTemplateAsync(outputPath, templateData, daprComponentResource.Name);

        return true;
    }

    private static async Task CreateDaprTemplateAsync(string outputPath, DaprComponentTemplateData data, string name)
    {
        var daprOutputPath = Path.Combine(outputPath, "dapr");
        if (!Directory.Exists(daprOutputPath))
        {
            Directory.CreateDirectory(daprOutputPath);
        }

        var daprFileOutputPath = Path.Combine(daprOutputPath, $"{name}.yaml");

        await CreateFileAsync(daprFileOutputPath, data);
    }

    private static async Task CreateFileAsync(string outputPath, DaprComponentTemplateData data)
    {
        var stubble = new StubbleBuilder().Build();
        var output = await stubble.RenderAsync(Templates.DaprComponentTemplate, data.ToKeyValues());

        await File.WriteAllTextAsync(outputPath, output);
    }
}
