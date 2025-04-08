using System;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomadify.Extensions;
using Spectre.Console;

namespace Nomadify.Actions;

public class AskPrivateRegistryCredentialsAction(IServiceProvider serviceProvider)
    : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (PreviousStateWasRestored())
        {
            return Task.FromResult(true);
        }

        Logger.WriteRuler("[purple]Handling private registry credentials[/]");

        if (!CurrentState.WithPrivateRegistry.GetValueOrDefault())
        {
            return Task.FromResult(true);
        }

        Logger.MarkupLine("Ensuring private registry credentials are set so that we can produce an image pull secret.");

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryUrl))
        {
            CurrentState.PrivateRegistryUrl = Logger.Prompt(
                new TextPrompt<string>("Enter registry url:")
                    .PromptStyle("blue")
                    .Validate(url => !string.IsNullOrEmpty(url), "Url required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryUsername))
        {
            CurrentState.PrivateRegistryUsername = Logger.Prompt(
                new TextPrompt<string>("Enter registry username:")
                    .PromptStyle("blue")
                    .Validate(username => !string.IsNullOrEmpty(username), "Username is required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryPassword))
        {
            CurrentState.PrivateRegistryPassword = Logger.Prompt(
                new TextPrompt<string>("Enter registry password:")
                    .Secret()
                    .PromptStyle("red")
                    .Validate(password => !string.IsNullOrEmpty(password), "Password is required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryEmail))
        {
            CurrentState.PrivateRegistryEmail = Logger.Prompt(
                new TextPrompt<string>("Enter registry email:")
                    .PromptStyle("blue")
                    .Validate(email => !string.IsNullOrEmpty(email), "Email is required and cannot be empty."));
        }

        Logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] Setting private registry credentials for image pull secret.");

        return Task.FromResult(true);
    }
}
