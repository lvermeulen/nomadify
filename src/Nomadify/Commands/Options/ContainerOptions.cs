using System.Collections.Generic;

namespace Nomadify.Commands.Options;

public class ContainerOptions
{
    public string? ContainerBuilder { get; set; } = string.Empty;
    public string? ImageName { get; set; } = string.Empty;
    public string? Registry { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public List<string>? Tags { get; set; } = ["latest"];
}
