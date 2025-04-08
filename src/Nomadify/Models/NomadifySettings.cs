using Nomad.Abstractions;

namespace Nomadify.Models;

public class NomadifySettings
{
    public const string FileName = "nomadify.json";

    public string? ProjectPath { get; set; }
    public string? NomadUrl { get; set; }
    public NomadifyRawExecSettings? RawExecSettings { get; set; }
    public NomadifyContainerSettings? ContainerSettings { get; set; } = new();
}