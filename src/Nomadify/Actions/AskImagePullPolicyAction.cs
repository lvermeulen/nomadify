using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Actions;

public class AskImagePullPolicyAction(IServiceProvider serviceProvider)
    : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (PreviousStateWasRestored())
        {
            return Task.FromResult(true);
        }

        Logger.WriteRuler("[purple]Handle Image Pull Policy[/]");

        if (!string.IsNullOrEmpty(CurrentState.ImagePullPolicy))
        {
            return Task.FromResult(true);
        }

        var choices = new List<string>
        {
            "IfNotPresent",
            "Always",
            "Never",
        };

        var choice = Logger.Prompt(
            new SelectionPrompt<string>()
                .Title("Select image pull policy for manifests")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                .AddChoices(choices));

        CurrentState.ImagePullPolicy = choice;

        return Task.FromResult(true);
    }
}
