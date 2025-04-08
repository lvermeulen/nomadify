using System;
using System.IO;
using System.Text.Json;
using Nomadify.Extensions;
using Nomadify.Interfaces;
using Nomadify.Models;

namespace Nomadify.Services;

public class NomadifyConfigurationService : INomadifyConfigurationService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public NomadifySettings? LoadConfigurationFile(string? projectPath)
    {
        var configurationPath = projectPath?.NormalizePath();
        if (string.IsNullOrWhiteSpace(configurationPath))
        {
            throw new InvalidOperationException("ProjectPath is not defined.");
        }

        var configurationFile = Path.Combine(configurationPath, NomadifySettings.FileName);

        if (!File.Exists(configurationFile))
        {
            return null;
        }

        var configurationJson = File.ReadAllText(configurationFile);

        var nomadifySettings = JsonSerializer.Deserialize<NomadifySettings>(configurationJson, _jsonSerializerOptions);
        return nomadifySettings;
    }

    public void SaveConfigurationFile(NomadifySettings? settings, string? projectPath)
    {
        ArgumentNullException.ThrowIfNull(projectPath);
        if (settings is null)
        {
            return;
        }

        var configurationPath = projectPath.NormalizePath();
        var configurationFile = Path.Combine(configurationPath, NomadifySettings.FileName);

        var configurationJson = JsonSerializer.Serialize(settings, _jsonSerializerOptions);

        File.WriteAllText(configurationFile, configurationJson);
    }
}
