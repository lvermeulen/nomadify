using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;

namespace Nomadify.Execution;

public interface IProcessRunner
{
    Task<ProcessRunResult> ExecuteCommand(ProcessRunOptions options);
    Task<bool> ExecuteCommandWithEnvironmentNoOutput(string command, ArgumentsBuilder argumentsBuilder, IReadOnlyDictionary<string, string?> environmentVariables);
    IResult<string> IsCommandAvailable(string commandName);
}
