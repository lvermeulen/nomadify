using System.Collections.Generic;
using Volume = Nomad.Abstractions.Components.V0.Volume;

namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithVolumes
{
    List<Volume>? Volumes { get; set; }
}
