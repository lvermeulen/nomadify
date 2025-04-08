using Ardalis.SmartEnum;

namespace Nomadify.Commands.Options;

public class ContainerBuilder : SmartEnum<ContainerBuilder, string>
{
    private ContainerBuilder(string name, string value)
        : base(name, value)
    { }

    public static readonly ContainerBuilder Docker = new(nameof(Docker), "docker");
    public static readonly ContainerBuilder Podman = new(nameof(Podman), "podman");
}
