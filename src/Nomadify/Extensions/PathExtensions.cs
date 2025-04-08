using System;
using System.IO;
using Nomad.Abstractions.Components;

namespace Nomadify.Extensions;

public static class PathExtensions
{
    public static string NormalizePath(this string pathToTarget)
    {
        if (string.IsNullOrEmpty(pathToTarget))
        {
            return Directory.GetCurrentDirectory();
        }

        if (!pathToTarget.StartsWith('.'))
        {
            return pathToTarget;
        }

        var currentDirectory = Directory.GetCurrentDirectory();
        var normalizedProjectPath = pathToTarget.Replace('\\', Path.DirectorySeparatorChar);

        return Path.Combine(currentDirectory, normalizedProjectPath);
    }

    public static string GetFullPath(this string path)
    {
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }

        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return path.StartsWith($"~{Path.DirectorySeparatorChar}")
            // The path is relative to the user's home directory
            ? Path.Combine(homePath, path.TrimStart('~', Path.DirectorySeparatorChar))
            // The path is relative to the current working directory
            : Path.GetFullPath(path);
    }

    public static string NomadifyAppDataFolder()
    {
        var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), NomadifyConstants.AppDataFolder);
        if (!Directory.Exists(appDataFolder))
        {
            Directory.CreateDirectory(appDataFolder);
        }

        return appDataFolder;
    }
}
