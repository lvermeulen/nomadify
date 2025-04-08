using System.Collections.Generic;
using Nomadify.Commands.Base;
using Nomadify.Commands.Options;

namespace Nomadify.Commands.Generate;

public sealed class GenerateOptions : BaseCommandOptions,
    IBuildOptions,
    IContainerOptions,
    IAspireOptions,
    IGenerateOptions,
    IPrivateRegistryCredentialsOptions,
    IDashboardOptions
{
    public string? AspireManifest { get; set; }
    public string? OutputPath { get; set; }
    public bool? NoBuild { get; set; }
    public string? ContainerBuilder { get; set; }
    public string? ContainerRegistry { get; set; }
    public string? ContainerRepositoryPrefix { get; set; }
    public List<string>? ContainerImageTags { get; set; }
    public string? ImagePullPolicy { get; set; }
    public string RuntimeIdentifier { get; set; } = "linux-arm64";
    public string? PrivateRegistryUrl { get; set; }
    public string? PrivateRegistryUsername { get; set; }
    public string? PrivateRegistryPassword { get; set; }
    public string? PrivateRegistryEmail { get; set; }
    public bool? WithPrivateRegistry { get; set; }
    public bool? IncludeDashboard { get; set; }
}
