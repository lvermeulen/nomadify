﻿using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;

namespace Nomadify.Commands.Options;

public sealed class ContainerBuilderOption : BaseOption<string>
{
    private static readonly string[] s_aliases = ["--container-builder"];

    private ContainerBuilderOption() : base(s_aliases, "NOMADIFY_CONTAINER_BUILDER", "docker")
    {
        Name = nameof(IContainerOptions.ContainerBuilder);
        Description = "The Container Builder: can be 'docker' or 'podman'. The default is 'docker'";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
        AddValidator(ValidateFormat);
    }

    public static ContainerBuilderOption Instance { get; } = new();

    private static void ValidateFormat(OptionResult optionResult)
    {
        var value = optionResult.GetValueOrDefault<string>();

        if (value is null)
        {
            throw new ArgumentException("--container-builder cannot be null.");
        }

        if (!ContainerBuilder.TryFromValue(value.ToLower(), out _))
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.Append("--container-builder must be one of: '");
            errorBuilder.AppendJoin("', '", ContainerBuilder.List.Select(x => x.Value));
            errorBuilder.Append("' and not quoted.");

            throw new ArgumentException(errorBuilder.ToString());
        }
    }
}
