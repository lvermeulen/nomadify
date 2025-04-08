using System;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nomadify.Extensions;

internal static class CommandLineBuilderExtensions
{
    public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<ServiceCollection> configureServices) => builder.UseDependencyInjection((_, services) => configureServices(services));

    private static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<InvocationContext, ServiceCollection> configureServices) => builder.AddMiddleware((context, next) =>
    {
        var services = new ServiceCollection();
        configureServices(context, services);

        services.TryAddSingleton(context.Console);

        context.BindingContext.AddService<IServiceCollection>(_ => services);

        return next(context);
    });
}
