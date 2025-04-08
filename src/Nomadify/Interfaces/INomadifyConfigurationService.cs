using Nomadify.Models;

namespace Nomadify.Interfaces;

public interface INomadifyConfigurationService
{
    NomadifySettings? LoadConfigurationFile(string? projectPath);
    void SaveConfigurationFile(NomadifySettings? settings, string? projectPath);
}
