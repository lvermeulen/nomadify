using Ardalis.SmartEnum;

namespace Nomadify.Commands.Options;

public class CompressionKind : SmartEnum<CompressionKind, string>
{
    private CompressionKind(string name, string value)
        : base(name, value)
    { }

    public static readonly CompressionKind None = new(nameof(None), "none");
    public static readonly CompressionKind Zip = new(nameof(Zip), "zip");
    public static readonly CompressionKind Tar = new(nameof(Tar), "tar");
}
