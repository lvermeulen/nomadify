using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class ContainerRegistryOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "-cr",
        "--container-registry"
    ];

    private ContainerRegistryOption() : base(s_aliases, "NOMADIFY_CONTAINER_REGISTRY", null)
    {
        Name = nameof(IContainerOptions.ContainerRegistry);
        Description = "The Container Registry to use as the fall-back value for all containers";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ContainerRegistryOption Instance { get; } = new();
}
