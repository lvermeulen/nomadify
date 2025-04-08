using System;
using System.Collections.Generic;
using System.Linq;
using Nomad.Abstractions.Components.V0;
using Nomad.Abstractions.Interfaces;

namespace Nomadify.Extensions;

public static class ResourceExtensions
{
    public static void EnsureBindingsHavePorts(this Dictionary<string, Resource> resources)
    {
        foreach (var resource in resources.Where(x => x.Value is IResourceWithBinding { Bindings: not null }))
        {
            var bindingResource = resource.Value as IResourceWithBinding;
            if (bindingResource is null || bindingResource.Bindings is null)
            {
                continue;
            }

            foreach (var binding in bindingResource.Bindings)
            {
                if (binding.Key.Equals("http", StringComparison.OrdinalIgnoreCase) && binding.Value.TargetPort is 0 or null)
                {
                    binding.Value.TargetPort = 8081;
                }

                if (binding.Key.Equals("https", StringComparison.OrdinalIgnoreCase) && binding.Value.TargetPort is 0 or null)
                {
                    binding.Value.TargetPort = 8443;
                }
            }
        }
    }
}
