using Nomad.Abstractions.Components.V0.Container;
using Nomad.Abstractions.Components.V0.Dapr;
using Nomad.Abstractions.Components.V0.Parameters;
using Nomad.Abstractions.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Nomad.Abstractions.Components.V0;

[ExcludeFromCodeCoverage]
[JsonPolymorphic]
[JsonDerivedType(typeof(ProjectResource), typeDiscriminator: "aspire.project")]
[JsonDerivedType(typeof(DockerfileResource), typeDiscriminator: "aspire.dockerfile")]
[JsonDerivedType(typeof(ContainerResource), typeDiscriminator: "aspire.container")]
[JsonDerivedType(typeof(DaprResource), typeDiscriminator: "aspire.dapr")]
[JsonDerivedType(typeof(DaprComponentResource), typeDiscriminator: "aspire.daprcomponent")]
[JsonDerivedType(typeof(ParameterResource), typeDiscriminator: "aspire.parameter")]
[JsonDerivedType(typeof(ValueResource), typeDiscriminator: "aspire.value")]
public abstract class Resource : IResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
