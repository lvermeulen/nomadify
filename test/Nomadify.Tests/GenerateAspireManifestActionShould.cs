using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomadify.Actions;
using Nomadify.Execution;
using NSubstitute;
using NSubstitute.ClearExtensions;
using Xunit;
using Xunit.Abstractions;

namespace Nomadify.Tests;

public class GenerateAspireManifestActionShould(NomadifyFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<NomadifyFixture>
{
    [Fact]
    public async Task Execute()
    {
        var console = fixture.Console;

        var state = fixture.CreateNomadifyState(projectPath: @"C:\iorepo\DifaTemplate\src\Weather.AppHost");
        var serviceProvider = fixture.CreateServiceProvider(state, console);
        var action = fixture.GetActionUnderTest<GenerateAspireManifestAction>(serviceProvider);

        var executionService = serviceProvider.GetRequiredService<IProcessRunner>();
        executionService.ClearSubstitute();
        executionService.ExecuteCommand(Arg.Is<ProcessRunOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(new ProcessRunResult(true, string.Empty, string.Empty, 0));

        var result = await action.ExecuteAsync();
        Assert.True(result);

        await executionService.Received(1).ExecuteCommand(
            Arg.Is<ProcessRunOptions>(options => options.Command != null && options.ArgumentsBuilder != null));

        testOutputHelper.WriteLine($"Executed {action.GetType().Name}.");
    }
}
