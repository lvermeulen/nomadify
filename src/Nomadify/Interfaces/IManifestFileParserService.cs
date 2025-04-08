using System.Collections.Generic;
using Nomad.Abstractions.Components.V0;

namespace Nomadify.Interfaces;

public interface IManifestFileParserService
{
    Dictionary<string, Resource> LoadAndParseAspireManifest(string? manifestFile);
}
