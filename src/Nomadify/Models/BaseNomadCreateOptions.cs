namespace Nomadify.Models;

public abstract class BaseNomadCreateOptions : BaseCreateOptions
{
    public required string ImagePullPolicy { get; set; }
}