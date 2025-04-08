using System.Threading.Tasks;

namespace Nomadify.Interfaces;

public interface IProjectPropertyService
{
    Task<string?> GetProjectPropertiesAsync(string? projectPath, params string[] propertyNames);
}
