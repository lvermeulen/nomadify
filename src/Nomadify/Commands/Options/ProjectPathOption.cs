using System.CommandLine;
using Nomad.Abstractions.Components;

namespace Nomadify.Commands.Options;

public sealed class ProjectPathOption : BaseOption<string>
{
    private static readonly string[] s_aliases =
    [
        "-p",
        "--project-path"
    ];

    private ProjectPathOption() : base(s_aliases, "NOMADIFY_PROJECT_PATH", NomadifyConstants.DefaultAspireProjectPath)
    {
        Name = nameof(IAspireOptions.ProjectPath);
        Description = "The path to the aspire project";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ProjectPathOption Instance { get; } = new();
}