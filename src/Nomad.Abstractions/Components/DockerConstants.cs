namespace Nomad.Abstractions.Components;

public static class DockerConstants
{
    public const string DockerCommand = "docker";
    public const string BuildCommand = "build";
    public const string PushCommand = "push";
    public const string PullCommand = "pull";

    // Docker build arguments
    public const string FileArgument = "--file";
    public const string BuildArgArgument = "--build-arg";
    public const string TagArgument = "--tag";
}
