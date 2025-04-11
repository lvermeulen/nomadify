using System.Collections.Generic;

namespace Nomadify.Processors;

public sealed class DaprComponentTemplateData
{
    public string? Name { get; private set; }
    public string? Type { get; private set; }
    public string? Version { get; private set; }
    public bool HasMetadata { get; private set; }
    public bool EmptyMetadata { get; private set; }

    public Dictionary<string, string>? Metadata { get; private set; }

    public DaprComponentTemplateData WithName(string? name)
    {
        Name = name;
        return this;
    }

    public DaprComponentTemplateData WithType(string? type)
    {
        Type = type;
        return this;
    }

    public DaprComponentTemplateData WithVersion(string? version)
    {
        Version = !string.IsNullOrWhiteSpace(version) ? version : "v1";
        return this;
    }

    public DaprComponentTemplateData WithMetadata(Dictionary<string, string>? metadata)
    {
        Metadata = metadata;

        if (Metadata is null || Metadata.Count == 0)
        {
            EmptyMetadata = true;
            HasMetadata = false;
            return this;
        }

        HasMetadata = true;
        EmptyMetadata = false;
        return this;
    }

    public Dictionary<string, object> ToKeyValues() => new()
    {
        ["name"] = Name ?? string.Empty,
        ["type"] = Type ?? string.Empty,
        ["version"] = Version ?? string.Empty,
        ["hasMetadata"] = HasMetadata,
        ["emptyMetadata"] = EmptyMetadata,
        ["metadata"] = Metadata ?? []
    };
}
