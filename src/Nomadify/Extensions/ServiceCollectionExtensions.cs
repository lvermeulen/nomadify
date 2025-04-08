using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions;
using Nomad.Abstractions.Components;
using Nomad.Cli;
using Nomadify.Actions;
using Nomadify.Execution;
using Nomadify.Interfaces;
using Nomadify.Processors;
using Nomadify.Services;
using Spectre.Console;

namespace Nomadify.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddNomadify(this IServiceCollection services) => services
        .AddSpectreConsole()
        .AddNomadifyState()
        .AddNomadifyServices()
        .AddNomadifyActions()
        .AddNomadifyProcessors()
        .AddPlaceholderTransformation();

    private static IServiceCollection AddSpectreConsole(this IServiceCollection services) => services.AddSingleton(AnsiConsole.Console);

    public static IServiceCollection AddNomadifyState(this IServiceCollection services) => services.AddSingleton<NomadifyState>();

    public static IServiceCollection AddNomadifyActions(this IServiceCollection services) => services
        .RegisterAction<InitializeConfigurationAction>()
        .RegisterAction<LoadConfigurationAction>()
        .RegisterAction<AskImagePullPolicyAction>()
        .RegisterAction<BuildAndPublishRawExecAction>()
        .RegisterAction<BuildAndPushContainersFromProjectsAction>()
        .RegisterAction<BuildAndPushContainersFromDockerfilesAction>()
        .RegisterAction<PopulateContainerDetailsForProjectsAction>()
        .RegisterAction<GenerateAspireManifestAction>()
        .RegisterAction<LoadAspireManifestAction>()
        .RegisterAction<ApplyJobToClusterAction>()
        .RegisterAction<RemoveJobFromClusterAction>()
        .RegisterAction<SubstituteValuesAspireManifestAction>()
        .RegisterAction<ApplyDaprAnnotationsAction>()
        .RegisterAction<PopulateInputsAction>()
        .RegisterAction<AskPrivateRegistryCredentialsAction>()
        .RegisterAction<IncludeAspireDashboardAction>();

    private static IServiceCollection RegisterAction<TImplementation>(this IServiceCollection services)
        where TImplementation : class, IAction => services.AddKeyedSingleton<IAction, TImplementation>(typeof(TImplementation).Name);

    private static IServiceCollection AddNomadifyServices(this IServiceCollection services) => services
        .AddShellExecution()
        .AddAspireManifestSupport()
        .AddNomadifyConfigurationSupport()
        .AddContainerSupport()
        .AddDaprSupport()
        .AddNomadSupport()
        .AddStateManagement();

    private static IServiceCollection AddAspireManifestSupport(this IServiceCollection services) => services
        .AddSingleton<AspireManifestCompositionService>()
        .AddSingleton<IManifestFileParserService, ManifestFileParserService>();

    private static IServiceCollection AddDaprSupport(this IServiceCollection services) => services
        .AddSingleton<IDaprCliService, DaprCliService>();

    private static IServiceCollection AddNomadSupport(this IServiceCollection services) => services
        .AddSingleton<INomadExecutionService, NomadCliService>();

    private static IServiceCollection AddNomadifyConfigurationSupport(this IServiceCollection services) =>
        services
            .AddSingleton<INomadifyConfigurationService, NomadifyConfigurationService>();

    private static IServiceCollection AddStateManagement(this IServiceCollection services) => services
        .AddSingleton<IStateService, StateService>();

    private static IServiceCollection AddShellExecution(this IServiceCollection services) => services
        .AddSingleton<IProcessRunner, ProcessRunner>();

    private static IServiceCollection AddContainerSupport(this IServiceCollection services) => services
        .AddSingleton<IProjectPropertyService, ProjectPropertyService>()
        .AddSingleton<IContainerDetailsService, ContainerDetailsService>();

    private static IServiceCollection AddNomadifyProcessors(this IServiceCollection services) => services
        .AddSingleton<ContainerProcessor>()
        .RegisterProcessor<ProjectProcessor>(NomadifyConstants.Project)
        .RegisterProcessor<DaprProcessor>(NomadifyConstants.Dapr)
        .RegisterProcessor<DaprComponentProcessor>(NomadifyConstants.DaprComponent)
        .RegisterProcessor<DockerfileProcessor>(NomadifyConstants.Dockerfile)
        .RegisterProcessor<ContainerProcessor>(NomadifyConstants.Container)
        .RegisterProcessor<ParameterProcessor>(NomadifyConstants.Parameter)
        .RegisterProcessor<ValueProcessor>(NomadifyConstants.Value);

    private static IServiceCollection AddPlaceholderTransformation(this IServiceCollection services) => services
        .AddSingleton<IResourceExpressionProcessor, ResourceExpressionProcessor>()
        .AddSingleton<IJsonExpressionProcessor, JsonExpressionProcessor>()
        .AddSingleton<IBindingProcessor, BindingProcessor>();


    private static IServiceCollection RegisterProcessor<TImplementation>(this IServiceCollection services, string key)
        where TImplementation : class, IResourceProcessor => services.AddKeyedSingleton<IResourceProcessor, TImplementation>(key);
}
