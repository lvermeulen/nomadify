using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

namespace Nomad.Abstractions.Components;

[ExcludeFromCodeCoverage]
public static class NomadifyConstants
{
    public const string DefaultArtifactsPath = "nomadify-output";
    public const string DefaultAspireProjectPath = ".";
    public const string StateFileName = "nomadify-state.json";
    public const string AppDataFolder = "Nomadify";
    public const string UpdatesDisabledFile = "updates-disabled";
    public const string LogoDisabledFile = "logo-disabled";
    public const string LastVersionCheckedFile = "last-version-checked.json";

    // component names
    public const string Project = "project.v0";
    public const string Dockerfile = "dockerfile.v0";
    public const string Container = "container.v0";
    public const string Dapr = "dapr.v0";
    public const string DaprComponent = "dapr.component.v0";
    public const string Bicep = "azure.bicep.v0";
    public const string Value = "value.v0";
    public const string Parameter = "parameter.v0";

    // binding names
    public const string Bindings = "bindings";
    public const string Http = "http";
    public const string Https = "https";
    public const string Port = "port";
    public const string TargetPort = "targetPort";
    public const string Env = "env";
    public const string ConnectionString = "connectionString";
    public const string Host = "host";
    public const string Url = "url";

    // msbuild names
    public const string ContainerRegistryArgument = "ContainerRegistry";
    public const string ContainerRepositoryArgument = "ContainerRepository";
    public const string ContainerImageNameArgument = "ContainerImageName";
    public const string ContainerImageTagArgument = "ContainerImageTag";
    public const string ContainerImageTagArguments = "ContainerImageTags";
    public const string ErrorOnDuplicatePublishOutputFilesArgument = "ErrorOnDuplicatePublishOutputFiles";

    // container
    public const string ContainerRegistry = "ContainerRegistry";
    public const string ContainerRepository = "ContainerRepository";
    public const string ContainerImageName = "ContainerImageName";
    public const string ContainerImageTag = "ContainerImageTag";

    // various
    public const string DefaultManifestFile = "manifest.json";
    public const string ManifestPublisherArgument = "manifest";
    public const string DashboardImage = "mcr.microsoft.com/dotnet/aspire-dashboard:9.1-arm64v8";

    // emoji names
    public const string CheckMark = Emoji.Known.CheckMark;
    public const string Rocket = Emoji.Known.Rocket;
    public const string Warning = Emoji.Known.Warning;
}
