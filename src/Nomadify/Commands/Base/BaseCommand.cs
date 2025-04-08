using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions;
using Nomadify.Commands.Options;
using Nomadify.Extensions;
using Nomadify.Interfaces;

namespace Nomadify.Commands.Base;

[ExcludeFromCodeCoverage]
public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : BaseCommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description)
        : base(name, description)
    {
        Handler = CommandHandler.Create<TOptions, IServiceCollection>(ConstructCommand);
    }

    private async Task<int> ConstructCommand(TOptions options, IServiceCollection services)
    {
        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(services.BuildServiceProvider());

        var stateService = handler.Services.GetRequiredService<IStateService>();

        var state = GetCurrentState(handler);

        await stateService.RestoreState(state);

        handler.CurrentState.PopulateStateFromOptions(options);

        var exitCode = await handler.HandleAsync(options);

        await stateService.SaveState(state);

        return exitCode;
    }

    private static NomadifyState GetCurrentState(TOptionsHandler handler) => handler.CurrentState;
}
