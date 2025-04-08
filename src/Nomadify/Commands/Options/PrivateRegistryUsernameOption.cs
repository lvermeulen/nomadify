using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class PrivateRegistryUsernameOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "--private-registry-username"
    ];

    private PrivateRegistryUsernameOption() : base(s_aliases, "NOMADIFY_PRIVATE_REGISTRY_USERNAME", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.PrivateRegistryUsername);
        Description = "The Private Registry username.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static PrivateRegistryUsernameOption Instance { get; } = new();
}
