using System;
using System.Threading.Tasks;
using Nomad.Abstractions;

namespace Nomadify.Commands.Options;

public interface ICommandOptionsHandler<in TOptions>
{
    NomadifyState CurrentState { get; set; }

    Task<int> HandleAsync(TOptions options);

    IServiceProvider Services { get; }
}
