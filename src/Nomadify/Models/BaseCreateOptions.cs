using Nomad.Abstractions;
using Nomad.Abstractions.Components.V0;

namespace Nomadify.Models;

public abstract class BaseCreateOptions
{
    public (string Key, Resource Value) Resource { get; set; }

    public bool? WithDashboard { get; set; }

    public bool? WithPrivateRegistry { get; set; }

    public NomadifyState? CurrentState { get; set; }
}