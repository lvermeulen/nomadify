using System.Threading.Tasks;
using Nomad.Abstractions.Components.V0;
using Nomadify.Commands.Options;

namespace Nomadify.Interfaces;

public interface IContainerDetailsService
{
    Task<MsBuildContainerProperties> GetContainerDetails(string resourceName, ProjectResource? projectResource, ContainerOptions options);
}
