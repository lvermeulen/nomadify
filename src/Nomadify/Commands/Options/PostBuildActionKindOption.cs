using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;

namespace Nomadify.Commands.Options;

public sealed class PostBuildActionKindOption : BaseOption<string>
{
    private static readonly string[] s_aliases = ["--post-build-action-kind"];

    private PostBuildActionKindOption() : base(s_aliases, "NOMADIFY_POST_BUILD_ACTION_KIND", "none")
    {
        Name = nameof(PostBuildActionKind);
        Description = "The post build action kind: can be 'none', 'copy' or 'move'. The default is 'none'";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
        AddValidator(ValidateFormat);
    }

    public static PostBuildActionKindOption Instance { get; } = new();

    private static void ValidateFormat(OptionResult optionResult)
    {
        var value = optionResult.GetValueOrDefault<string>();

        if (value is null)
        {
            throw new ArgumentException("--post-build-action-kind cannot be null.");
        }

        if (!PostBuildActionKind.TryFromValue(value.ToLower(), out _))
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.Append("--PostBuildAction-kind must be one of: '");
            errorBuilder.AppendJoin("', '", PostBuildActionKind.List.Select(x => x.Value));
            errorBuilder.Append("' and not quoted.");

            throw new ArgumentException(errorBuilder.ToString());
        }
    }
}
