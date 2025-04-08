using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class RuntimeIdentifierOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "--runtime-identifier"
    ];

    private RuntimeIdentifierOption() : base(s_aliases, "NOMADIFY_RUNTIME_IDENTIFIER", null)
    {
        Name = nameof(IBuildOptions.RuntimeIdentifier);
        Description = "The Custom Runtime identifier to use for .net project builds.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static RuntimeIdentifierOption Instance { get; } = new();
}
