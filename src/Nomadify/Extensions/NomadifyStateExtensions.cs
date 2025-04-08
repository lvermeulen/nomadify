using System;
using System.Linq;
using System.Reflection;
using Nomad.Abstractions;
using Nomadify.Commands.Options;

namespace Nomadify.Extensions;

public static class NomadifyStateExtensions
{
    public static void ReplaceCurrentStateWithPreviousState(this NomadifyState currentState, NomadifyState previousState, bool restoreAllRestorable)
    {
        ArgumentNullException.ThrowIfNull(currentState);
        ArgumentNullException.ThrowIfNull(previousState);

        if (restoreAllRestorable)
        {
            var properties = typeof(NomadifyState).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => Attribute.IsDefined(p, typeof(RestorableStatePropertyAttribute)));

            foreach (var property in properties)
            {
                var previousStatePropertyValue = property.GetValue(previousState);
                if (previousStatePropertyValue is null)
                {
                    continue;
                }

                property.SetValue(currentState, previousStatePropertyValue);
            }

            currentState.StateWasLoadedFromPrevious = true;
        }
    }

    public static void PopulateStateFromOptions<TOptions>(this NomadifyState state, TOptions options)
        where TOptions : ICommandOptions
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(options);

        var properties = typeof(TOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(options);
            if (propertyValue is null)
            {
                continue;
            }

            var stateProperty = state.GetType().GetProperty(property.Name);

            stateProperty?.SetValue(state, propertyValue);
        }
    }
}
