namespace Nomadify.Commands.Options;

public interface IRunOptions
{
    string? Namespace { get; set; }
    bool? NoBuild { get; set; }
    string? ImagePullPolicy { get; set; }
    string? RuntimeIdentifier { get; set; }
}
