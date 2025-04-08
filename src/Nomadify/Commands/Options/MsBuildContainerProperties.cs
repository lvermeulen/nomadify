using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Nomad.Abstractions.Components;

namespace Nomadify.Commands.Options;

[ExcludeFromCodeCoverage]
public sealed class MsBuildContainerProperties : IMsBuildProperties
{
    [JsonPropertyName(NomadifyConstants.ContainerRegistryArgument)]
    public string? ContainerRegistry { get; set; }

    [JsonPropertyName(NomadifyConstants.ContainerRepositoryArgument)]
    public string? ContainerRepository { get; set; }

    [JsonPropertyName(NomadifyConstants.ContainerImageNameArgument)]
    public string? ContainerImageName { get; set; }

    [JsonPropertyName(NomadifyConstants.ContainerImageTagArgument)]
    public string? ContainerImageTag { get; set; }

    [JsonIgnore]
    public string? FullContainerImage { get; set; }
}
