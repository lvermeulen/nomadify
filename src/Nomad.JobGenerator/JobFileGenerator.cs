using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Abstractions;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Helpers;

namespace Nomad.JobGenerator;

public static class JobFileGenerator
{
    public static async Task<string> GenerateJobAsync(IJobFileGeneratorOptions options, CancellationToken cancellationToken = default)
    {
        var optionsMap = options.ToKeyValues().MergeLeft(options.DefaultValues
            .Select(item => (item.Key, item.Value))
            .WithCorrectedBooleanValues()
            .ToDictionary());

        var stubble = new StubbleBuilder().Build();
        var result = await stubble.RenderAsync(Templates.JobTemplate, optionsMap);

        result = InsertPorts(result, options);
        result = await InsertTasks(result, options, stubble, cancellationToken);

        result += "\n}";

        return result.ToValidHcl();
    }

    private static string InsertPorts(string s, IJobFileGeneratorOptions options)
    {
        // replace "<! ports !>"
        var ports = GeneratePorts(options);
        return s.Replace("<! ports !>", string.Join("\n", ports));
    }

    private static async Task<string> InsertTasks(string s, IJobFileGeneratorOptions options, StubbleVisitorRenderer stubble, CancellationToken cancellationToken)
    {
        // replace "<! tasks !>"
        var tasks = await GenerateTaskTemplates(options.TaskTemplates.Values.ToList(), options, cancellationToken);

        if (options.HasDapr())
        {
            var optionsMap = options.ToKeyValues().MergeLeft(options.DefaultValues
                .Select(item => (item.Key, item.Value))
                .WithCorrectedBooleanValues()
                .ToDictionary());
            tasks.Add(await stubble.RenderAsync(Templates.DaprdTaskTemplate, optionsMap));
        }

        if (options.State.IncludeDashboard == true)
        {
            tasks.Add(await stubble.RenderAsync(Templates.AspireDashboardTaskTemplate, options.DefaultValues
                .Select(item => (item.Key, item.Value))
                .Where(x => x.Key.StartsWith("Group", StringComparison.OrdinalIgnoreCase))
                .WithCorrectedBooleanValues()
                .ToDictionary()));
        }

        return s.Replace("<! tasks !>", string.Join("", tasks));
    }

    private static IEnumerable<string> GeneratePorts(IJobFileGeneratorOptions options)
    {
        var result = options.TaskTemplates.Keys;
        var results = result.Select(x => $"port \"http-{x}\" {{}}\nport \"https-{x}\" {{}}").ToList();

        if (options.HasDapr())
        {
            results.Add("port \"http-daprd\" {}");
            results.Add("port \"http-daprd-app\" {}");
            results.Add("port \"http-daprd-grpc\" {}");
            results.Add("port \"http-daprd-rpc\" {}");
        }

        if (options.State.IncludeDashboard == true)
        {
            results.Add("port \"http-opentelemetry\" { to = 4317 }");
            results.Add("port \"http-aspire-dashboard\" { to = 18888 }");
        }

        return results;
    }

    private static async Task<IList<string>> GenerateTaskTemplates(IList<ITaskTemplate> taskTemplates, IJobFileGeneratorOptions options, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return [];
        }

        var results = new List<string>();
        var stubble = new StubbleBuilder().Build();

        foreach (var taskTemplate in taskTemplates)
        {
            var optionsMap = taskTemplate.ToKeyValues().MergeLeft(options.DefaultValues
                .Select(item => (item.Key, item.Value))
                .WithCorrectedBooleanValues()
                .ToDictionary());

            var result = (await stubble.RenderAsync(Templates.TaskTemplate, optionsMap))
                .Replace("<! env-vars !>", string.Join("\n", (optionsMap["GroupTaskEnv"] as List<string>)!));

            results.Add(result);
        }

        return results;
    }
}
