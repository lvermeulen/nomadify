namespace Nomadify.Commands.Options;

public interface IGenerateOptions
{
    string? OutputPath { get; set; }
    bool? NoBuild { get; set; }
    string? ImagePullPolicy { get; set; }
}
