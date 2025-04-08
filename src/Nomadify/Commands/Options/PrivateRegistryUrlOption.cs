using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class PrivateRegistryUrlOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "--private-registry-url"
    ];

    private PrivateRegistryUrlOption() : base(s_aliases, "NOMADIFY_PRIVATE_REGISTRY_URL", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.PrivateRegistryUrl);
        Description = "The Private Registry url.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static PrivateRegistryUrlOption Instance { get; } = new();
}
