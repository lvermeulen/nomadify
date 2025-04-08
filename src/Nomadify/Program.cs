using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Nomadify;
using Nomadify.Extensions;

NomadifyCli.WelcomeMessage();

return await new CommandLineBuilder(new NomadifyCli())
    .UseDefaults()
    .UseDependencyInjection(services =>
    {
        services
            .AddLogging()
            .AddNomadify();
    })
    .Build()
    .InvokeAsync(args);
