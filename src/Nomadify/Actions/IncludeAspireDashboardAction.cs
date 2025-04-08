using System;
using System.Threading.Tasks;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Actions;

public class IncludeAspireDashboardAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (PreviousStateWasRestored())
        {
            return Task.FromResult(true);
        }

        Logger.WriteRuler("[purple]Handling Aspire Dashboard[/]");

        if (CurrentState.IncludeDashboard is not null)
        {
            return Task.FromResult(true);
        }

        AskShouldIncludeDashboard();

        return Task.FromResult(true);
    }

    private void AskShouldIncludeDashboard()
    {
        var shouldIncludeAspireDashboard = Logger.Confirm("[bold]Would you like to deploy the aspire dashboard and connect the OTLP endpoint?[/]");

        CurrentState.IncludeDashboard = shouldIncludeAspireDashboard;
        if (!shouldIncludeAspireDashboard)
        {
            Logger.MarkupLine("[yellow](!)[/] Skipping Aspire Dashboard deployment");
        }
    }
}