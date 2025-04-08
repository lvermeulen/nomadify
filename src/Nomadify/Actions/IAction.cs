using System.Threading.Tasks;

namespace Nomadify.Actions;

public interface IAction
{
    Task<bool> ExecuteAsync();
}
