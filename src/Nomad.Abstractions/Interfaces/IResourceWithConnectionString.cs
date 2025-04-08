namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithConnectionString : IResource
{
    string? ConnectionString { get; set; }
}
