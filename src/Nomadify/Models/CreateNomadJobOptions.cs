namespace Nomadify.Models;

public sealed class CreateNomadJobOptions : BaseNomadCreateOptions
{
    public bool EncodeSecrets { get; set; } = true;
}