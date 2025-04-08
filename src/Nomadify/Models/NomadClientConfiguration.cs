using Microsoft.Extensions.Configuration;

namespace Nomadify.Models;

public static class NomadClientConfiguration
{
    public static string? GetConfigurationFor(string configurationKey)
    {
        var config = LoadConfiguration(configurationKey);
        return config;
    }

    private static string? LoadConfiguration(string configurationKey)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(NomadifySettings.FileName)
            .Build();

        var result = configuration[configurationKey];
        return result;
    }
}