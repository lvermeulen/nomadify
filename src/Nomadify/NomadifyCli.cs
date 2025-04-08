using System;
using System.CommandLine;
using System.IO;
using Nomad.Abstractions.Components;
using Nomadify.Commands.Apply;
using Nomadify.Commands.Generate;
using Spectre.Console;

namespace Nomadify;

internal class NomadifyCli : RootCommand
{
    internal static void WelcomeMessage()
    {
        if (ShouldSkipLogo())
        {
            return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText("Nomadify").Color(Color.Pink1));
        AnsiConsole.MarkupLine("[bold lime]Handle deployments of a .NET Aspire AppHost to HashiCorp Nomad[/]");
        AnsiConsole.WriteLine();
    }

    private static bool ShouldSkipLogo()
    {
        var appDataFolder = GetAppDataFolder();
        var skipLogoFile = Path.Combine(appDataFolder, NomadifyConstants.LogoDisabledFile);
        var skipLogo = Environment.GetEnvironmentVariable("NOMADIFY_NO_LOGO");

        return !string.IsNullOrEmpty(skipLogo) || File.Exists(skipLogoFile);
    }

    private static string GetAppDataFolder()
    {
        var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), NomadifyConstants.AppDataFolder);

        if (!Directory.Exists(appDataFolder))
        {
            Directory.CreateDirectory(appDataFolder);
        }

        return appDataFolder;
    }

    public NomadifyCli()
    {
        AddCommand(new GenerateCommand());
        AddCommand(new ApplyCommand());
    }
}