using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nomad.Abstractions;
using Nomadify.Actions;
using Nomadify.Execution;
using Nomadify.Extensions;
using NSubstitute;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Nomadify.Tests;

public class NomadifyFixture
{
    public const string DefaultProjectPath = "/some-path";
    public const string DefaultContainerRegistry = "test-registry";
    public const string DefaultContainerImageTag = "test-tag";

    public ILoggerFactory LoggerFactory { get; set; }
    public IServiceProvider ServiceProvider { get; set; }

    public IAnsiConsole Console
    {
        get
        {
            var console = new TestConsole()
            {
                Profile = { Capabilities = { Interactive = true } }
            };
            console.Input.PushTextWithEnter("y");
            console.Input.PushKey(ConsoleKey.Enter);

            return console;
        }
    }

    public NomadifyFixture()
    {
        LoggerFactory = new NullLoggerFactory();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddNomadify();
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public TAction GetActionUnderTest<TAction>(IServiceProvider serviceProvider) where TAction : class, IAction =>
        (serviceProvider.GetRequiredKeyedService<IAction>(typeof(TAction).Name) as TAction)!;

    public IServiceProvider CreateServiceProvider(NomadifyState state, IAnsiConsole? testConsole = null)
    {
        var console = testConsole ?? new TestConsole();

        var services = new ServiceCollection();
        services.AddNomadify();

        services.RemoveAll<ProcessRunner>();
        services.AddSingleton(Substitute.For<IProcessRunner>());

        services.RemoveAll<IAnsiConsole>();
        services.AddSingleton(console);

        services.RemoveAll<NomadifyState>();
        services.AddSingleton(state);

        return services.BuildServiceProvider();
    }

    public NomadifyState CreateNomadifyState(bool nonInteractive = false,
        string? projectPath = DefaultProjectPath,
        string? containerRegistry = DefaultContainerRegistry,
        string? containerPrefix = null,
        string? containerBuilder = null,
        string? containerImageTag = DefaultContainerImageTag,
        string? aspireManifest = null)
    {
        var state = new NomadifyState
        {
            OutputPath = "./",
            NomadJobName = "test-job",
            ReplacementValuesFile = "defaultValues.json",
            ContainerRegistry = containerRegistry,
            ContainerImageTags = [containerImageTag ?? DefaultContainerImageTag],
            ContainerBuilder = containerBuilder,
            ContainerRepositoryPrefix = containerPrefix
        };

        if (!string.IsNullOrEmpty(projectPath))
        {
            state.ProjectPath = projectPath;
        }

        if (!string.IsNullOrEmpty(aspireManifest))
        {
            state.AspireManifest = aspireManifest;
        }

        return state;
    }
}
