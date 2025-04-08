using System.Threading;
using System.Threading.Tasks;
using Nomad.Abstractions;
using Nomad.JobGenerator;
using Nomadify.Execution;

namespace Nomad.Cli;

public class NomadCliService(IProcessRunner processRunner) : INomadExecutionService
{
    public async Task<(string Name, string Hcl)> CreateJobAsync(IJobFileGeneratorOptions options, CancellationToken cancellationToken = default)
    {
        var result = await JobFileGenerator.GenerateJobAsync(options, cancellationToken);
        return (options.ServiceName, result);
    }

    public async Task<bool> ValidateJobAsync(string jobFileName, string nomadUrl, CancellationToken cancellationToken = default)
    {
        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument("job", string.Empty, quoteValue: false)
            .AppendArgument("validate", string.Empty, quoteValue: false)
            .AppendArgument("-address", nomadUrl, quoteValue: true)
            .AppendArgument(jobFileName, string.Empty, quoteValue: false);

        var result = await ExecuteNomadCliAsync("Validating job file...", argumentsBuilder);
        return result;
    }

    public async Task<bool> RunJobAsync(string jobFileName, string nomadUrl, CancellationToken cancellationToken = default)
    {
        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument("job", string.Empty, quoteValue: false)
            .AppendArgument("run", string.Empty, quoteValue: false)
            .AppendArgument("-detach", string.Empty, quoteValue: false)
            .AppendArgument("-address", nomadUrl, quoteValue: true)
            .AppendArgument(jobFileName, string.Empty, quoteValue: false);

        return await ExecuteNomadCliAsync("Running job file...", argumentsBuilder);
    }

    public async Task<bool> RemoveJobAsync(string jobName, string nomadUrl, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        return true;
    }

    private async Task<bool> ExecuteNomadCliAsync(string preCommandMessage, ArgumentsBuilder argumentsBuilder)
    {
        var result = await processRunner.ExecuteCommand(new()
        {
            Command = "nomad.exe",
            PreCommandMessage = preCommandMessage,
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = true
        });

        return result.ExitCode == 0;
    }
}
