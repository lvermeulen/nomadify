using Microsoft.Extensions.Logging;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Container;
using Nomadify.Services;
using Nomadify.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Nomadify.Tests;

public class ManifestFileParserServiceShould(NomadifyFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<NomadifyFixture>
{
    private readonly ManifestFileParserService _service = new(fixture.ServiceProvider, new Logger<ManifestFileParserService>(fixture.LoggerFactory));

    [Fact]
    public void ParseManifestFile()
    {
        var resources = _service.LoadAndParseAspireManifest("manifest.json");

        foreach (var resource in resources.Values)
        {
            testOutputHelper.WriteLine($"Name: {resource.Name}");
            switch (resource)
            {
                case ProjectResource projectResource:
                {
                    testOutputHelper.Output(projectResource);

                    break;
                }
                case ContainerResource containerResource:
                {
                    testOutputHelper.Output(containerResource);

                    break;
                }
                case BicepResource bicepResource:
                {
                    testOutputHelper.Output(bicepResource);

                    break;
                }
            }

            testOutputHelper.WriteLine("\n");
        }

        Assert.NotNull(resources);
    }
}
