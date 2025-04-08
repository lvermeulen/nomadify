using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;

namespace Nomadify.Commands.Options;

public sealed class CompressionKindOption : BaseOption<string>
{
    private static readonly string[] s_aliases = ["--compression-kind"];

    private CompressionKindOption() : base(s_aliases, "NOMADIFY_COMPRESSION_KIND", "none")
    {
        Name = nameof(CompressionKind);
        Description = "The compression kind: can be 'none', 'zip' or 'tar'. The default is 'none'";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
        AddValidator(ValidateFormat);
    }

    public static CompressionKindOption Instance { get; } = new();

    private static void ValidateFormat(OptionResult optionResult)
    {
        var value = optionResult.GetValueOrDefault<string>();

        if (value is null)
        {
            throw new ArgumentException("--compression-kind cannot be null.");
        }

        if (!CompressionKind.TryFromValue(value.ToLower(), out _))
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.Append("--compression-kind must be one of: '");
            errorBuilder.AppendJoin("', '", CompressionKind.List.Select(x => x.Value));
            errorBuilder.Append("' and not quoted.");

            throw new ArgumentException(errorBuilder.ToString());
        }
    }
}
