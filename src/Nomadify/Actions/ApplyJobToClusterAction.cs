using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions;
using Nomad.Abstractions.Components;
using Nomad.JobGenerator;
using Nomadify.Extensions;
using Nomadify.Processors;
using Spectre.Console;

namespace Nomadify.Actions;

public sealed class ApplyJobToClusterAction(INomadExecutionService nomadExecutionService, IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Deployment to Cluster[/]");

        bool result;
        try
        {
            var fullOutputPath = Path.GetFullPath(CurrentState.OutputPath ?? string.Empty);

            result = await PerformStart(fullOutputPath);
        }
        catch (Exception e)
        {
            Logger.MarkupLine("[red](!)[/] Failed to run job in cluster.");
            Logger.MarkupLine($"[red](!)[/] Error: {e.Message}");
            return false;
        }

        return result;
    }

    public async Task<bool> PerformStart(string outputFolder)
    {
        var tasks = await GenerateTaskTemplatesAsync();

        var result = await RunTasks(outputFolder, tasks);
        if (result)
        {
            Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done:[/] Deployments successfully applied to cluster.'");
            return true;
        }

        Logger.MarkupLine($"[red]({NomadifyConstants.Warning}) Done:[/] Deployments NOT successfully applied to cluster.'");
        return false;
    }

    private async Task<bool> RunTasks(string outputFolder, Dictionary<string, ITaskTemplate> tasks)
    {
        var jobNameFromState = CurrentState.NomadJobName;
        ArgumentNullException.ThrowIfNull(jobNameFromState);

        // generate output
        var defaultValues = new Dictionary<string, object>();
        if (File.Exists(CurrentState.ReplacementValuesFile))
        {
            var defaultValuesFromFile = await File.ReadAllTextAsync(CurrentState.ReplacementValuesFile);
            defaultValues = JsonSerializer.Deserialize<Dictionary<string, object>>(defaultValuesFromFile);
        }

        var jobFileGeneratorOptions = new JobFileGeneratorOptions(jobNameFromState, tasks, defaultValues ?? [], CurrentState) as IJobFileGeneratorOptions;
        var (jobName, hcl) = await nomadExecutionService.CreateJobAsync(jobFileGeneratorOptions);
        var jobFileName = Path.Combine(outputFolder, $"{jobName}.job.hcl");

        // clean up output file
        if (File.Exists(jobFileName))
        {
            File.Delete(jobFileName);
        }

        // create output file
        await File.WriteAllTextAsync(jobFileName, hcl);

        // validate output file
        if (!await nomadExecutionService.ValidateJobAsync(jobFileName, CurrentState.NomadUrl!))
        {
            Logger.MarkupLine($"[red]({NomadifyConstants.Warning}) Job was not successfully validated [/].'");
            return false;
        }

        // run output file
        return await nomadExecutionService.RunJobAsync(jobFileName, CurrentState.NomadUrl!);
    }

    private async Task<Dictionary<string, ITaskTemplate>> GenerateTaskTemplatesAsync()
    {
        if (Services.GetRequiredKeyedService<IResourceProcessor>(NomadifyConstants.Project) is not ProjectProcessor)
        {
            throw new InvalidOperationException("Could not resolve project processor.");
        }

        var result = new Dictionary<string, ITaskTemplate>();

        //foreach (var resource in CurrentState.DaprProjectComponents.Where(x => x.Value is not null))
        //{
        //    //TODO: implement?
        //    //Logger.MarkupLine($"[red]({NomadifyConstants.Warning}) This is where we handle Dapr sidecar {resource.Key} in the tasks's template section.[/]");
        //}

        foreach (var resource in CurrentState.SelectedRawExecProjectComponents.Where(x => x.Value is not null))
        {
            result.Add(resource.Key, ProjectProcessor.CreateNomadRawExecTaskTemplate(resource, CurrentState));
        }

        foreach (var resource in CurrentState.SelectedDockerProjectComponents.Where(x => x.Value is not null))
        {
            result.Add(resource.Key, ProjectProcessor.CreateNomadDockerTaskTemplate(resource, CurrentState));
        }

        foreach (var resource in CurrentState.SelectedDockerfileProjectComponents.Where(x => x.Value is not null))
        {
            result.Add(resource.Key, ProjectProcessor.CreateNomadDockerTaskTemplate(resource, CurrentState));
        }

        return result;
    }
}
