using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ardalis.SmartEnum.SystemTextJson;
using Nomad.Abstractions;
using Nomad.Abstractions.Components;
using Nomadify.Extensions;
using Nomadify.Interfaces;
using Nomadify.Models;
using Spectre.Console;

namespace Nomadify.Services;

public class StateService(IAnsiConsole logger) : IStateService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        IncludeFields = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters =
        {
            new SmartEnumNameConverter<ExistingSecretsType, string>()
        }
    };

    public async Task SaveState(NomadifyState state)
    {
        var stateFile = Path.Combine(Directory.GetCurrentDirectory(), NomadifyConstants.StateFileName);
        var stateAsJson = JsonSerializer.Serialize(state, _jsonSerializerOptions);

        await File.WriteAllTextAsync(stateFile, stateAsJson);
    }

    public async Task RestoreState(NomadifyState state)
    {
        logger.WriteRuler("[purple]Handling Nomadify State[/]");

        var stateFile = Path.Combine(Directory.GetCurrentDirectory(), NomadifyConstants.StateFileName);
        await RestoreAllState(state, stateFile);
    }

    private async Task RestoreAllState(NomadifyState state, string stateFile)
    {
        await RestoreState(state, stateFile, true);
        LogAllStateReloaded(stateFile);
    }

    private async Task RestoreState(NomadifyState state, string stateFile, bool shouldUseAllPreviousStateValues)
    {
        var stateAsJson = await File.ReadAllTextAsync(stateFile);
        var previousState = JsonSerializer.Deserialize<NomadifyState>(stateAsJson, _jsonSerializerOptions);
        state.ReplaceCurrentStateWithPreviousState(previousState!, shouldUseAllPreviousStateValues);
        state.UseAllPreviousStateValues = shouldUseAllPreviousStateValues;
    }

    private void LogAllStateReloaded(string stateFile) =>
        logger.MarkupLine($"[green]({NomadifyConstants.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]. Will run without re-prompting for values.");
}
