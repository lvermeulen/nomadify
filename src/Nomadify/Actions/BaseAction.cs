using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions;
using Spectre.Console;

namespace Nomadify.Actions;

public abstract class BaseAction(IServiceProvider serviceProvider) : IAction
{
    protected IAnsiConsole Logger { get; } = serviceProvider.GetRequiredService<IAnsiConsole>();
    protected NomadifyState CurrentState { get; } = serviceProvider.GetRequiredService<NomadifyState>();
    protected IServiceProvider Services { get; } = serviceProvider;

    public abstract Task<bool> ExecuteAsync();

    protected virtual bool PreviousStateWasRestored() => CurrentState.StateWasLoadedFromPrevious;
}
