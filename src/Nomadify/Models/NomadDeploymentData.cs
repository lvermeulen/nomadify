using System;
using System.Collections.Generic;
using System.Linq;
using Nomad.Abstractions.Components.V0;

namespace Nomadify.Models;

public class NomadDeploymentData
{
    public string? Name { get; private set; }
    public string? Namespace { get; private set; }
    public Dictionary<string, string?> Env { get; private set; } = [];
    public Dictionary<string, string?> Secrets { get; private set; } = [];
    public Dictionary<string, string> Annotations { get; private set; } = [];
    public IReadOnlyCollection<Volume> Volumes { get; private set; } = [];
    public IReadOnlyCollection<Ports> Ports { get; private set; } = [];
    public IReadOnlyCollection<string> Manifests { get; private set; } = [];
    public IReadOnlyCollection<string> Args { get; private set; } = [];
    public bool? IsProject { get; private set; }
    public bool? WithPrivateRegistry { get; private set; } = false;
    public bool? WithDashboard { get; private set; } = false;
    public string? ContainerImage { get; private set; }
    public string? Entrypoint { get; private set; }
    public string? ImagePullPolicy { get; private set; }
    public string? ServiceType { get; private set; } = "ClusterIP";

    public NomadDeploymentData SetName(string name)
    {
        Name = name.ToLowerInvariant();
        return this;
    }

    public NomadDeploymentData SetNamespace(string? ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return this;
        }

        Namespace = ns.ToLowerInvariant();
        return this;
    }

    public NomadDeploymentData SetEnv(Dictionary<string, string?> env)
    {
        Env = env.Where(x => !string.IsNullOrEmpty(x.Value)).ToDictionary(x => x.Key, x => x.Value);
        return this;
    }

    public NomadDeploymentData SetSecrets(Dictionary<string, string?> secrets)
    {
        Secrets = secrets;
        return this;
    }

    public NomadDeploymentData SetAnnotations(Dictionary<string, string>? annotations)
    {
        Annotations = annotations ?? [];
        return this;
    }

    public NomadDeploymentData SetManifests(IReadOnlyCollection<string> manifests)
    {
        Manifests = manifests;
        return this;
    }

    public NomadDeploymentData SetArgs(IReadOnlyCollection<string>? args)
    {
        Args = args ?? [];
        return this;
    }

    public NomadDeploymentData SetIsProject(bool project)
    {
        IsProject = project;
        return this;
    }

    public NomadDeploymentData SetEntrypoint(string? entrypoint)
    {
        Entrypoint = entrypoint;
        return this;
    }

    public NomadDeploymentData SetVolumes(List<Volume>? volumes)
    {
        Volumes = volumes ?? [];
        return this;
    }

    public NomadDeploymentData SetWithPrivateRegistry(bool isPrivateRegistry)
    {
        WithPrivateRegistry = isPrivateRegistry;
        return this;
    }

    public NomadDeploymentData SetContainerImage(string? containerImage)
    {
        ContainerImage = containerImage?.ToLowerInvariant();
        return this;
    }

    public NomadDeploymentData SetImagePullPolicy(string? imagePullPolicy)
    {
        ImagePullPolicy = imagePullPolicy ?? "IfNotPresent";
        return this;
    }

    public NomadDeploymentData SetWithDashboard(bool? withDashboard)
    {
        WithDashboard = withDashboard ?? false;
        return this;
    }

    public NomadDeploymentData SetPorts(List<Ports>? ports)
    {
        Ports = ports ?? [];
        return this;
    }

    public NomadDeploymentData SetServiceType(string? serviceType)
    {
        ServiceType = serviceType ?? "ClusterIP";
        return this;
    }

    public NomadDeploymentData Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Name must be set.");
        }

        return this;
    }
}
