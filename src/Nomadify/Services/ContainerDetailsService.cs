using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Nomad.Abstractions.Components;
using Nomad.Abstractions.Components.V0;
using Nomadify.Commands.Options;
using Nomadify.Interfaces;

namespace Nomadify.Services;

public class ContainerDetailsService(IProjectPropertyService propertyService) : IContainerDetailsService
{
    private static readonly StringBuilder s_imageBuilder = new();

    public async Task<MsBuildContainerProperties> GetContainerDetails(string resourceName, ProjectResource? projectResource, ContainerOptions options)
    {
        ArgumentNullException.ThrowIfNull(projectResource);

        var containerPropertiesJson = await propertyService.GetProjectPropertiesAsync(
            projectResource.Path,
            NomadifyConstants.ContainerRegistry,
            NomadifyConstants.ContainerRepository,
            NomadifyConstants.ContainerImageName,
            NomadifyConstants.ContainerImageTag);

        var msBuildProperties = JsonSerializer.Deserialize<MsBuildProperties<MsBuildContainerProperties>>(containerPropertiesJson ?? "{}");
        ArgumentNullException.ThrowIfNull(msBuildProperties);
        ArgumentNullException.ThrowIfNull(msBuildProperties.Properties);

        // Exit app if container registry is empty. We need it.
        EnsureContainerRegistryIsNotEmpty(msBuildProperties.Properties, options.Registry);

        // Fallback to service name if image name is not provided from anywhere. (imageName is deprecated using repository like it says to).
        if (string.IsNullOrEmpty(msBuildProperties.Properties.ContainerRepository) && string.IsNullOrEmpty(msBuildProperties.Properties.ContainerImageName))
        {
            msBuildProperties.Properties.ContainerRepository = resourceName;
        }

        // Fallback to latest tag if tag not specified.
        HandleTags(msBuildProperties, options.Tags);

        msBuildProperties.Properties.FullContainerImage = GetFullImage(msBuildProperties.Properties, options.Prefix);

        return msBuildProperties.Properties;
    }

    private static string GetFullImage(MsBuildContainerProperties containerDetails, string? containerPrefix)
    {
        s_imageBuilder.Clear();

        HandleRegistry(containerDetails);
        HandleRepository(containerDetails, containerPrefix!);
        HandleImage(containerDetails);
        HandleTag(containerDetails);

        return s_imageBuilder.ToString().Trim('/');
    }

    private static void HandleTag(MsBuildContainerProperties containerDetails) => s_imageBuilder.Append($":{containerDetails.ContainerImageTag!.Split(";").FirstOrDefault()}");

    private static void HandleImage(MsBuildContainerProperties containerDetails)
    {
        if (HasImageName(containerDetails))
        {
            s_imageBuilder.Append($"{containerDetails.ContainerImageName}");
        }
    }

    private static void HandleRepository(MsBuildContainerProperties containerDetails, string imagePrefix)
    {
        if (HasRepository(containerDetails))
        {
            if (!string.IsNullOrEmpty(imagePrefix))
            {
                s_imageBuilder.Append($"{imagePrefix}/{containerDetails.ContainerRepository}");
            }
            else
            {
                s_imageBuilder.Append($"{containerDetails.ContainerRepository}");
            }
        }

        if (HasImageName(containerDetails))
        {
            s_imageBuilder.Append('/');
        }
    }

    private static void HandleRegistry(MsBuildContainerProperties containerDetails)
    {
        if (HasRegistry(containerDetails))
        {
            s_imageBuilder.Append($"{containerDetails.ContainerRegistry}");
        }

        if (HasRepository(containerDetails))
        {
            s_imageBuilder.Append('/');
        }
    }

    private static void EnsureContainerRegistryIsNotEmpty(MsBuildContainerProperties details, string? containerRegistry)
    {
        if (HasRegistry(details))
        {
            return;
        }

        // Use our custom fall-back value if it exists
        if (!string.IsNullOrEmpty(containerRegistry))
        {
            details.ContainerRegistry = containerRegistry;
        }
    }

    private static void HandleTags(MsBuildProperties<MsBuildContainerProperties> msBuildProperties, List<string>? containerImageTag)
    {
        if (!string.IsNullOrEmpty(msBuildProperties.Properties!.ContainerImageTag))
        {
            return;
        }

        if (containerImageTag is not null)
        {
            msBuildProperties.Properties.ContainerImageTag = string.Join(';', containerImageTag);
            return;
        }

        msBuildProperties.Properties.ContainerImageTag = "latest";
    }

    private static bool HasImageName(MsBuildContainerProperties? containerDetails) => !string.IsNullOrEmpty(containerDetails?.ContainerImageName);
    private static bool HasRepository(MsBuildContainerProperties? containerDetails) => !string.IsNullOrEmpty(containerDetails?.ContainerRepository);
    private static bool HasRegistry(MsBuildContainerProperties? containerDetails) => !string.IsNullOrEmpty(containerDetails?.ContainerRegistry);
}
