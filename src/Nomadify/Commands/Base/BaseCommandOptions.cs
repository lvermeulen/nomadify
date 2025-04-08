using System.Diagnostics.CodeAnalysis;
using Nomadify.Commands.Options;

namespace Nomadify.Commands.Base;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptions : ICommandOptions
{
    public string? ProjectPath { get; set; }
}
