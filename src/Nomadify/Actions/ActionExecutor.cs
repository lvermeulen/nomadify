using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nomad.Abstractions.Components;
using Nomadify.Execution.Exceptions;
using Spectre.Console;

namespace Nomadify.Actions;

public class ActionExecutor(IAnsiConsole console, IServiceProvider serviceProvider)
{
    public static ActionExecutor CreateInstance(IServiceProvider serviceProvider) =>
        new(serviceProvider.GetRequiredService<IAnsiConsole>(), serviceProvider);

    private readonly Queue<ExecutionAction> _actionQueue = new();

    public ActionExecutor QueueAction(string actionKey, Func<Task>? onFailure = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(actionKey);
        _actionQueue.Enqueue(new(actionKey, onFailure));

        return this;
    }

    public async Task<int> ExecuteCommandsAsync()
    {
        while (_actionQueue.Count > 0)
        {
            var executionAction = _actionQueue.Dequeue();
            var action = serviceProvider.GetRequiredKeyedService<IAction>(executionAction.ActionKey);

            try
            {
                var success = await action.ExecuteAsync();
                if (success)
                {
                    continue;
                }

                await HandleActionFailure(executionAction.OnFailure);
                return 1;
            }
            catch (ExitCodeException exitException)
            {
                // Do nothing - the action is planned, and will skip the rest of the queue, returning the exit code.
                console.MarkupLine($"[red bold]({exitException.ExitCode}): Nomadify will now exit.[/]");
                return exitException.ExitCode;
            }
            catch (Exception ex)
            {
                console.MarkupLine($"[red bold]Error executing action [blue]'{executionAction.ActionKey}'[/]:[/]");
                console.WriteException(ex);
                await HandleActionFailure(executionAction.OnFailure);
                return 1;
            }
        }

        console.WriteLine();
        console.MarkupLine($"[bold] {NomadifyConstants.Rocket} Execution Completed {NomadifyConstants.Rocket}[/]");
        return 0;
    }

    private static Task HandleActionFailure(Func<Task>? onFailure = null) =>
        onFailure != null ? onFailure() : Task.CompletedTask;

    private sealed record ExecutionAction(string ActionKey, Func<Task>? OnFailure = null);
}
