using System.Diagnostics.CodeAnalysis;

namespace Nomad.Abstractions.Components;

[ExcludeFromCodeCoverage]
public static class DotNetSdkConstants
{
    public const string DuplicateFileOutputError = "NETSDK1152";
    public const string UnknownContainerRegistryAddress = "CONTAINER1013";

    public const string MsBuildArgument = "msbuild";
    public const string RunArgument = "run";
    public const string PublishArgument = "publish";
    public const string ArgumentDelimiter = "--";
    public const string VerbosityArgument = "--verbosity";

    public const string ProjectArgument = "--project";
    public const string PublisherArgument = "--publisher";
    public const string OutputArgument = "--output";
    public const string OutputPathArgument = "--output-path";
    public const string NoSelfContainedArgument = "--no-self-contained";
    public const string NoLogoArgument = "--nologo";
    public const string RuntimeIdentifierArgument = "--runtime";
    public const string ContainerTargetArgument = "-t:PublishContainer";
    public const string GetPropertyArgument = "--getProperty";

    public const string ContainerRegistryArgument = $"-p:{NomadifyConstants.ContainerRegistryArgument}";
    public const string ContainerRepositoryArgument = $"-p:{NomadifyConstants.ContainerRepositoryArgument}";
    public const string ContainerImageNameArgument = $"-p:{NomadifyConstants.ContainerImageNameArgument}";
    public const string ContainerImageTagArgument = $"-p:{NomadifyConstants.ContainerImageTagArgument}";
    public const string ContainerImageTagArguments = $"-p:{NomadifyConstants.ContainerImageTagArguments}";
    public const string ErrorOnDuplicatePublishOutputFilesArgument = $"-p:{NomadifyConstants.ErrorOnDuplicatePublishOutputFilesArgument}";

    public const string DefaultVerbosity = "quiet";

    public const string DotNetCommand = "dotnet";
}
