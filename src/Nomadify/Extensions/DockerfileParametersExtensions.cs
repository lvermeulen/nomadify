using System.Collections.Generic;
using System.Text;
using Nomadify.Commands.Options;

namespace Nomadify.Extensions;

public static class DockerfileParametersExtensions
{
    private static readonly StringBuilder s_tagBuilder = new();

    public static List<string> ToImageNames(this ContainerOptions options, string resourceKey)
    {
        var images = new List<string>();

        foreach (var tag in options.Tags!)
        {
            s_tagBuilder.Clear();

            if (!string.IsNullOrEmpty(options.Registry))
            {
                s_tagBuilder.Append($"{options.Registry}/");
            }

            if (!string.IsNullOrEmpty(options.Prefix))
            {
                s_tagBuilder.Append($"{options.Prefix}/");
            }

            if (string.IsNullOrEmpty(options.ImageName))
            {
                options.ImageName = resourceKey;
            }

            s_tagBuilder.Append(options.ImageName);

            AppendTag(tag);

            images.Add(s_tagBuilder.ToString());
        }

        return images;
    }

    private static void AppendTag(string? tag)
    {
        if (!string.IsNullOrEmpty(tag))
        {
            s_tagBuilder.Append($":{tag}");
            return;
        }

        s_tagBuilder.Append(":latest");
    }
}
