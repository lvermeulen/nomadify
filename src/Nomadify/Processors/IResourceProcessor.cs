using System.Text.Json;
using Nomad.Abstractions.Components.V0;

namespace Nomadify.Processors;

public interface IResourceProcessor
{
    Resource? Deserialize(ref Utf8JsonReader reader);
}
