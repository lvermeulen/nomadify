using System;
using System.Collections.Generic;
using System.Linq;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Interfaces;

namespace Nomadify.Extensions;

public static class ResourceExtensions
{
    public static int EnsureBindingsHavePorts(this Dictionary<string, Resource> resources)
    {
        var counter = 0;

        foreach (var resource in resources.Where(x => x.Value is IResourceWithBinding { Bindings: not null }))
        {
            if (resource.Value is not IResourceWithBinding bindingResource || bindingResource.Bindings is null)
            {
                continue;
            }

            foreach (var binding in bindingResource.Bindings)
            {
                if (binding.Key.Equals("http", StringComparison.OrdinalIgnoreCase) && binding.Value.TargetPort is 0 or null)
                {
                    binding.Value.TargetPort = 8081;
                    counter++;
                }

                if (binding.Key.Equals("https", StringComparison.OrdinalIgnoreCase) && binding.Value.TargetPort is 0 or null)
                {
                    binding.Value.TargetPort = 8443;
                    counter++;
                }
            }
        }

        return counter;
    }
}
