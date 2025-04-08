using System;
using System.Threading.Tasks;
using Nomadify.Actions;
using Nomadify.Commands.Base;

namespace Nomadify.Commands.Apply;

public sealed class ApplyCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<ApplyOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(ApplyOptions options) =>
        ActionExecutor
            .QueueAction(nameof(ApplyJobToClusterAction))
            .ExecuteCommandsAsync();
}