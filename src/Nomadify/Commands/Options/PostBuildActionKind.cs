using Ardalis.SmartEnum;

namespace Nomadify.Commands.Options;

public class PostBuildActionKind : SmartEnum<PostBuildActionKind, string>
{
    private PostBuildActionKind(string name, string value)
        : base(name, value)
    { }

    public static readonly PostBuildActionKind None = new(nameof(None), "none");
    public static readonly PostBuildActionKind Copy = new(nameof(Copy), "copy");
    public static readonly PostBuildActionKind Move = new(nameof(Move), "move");
}
