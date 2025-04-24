using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nomad.Abstractions;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Dapr;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Helpers;

namespace Nomad.JobGenerator;

public static class JobFileGenerator
{
    public static async Task<string> GenerateJobAsync(IJobFileGeneratorOptions options, CancellationToken cancellationToken = default)
    {
        var stubble = new StubbleBuilder().Build();
        var result = await stubble.RenderAsync(Templates.JobTemplate, options.ToKeyValues().MergeLeft(options.DefaultValues
            .Select(item => (item.Key, item.Value))
            .WithCorrectedBooleanValues()
            .ToDictionary()));

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
            foreach (var daprComponent in options.State.DaprComponents)
            {
                tasks.Add(await GenerateDaprTask(daprComponent.Value, options, stubble, cancellationToken));
            }
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

    private static async Task<string> GenerateDaprTask(Resource? resource, IJobFileGeneratorOptions options, StubbleVisitorRenderer stubble, CancellationToken cancellationToken)
    {
        if (resource is not DaprComponentResource daprComponent)
        {
            return string.Empty;
        }

        var map = options.ToKeyValues();
        map["DaprComponentResourceName"] = daprComponent.Name;
        var optionsMap = map.MergeLeft(options.DefaultValues
            .Select(item => (item.Key, item.Value))
            .WithCorrectedBooleanValues()
            .ToDictionary());
        var result = await stubble.RenderAsync(Templates.DaprdSidecarTemplate, optionsMap);
        result = result.Replace("<! templates !>", await GenerateDaprdTemplates(options, optionsMap, cancellationToken));

        return result;
    }

    private static async Task<string?> GenerateDaprdTemplates(IJobFileGeneratorOptions options, Dictionary<string, object>? optionsMap, CancellationToken cancellationToken)
    {
        var stubble = new StubbleBuilder().Build();
        var result = await stubble.RenderAsync(Templates.DaprdConfigYamlTemplate, optionsMap);

        var results = new List<string>
        {
            result
        };

        // generate component yaml files
        foreach (var resource in options.State.DaprComponents)
        {
            if (resource.Value is DaprComponentResource daprComponent)
            {
                var daprFilename = $"{daprComponent.Name}.yaml";
                var file = Path.Combine(options.State.OutputPath ?? string.Empty, "dapr", daprFilename);
                var contents = await File.ReadAllTextAsync(file, cancellationToken);

                results.Add(@$"
  template {{
    data        = <<EOF
{contents}
EOF

    destination = ""local/.dapr/components/{daprFilename}""
  }}");
            }
        }

        return string.Join("\n", results);
    }

    private static IEnumerable<string> GeneratePorts(IJobFileGeneratorOptions options)
    {
        var result = options.TaskTemplates.Keys;
        
        // generate task ports
        var results = result.Select(x => $"port \"http-{x}\" {{}}\nport \"https-{x}\" {{}}").ToList();

        if (options.HasDapr())
        {
            // generate Dapr ports
            foreach (var daprComponent in options.State.DaprComponents)
            {
                if (daprComponent.Value is DaprComponentResource daprComponentResource)
                {
                    results.Add($"port \"http-daprd-{daprComponentResource.Name}\" {{}}");
                    results.Add($"port \"http-daprd-grpc-{daprComponentResource.Name}\" {{}}");
                    results.Add($"port \"http-daprd-http-{daprComponentResource.Name}\" {{}}");
                    results.Add($"port \"http-daprd-metrics-{daprComponentResource.Name}\" {{}}");
                }
            }
        }

        if (options.State.IncludeDashboard == true)
        {
            // generate Aspire dashboard ports
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
