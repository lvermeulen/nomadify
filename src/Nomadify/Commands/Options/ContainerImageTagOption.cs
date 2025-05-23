﻿using System.Collections.Generic;
using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class ContainerImageTagOption : BaseOption<List<string>?>
{
    private static readonly string[] s_aliases =
    [
        "-ct",
        "--container-image-tag"
    ];

    private ContainerImageTagOption() : base(s_aliases, "NOMADIFY_CONTAINER_IMAGE_TAG", null)
    {
        Name = nameof(IContainerOptions.ContainerImageTags);
        Description = "The Container Image Tags to use for all containers. Can include multiple times.";
        Arity = ArgumentArity.ZeroOrMore;
        IsRequired = false;
    }

    public static ContainerImageTagOption Instance { get; } = new();
}
