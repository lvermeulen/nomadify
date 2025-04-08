using System.CommandLine;
using Nomad.Abstractions.Components;

namespace Nomadify.Commands.Options;

public sealed class OutputPathOption : BaseOption<string>
{
    private static readonly string[] s_aliases =
    [
        "-o",
        "--output-path"
    ];

    private OutputPathOption() : base(s_aliases, "NOMADIFY_OUTPUT_PATH", NomadifyConstants.DefaultArtifactsPath)
    {
        Name = nameof(IGenerateOptions.OutputPath);
        Description = "The output path for generated manifests";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static OutputPathOption Instance { get; } = new();
}
