﻿using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;

namespace Nomadify.Commands.Options;

public sealed class ImagePullPolicyOption : BaseOption<string?>
{
    private static readonly string[] s_aliases =
    [
        "--image-pull-policy"
    ];

    private ImagePullPolicyOption() : base(s_aliases, "NOMADIFY_IMAGE_PULL_POLICY", null)
    {
        Name = nameof(IGenerateOptions.ImagePullPolicy);
        Description = "The Image pull policy to use when generating manifests";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
        AddValidator(ValidateFormat);
    }

    public static ImagePullPolicyOption Instance { get; } = new();

    private static void ValidateFormat(OptionResult optionResult)
    {
        var value = optionResult.GetValueOrDefault<string?>();

        if (value is null)
        {
            return;
        }

        if (!ImagePullPolicy.TryFromValue(value, out _))
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.Append("--image-pull-policy must be one of: '");
            errorBuilder.AppendJoin("', '", ImagePullPolicy.List.Select(x => x.Value));
            errorBuilder.Append("' and not quoted. It is case sensitive.");

            throw new ArgumentException(errorBuilder.ToString());
        }
    }
}
