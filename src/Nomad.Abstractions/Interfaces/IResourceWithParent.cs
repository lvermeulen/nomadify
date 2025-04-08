namespace Nomad.Abstractions.Interfaces;

public interface IResourceWithParent : IResource
{
    string? Parent { get; set; }
}
