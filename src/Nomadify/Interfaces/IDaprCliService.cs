using System.Threading.Tasks;
using Nomadify.Execution;

namespace Nomadify.Interfaces;

public interface IDaprCliService
{
    bool IsDaprCliInstalledOnMachine();
    Task<bool> IsDaprInstalledInCluster();
    Task<ProcessRunResult> InstallDaprInCluster();
    Task<ProcessRunResult> RemoveDaprFromCluster();
}
