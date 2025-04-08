﻿using System.Collections.Generic;

namespace Nomadify.Execution;

public class ArgumentsBuilder
{
    private readonly Dictionary<string, List<string>> _arguments = [];

    public static ArgumentsBuilder Create() => new();

    public ArgumentsBuilder Clear()
    {
        _arguments.Clear();

        return this;
    }

    public ArgumentsBuilder AppendArgument(string argument, string? newValue, bool allowDuplicates = false, bool quoteValue = true)
    {
        if (newValue is null)
        {
            return this;
        }

        if (!_arguments.TryGetValue(argument, out var value))
        {
            value = quoteValue ? [$"\"{newValue}\""] : [newValue];
            _arguments[argument] = value;

            return this;
        }

        if (allowDuplicates)
        {
            value.Add(quoteValue ? $"\"{newValue}\"" : newValue);
        }

        return this;
    }

    public string RenderArguments(char propertyKeySeparator = ' ')
    {
        var renderedArguments = new List<string>();

        foreach (var arg in _arguments)
        {
            foreach (var value in arg.Value)
            {
                if (value == string.Empty)
                {
                    renderedArguments.Add(arg.Key);
                    continue;
                }

                if (arg.Key.StartsWith("-p"))
                {
                    renderedArguments.Add($"{arg.Key}={value}");
                    continue;
                }

                renderedArguments.Add($"{arg.Key}{propertyKeySeparator}{value}");
            }
        }

        return string.Join(" ", renderedArguments);
    }

    public override string ToString() => string.Join(" ", _arguments);
}
