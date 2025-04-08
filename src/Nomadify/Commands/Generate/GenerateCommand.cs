using System.CommandLine;
using System.CommandLine.IO;
using Nomadify.Commands.Base;
using Nomadify.Commands.Options;

namespace Nomadify.Commands.Generate;

public class GenerateCommand : BaseCommand<GenerateOptions, GenerateCommandHandler>
{
    public IConsole Console { get; set; } = new TestConsole();

    public GenerateCommand()
        : base(name: "generate", "Generates manifest file")
    {
        AddOption(ProjectPathOption.Instance);
        AddOption(AspireManifestOption.Instance);
        AddOption(OutputPathOption.Instance);
        AddOption(NoBuildOption.Instance);
        AddOption(CompressionKindOption.Instance);
        AddOption(ContainerBuilderOption.Instance);
        AddOption(ContainerImageTagOption.Instance);
        AddOption(ContainerRegistryOption.Instance);
        AddOption(ContainerRepositoryPrefixOption.Instance);
        AddOption(ImagePullPolicyOption.Instance);
        AddOption(RuntimeIdentifierOption.Instance);
        AddOption(PrivateRegistryOption.Instance);
        AddOption(PrivateRegistryUrlOption.Instance);
        AddOption(PrivateRegistryUsernameOption.Instance);
        AddOption(PrivateRegistryPasswordOption.Instance);
        AddOption(PrivateRegistryEmailOption.Instance);
        AddOption(IncludeDashboardOption.Instance);
    }
}
