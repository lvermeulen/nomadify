using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions;
using Nomadify.Actions;
using Nomadify.Commands.Options;

namespace Nomadify.Commands.Base;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptionsHandler<TOptions> : ICommandOptionsHandler<TOptions> where TOptions : class, ICommandOptions
{
    protected BaseCommandOptionsHandler(IServiceProvider serviceProvider)
    {
        Services = serviceProvider;
        CurrentState = Services.GetRequiredService<NomadifyState>();
        ActionExecutor = ActionExecutor.CreateInstance(serviceProvider);
    }

    public NomadifyState CurrentState { get; set; }
    public IServiceProvider Services { get; }
    protected ActionExecutor ActionExecutor { get; set; }
    public abstract Task<int> HandleAsync(TOptions options);
}
