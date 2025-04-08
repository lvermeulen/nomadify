namespace Nomad.Abstractions.Interfaces;

public interface IResource
{
    string Name { get; set; }
    string Type { get; set; }
}
