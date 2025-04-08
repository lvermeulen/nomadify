using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Components.V0.Container;
using Nomad.Abstractions.Interfaces;
using Xunit.Abstractions;

namespace Nomadify.Tests.Extensions;

internal static class TestOutputHelperExtensions
{
    public static void Output(this ITestOutputHelper testOutputHelper, ProjectResource resource)
    {
        testOutputHelper.OutputType(resource);
        testOutputHelper.WriteLine($"Path: {resource.Path}");
        testOutputHelper.OutputEnv(resource);
        testOutputHelper.OutputBindings(resource);
    }

    public static void Output(this ITestOutputHelper testOutputHelper, ContainerResource resource)
    {
        testOutputHelper.OutputType(resource);
        testOutputHelper.OutputConnectionString(resource);
        testOutputHelper.OutputImage(resource);
        testOutputHelper.OutputEnv(resource);
        testOutputHelper.OutputBindings(resource);
    }

    public static void Output(this ITestOutputHelper testOutputHelper, BicepResource resource)
    {
        testOutputHelper.OutputType(resource);
        testOutputHelper.OutputConnectionString(resource);
        testOutputHelper.WriteLine($"Path: {resource.Path}");
        testOutputHelper.OutputEnv(resource);
        foreach (var parameterInput in resource.Inputs)
        {
            testOutputHelper.WriteLine($"Param: {parameterInput.Key} = \"{parameterInput.Value}\"");
        }
    }

    private static void OutputType<TResource>(this ITestOutputHelper testOutputHelper, TResource resource)
        where TResource : Resource
    {
        testOutputHelper.WriteLine($"Type: {resource.Type}");
    }

    private static void OutputConnectionString<TResource>(this ITestOutputHelper testOutputHelper, TResource resource)
        where TResource : Resource, IResourceWithConnectionString
    {
        testOutputHelper.WriteLine($"ConnectionString: {resource.ConnectionString}");
    }

    private static void OutputImage<TResource>(this ITestOutputHelper testOutputHelper, TResource resource)
        where TResource : ContainerResource
    {
        testOutputHelper.WriteLine($"Image: {resource.Image}");
    }

    private static void OutputEnv<TResource>(this ITestOutputHelper testOutputHelper, TResource resource)
        where TResource : Resource, IResourceWithEnvironmentalVariables
    {
        if (resource.Env is null)
        {
            return;
        }

        foreach (var environmentVariable in resource.Env)
        {
            testOutputHelper.WriteLine($"Env: {environmentVariable.Key} = {environmentVariable.Value}");
        }
    }

    private static void OutputBindings<TResource>(this ITestOutputHelper testOutputHelper, TResource resource)
        where TResource : Resource, IResourceWithBinding
    {
        if (resource.Bindings is null)
        {
            return;
        }

        if (resource.Bindings.TryGetValue("tcp", out var bindingTcp))
        {
            var output = $"Binding tcp Scheme: {bindingTcp.Scheme}, Protocol: {bindingTcp.Protocol}, Transport: {bindingTcp.Transport}";
            if (bindingTcp.TargetPort is not null)
            {
                output += $", TargetPort: {bindingTcp.TargetPort}";
            }

            testOutputHelper.WriteLine(output);
        }

        if (resource.Bindings.TryGetValue("http", out var bindingHttp))
        {
            var output = $"Binding http Scheme: {bindingHttp.Scheme}, Protocol: {bindingHttp.Protocol}, Transport: {bindingHttp.Transport}";
            if (bindingHttp.TargetPort is not null)
            {
                output += $", TargetPort: {bindingHttp.TargetPort}";
            }

            testOutputHelper.WriteLine(output);
        }

        if (resource.Bindings.TryGetValue("https", out var bindingHttps))
        {
            var output = $"Binding https Scheme: {bindingHttps.Scheme}, Protocol: {bindingHttps.Protocol}, Transport: {bindingHttps.Transport}";
            if (bindingHttps.TargetPort is not null)
            {
                output += $", TargetPort: {bindingHttps.TargetPort}";
            }

            testOutputHelper.WriteLine(output);
        }
    }
}
