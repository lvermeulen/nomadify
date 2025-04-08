using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class NoBuildOption : BaseOption<bool?>
{
    private static readonly string[] s_aliases = ["--no-build"];

    private NoBuildOption() : base(s_aliases, "NOMADIFY_NO_BUILD", null)
    {
        Name = nameof(IGenerateOptions.NoBuild);
        Description = "Skips build and push of components";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static NoBuildOption Instance { get; } = new();
}
