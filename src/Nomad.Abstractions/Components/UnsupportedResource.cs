using System.Diagnostics.CodeAnalysis;
using Nomad.Abstractions.Components.V0;

namespace Nomad.Abstractions.Components;

[ExcludeFromCodeCoverage]
public class UnsupportedResource : Resource
{
    public UnsupportedResource() => Type = "unknown.resource.v0";
}
