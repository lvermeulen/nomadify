using System;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Nomadify.Commands.Options;

public sealed class NomadUrlOption : BaseOption<string>
{
    private static readonly string[] s_aliases = ["--nomad-url"];

    private NomadUrlOption() : base(s_aliases, "NOMADIFY_NOMAD_URL", "")
    {
        Name = nameof(IContainerOptions.ContainerBuilder);
        Description = "The Nomad URL";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
        AddValidator(ValidateFormat);
    }

    public static NomadUrlOption Instance { get; } = new();

    private static void ValidateFormat(OptionResult optionResult)
    {
        var value = optionResult.GetValueOrDefault<string>();

        if (value is null)
        {
            throw new ArgumentException("--nomad-url cannot be null.");
        }
    }
}
