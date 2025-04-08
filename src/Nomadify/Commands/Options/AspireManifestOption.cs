using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class AspireManifestOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "-m",
        "--aspire-manifest"
    ];

    private AspireManifestOption() : base(s_aliases, "NOMADIFY_ASPIRE_MANIFEST_PATH", null)
    {
        Name = nameof(IAspireOptions.AspireManifest);
        Description = "The aspire manifest file to use";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static AspireManifestOption Instance { get; } = new();
}
