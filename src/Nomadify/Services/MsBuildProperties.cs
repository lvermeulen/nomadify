using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Nomadify.Commands.Options;

namespace Nomadify.Services;

[ExcludeFromCodeCoverage]
public sealed class MsBuildProperties<T>
    where T : IMsBuildProperties
{
    [JsonPropertyName("Properties")]
    public T? Properties { get; set; }
}
