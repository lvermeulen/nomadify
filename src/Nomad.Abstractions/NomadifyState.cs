using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Nomad.Abstractions.Components.V0.Dapr;

namespace Nomad.Abstractions;

public class NomadifyState
{
    [RestorableStateProperty]
    [JsonPropertyName("projectPath")]
    public string? ProjectPath { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("nomadJobName")]
    public string? NomadJobName { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("nomadUrl")]
    public string? NomadUrl { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("rawExecSettings")]
    public NomadifyRawExecSettings RawExecSettings { get; set; } = new();

    [RestorableStateProperty]
    [JsonPropertyName("fileServerUrl")]
    public string? FileServerUrl { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("outputPath")]
    public string? OutputPath { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("replacementValuesFile")]
    public string? ReplacementValuesFile { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerRegistry")]
    public string? ContainerRegistry { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerImageTags")]
    public List<string>? ContainerImageTags { get; set; } = ["latest"];

    [RestorableStateProperty]
    [JsonPropertyName("runtimeIdentifier")]
    public string RuntimeIdentifier { get; set; } = "linux-arm64";

    [RestorableStateProperty]
    [JsonPropertyName("imagePullPolicy")]
    public string? ImagePullPolicy { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerBuilder")]
    public string? ContainerBuilder { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerRepositoryPrefix")]
    public string? ContainerRepositoryPrefix { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("privateRegistryUrl")]
    public string? PrivateRegistryUrl { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("privateRegistryUsername")]
    public string? PrivateRegistryUsername { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("privateRegistryEmail")]
    public string? PrivateRegistryEmail { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("withPrivateRegistry")]
    public bool? WithPrivateRegistry { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("includeDashboard")]
    public bool? IncludeDashboard { get; set; }

    [JsonIgnore]
    public bool? NoBuild { get; set; }

    [JsonIgnore]
    public string? AspireManifest { get; set; }

    [JsonIgnore]
    public List<string> AspireRawExecComponentsToProcess { get; set; } = [];

    [JsonIgnore]
    public List<string> AspireDockerComponentsToProcess { get; set; } = [];

    [JsonIgnore]
    public List<string> AspireDockerfileComponentsToProcess { get; set; } = [];

    [JsonIgnore]
    public Dictionary<string, Resource>? LoadedAspireManifestResources { get; set; } = new();

    [JsonIgnore]
    public string? PrivateRegistryPassword { get; set; }

    [JsonIgnore]
    public bool StateWasLoadedFromPrevious { get; set; }

    [JsonIgnore]
    public bool UseAllPreviousStateValues { get; set; }

    [JsonIgnore]
    public List<string> AspireComponentsToProcess => DaprProjectComponents
        .Union(SelectedRawExecProjectComponents)
        .Union(SelectedDockerProjectComponents)
        .Union(SelectedDockerfileProjectComponents)
        .Select(x => x.Key)
        .ToList();

    [JsonIgnore]
    public List<(string Key, Resource? Value)> DaprProjects => LoadedAspireManifestResources
        .Where(x => x.Value is DaprResource && AspireRawExecComponentsToProcess.Contains(x.Key))
        .Select(x => (x.Key, x.Value))
        .ToList();

    [JsonIgnore]
    public List<(string Key, Resource? Value)> DaprComponents => LoadedAspireManifestResources
        .Where(x => x.Value is DaprComponentResource)
        .Select(x => (x.Key, x.Value))
        .ToList();

    [JsonIgnore]
    public List<(string Key, Resource? Value)> DaprProjectComponents => DaprProjects
        .Union(DaprComponents)
        .Select(x => (x.Key, x.Value))
        .ToList();

    [JsonIgnore]
    public List<(string Key, Resource? Value)> SelectedRawExecProjectComponents => LoadedAspireManifestResources
        .Where(x => x.Value is ProjectResource && AspireRawExecComponentsToProcess.Contains(x.Key))
        .Select(x => (x.Key, x.Value))
        .ToList();

    [JsonIgnore]
    public List<(string Key, Resource? Value)> SelectedDockerProjectComponents => LoadedAspireManifestResources
        .Where(x => x.Value is ProjectResource && AspireDockerComponentsToProcess.Contains(x.Key))
        .Select(x => (x.Key, x.Value))
        .ToList();

    [JsonIgnore]
    public List<(string Key, Resource? Value)> SelectedDockerfileProjectComponents => LoadedAspireManifestResources
        .Where(x => x.Value is DockerfileResource && AspireDockerfileComponentsToProcess.Contains(x.Key))
        .Select(x => (x.Key, x.Value))
        .ToList();

    [JsonIgnore]
    public List<(string Key, Resource Value)> AllSelectedSupportedComponents => LoadedAspireManifestResources
        .Where(x => x.Value is not UnsupportedResource && AspireComponentsToProcess.Contains(x.Key))
        .Select(x => (x.Key, x.Value))
        .ToList();

    public bool HasDapr() => DaprProjectComponents.Count > 0;
}
