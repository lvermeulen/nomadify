using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using Nomadify.Interfaces;
using Nomadify.Processors;

namespace Nomadify.Services;

public class ManifestFileParserService(IServiceProvider serviceProvider, ILogger<ManifestFileParserService> logger) : IManifestFileParserService
{
    public Dictionary<string, Resource> LoadAndParseAspireManifest(string? manifestFile)
    {
        if (manifestFile is null)
        {
            return new();
        }

        var resources = new Dictionary<string, Resource>();

        if (!File.Exists(manifestFile))
        {
            throw new InvalidOperationException($"The manifest file could not be loaded from: '{manifestFile}'");
        }

        var inputJson = File.ReadAllText(manifestFile);

        var jsonObject = JsonSerializer.Deserialize<JsonElement>(inputJson);

        if (!jsonObject.TryGetProperty("resources", out var resourcesElement) || resourcesElement.ValueKind != JsonValueKind.Object)
        {
            return resources;
        }

        foreach (var resourceProperty in resourcesElement.EnumerateObject())
        {
            var resourceName = resourceProperty.Name;
            var resourceElement = resourceProperty.Value;

            var type = resourceElement.TryGetProperty("type", out var typeElement) ? typeElement.GetString() : null;

            if (type is null)
            {
                logger.LogInformation($"[yellow]Resource {resourceName} does not have a type. Skipping as UnsupportedResource.[/]");
                resources.Add(resourceName, new UnsupportedResource());
                continue;
            }

            var rawBytes = Encoding.UTF8.GetBytes(resourceElement.GetRawText());
            var reader = new Utf8JsonReader(rawBytes);

            var handler = serviceProvider.GetKeyedService<IResourceProcessor>(type);
            var resource = handler != null
                ? handler.Deserialize(ref reader)
                : new UnsupportedResource();

            if (resource is not null)
            {
                resource.Name = resourceName;
                resources.Add(resourceName, resource);
            }
        }

        return resources;
    }
}
