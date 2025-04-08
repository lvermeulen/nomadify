using Ardalis.SmartEnum;

namespace Nomadify.Commands.Options;

public class ImagePullPolicy : SmartEnum<ImagePullPolicy, string>
{
    private ImagePullPolicy(string name, string value)
        : base(name, value)
    { }

    public static readonly ImagePullPolicy IfNotPresent = new(nameof(IfNotPresent), "IfNotPresent");
    public static readonly ImagePullPolicy Always = new(nameof(Always), "Always");
    public static readonly ImagePullPolicy Never = new(nameof(Never), "Never");
}
