using System.Threading;
using System.Threading.Tasks;

namespace Nomad.Abstractions;

public interface INomadExecutionService
{
    Task<(string Name, string Hcl)> CreateJobAsync(IJobFileGeneratorOptions options, CancellationToken cancellationToken = default);
    Task<bool> ValidateJobAsync(string jobFileName, string nomadUrl, CancellationToken cancellationToken = default);
    Task<bool> RunJobAsync(string jobFileName, string nomadUrl, CancellationToken cancellationToken = default);
    Task<bool> RemoveJobAsync(string jobName, string nomadUrl, CancellationToken cancellationToken = default);
}
