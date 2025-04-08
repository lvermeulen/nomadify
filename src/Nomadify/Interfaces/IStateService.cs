using System.Threading.Tasks;
using Nomad.Abstractions;

namespace Nomadify.Interfaces;

public interface IStateService
{
    Task SaveState(NomadifyState state);
    Task RestoreState(NomadifyState state);
}
