using System.Collections.Generic;
using System.Threading.Tasks;
using Nomadify.Execution;
using Nomadify.Interfaces;

namespace Nomadify.Services;

public class DaprCliService(IProcessRunner processRunner) : IDaprCliService
{
    private string _daprPath = "dapr";

    public bool IsDaprCliInstalledOnMachine()
    {
        var result = processRunner.IsCommandAvailable("dapr");

        if (result.IsFailed)
        {
            return false;
        }

        _daprPath = result.Value;
        return true;
    }

    public async Task<bool> IsDaprInstalledInCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("status", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await processRunner.ExecuteCommandWithEnvironmentNoOutput(_daprPath, argumentsBuilder, new Dictionary<string, string?>());
    }

    public async Task<ProcessRunResult> InstallDaprInCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("init", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await processRunner.ExecuteCommand(new()
        {
            Command = _daprPath,
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = true,
        });
    }

    public async Task<ProcessRunResult> RemoveDaprFromCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("uninstall", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await processRunner.ExecuteCommand(new()
        {
            Command = _daprPath,
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = true,
        });
    }
}