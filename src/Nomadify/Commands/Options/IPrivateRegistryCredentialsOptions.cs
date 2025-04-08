namespace Nomadify.Commands.Options;

public interface IPrivateRegistryCredentialsOptions
{
    string? PrivateRegistryUrl { get; set; }
    string? PrivateRegistryUsername { get; set; }
    string? PrivateRegistryPassword { get; set; }
    string? PrivateRegistryEmail { get; set; }
    bool? WithPrivateRegistry { get; set; }
}
