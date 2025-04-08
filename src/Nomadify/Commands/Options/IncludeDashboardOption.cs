using System.CommandLine;

namespace Nomadify.Commands.Options;

public sealed class IncludeDashboardOption : BaseOption<bool?>
{
    private static readonly string[] s_aliases = ["--include-dashboard", "--with-dashboard"];

    private IncludeDashboardOption() : base(s_aliases, "NOMADIFY_INCLUDE_DASHBOARD", null)
    {
        Name = nameof(IDashboardOptions.IncludeDashboard);
        Description = "Include the Aspire Dashboard in the generated manifests";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static IncludeDashboardOption Instance { get; } = new();
}
