using Nomadify.Commands.Base;
using Nomadify.Commands.Options;

namespace Nomadify.Commands.Apply;

public sealed class ApplyCommand : BaseCommand<ApplyOptions, ApplyCommandHandler>
{
    public ApplyCommand() : base("apply", "Apply the generated nomad manifest to the cluster.")
    {
        AddOption(ProjectPathOption.Instance);
    }
}
