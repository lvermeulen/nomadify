using System.Collections.Generic;

namespace Nomadify.Models;

public class NomadifyContainerSettings
{
    public string? Registry { get; set; }
    public string? RepositoryPrefix { get; set; }
    public List<string>? Tags { get; set; }
    public string? Builder { get; set; }
}