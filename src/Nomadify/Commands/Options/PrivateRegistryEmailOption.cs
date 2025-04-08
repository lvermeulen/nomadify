using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class PrivateRegistryEmailOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "--private-registry-email"
    ];

    private PrivateRegistryEmailOption() : base(s_aliases, "NOMADIFY_PRIVATE_REGISTRY_EMAIL", "nomadify@aka.ms")
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.PrivateRegistryEmail);
        Description = "The Private Registry email. It is required and defaults to 'nomadify@aka.ms'.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static PrivateRegistryEmailOption Instance { get; } = new();
}
