using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class PrivateRegistryPasswordOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "--private-registry-password"
    ];

    private PrivateRegistryPasswordOption() : base(s_aliases, "NOMADIFY_PRIVATE_REGISTRY_PASSWORD", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.PrivateRegistryPassword);
        Description = "The Private Registry password.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static PrivateRegistryPasswordOption Instance { get; } = new();
}
