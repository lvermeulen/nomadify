![Icon](https://i.imgur.com/6h4Mghl.png?1)
# Nomadify
![Build status](https://github.com/lvermeulen/Nomadify/actions/workflows/build.yml/badge.svg)
[![license](https://img.shields.io/github/license/lvermeulen/Nomadify.svg?maxAge=2592000)](https://github.com/lvermeulen/Nomadify/blob/master/LICENSE) 
[![NuGet](https://img.shields.io/nuget/v/Nomadify.svg?maxAge=86400)](https://www.nuget.org/packages/Nomadify/) 
![downloads](https://img.shields.io/nuget/dt/Nomadify)
![net9.0](https://img.shields.io/badge/net-9.0-yellowgreen.svg)

Nomadify handles deployments of a .NET Aspire AppHost to HashiCorp Nomad.

## Features:
* Raw exec projects
* Docker projects
* Dockerfile projects

## Usage:
Run the following command:
```bash
dotnet nomadify generate --project-path <path/to/AppHost/project>
```
and follow the prompts.

## Command-line parameters
* ```project-path```: the path to your AppHost project
* ```aspire-manifest```: the path to an existing manifest.json file
* ```no-build```: skips the build
* ```compression-kind```: the compression kind to use. Supported values are: zip, tar
* ```container-builder```: the container builder to use. Supported values are: docker, podman
* ```runtime-identifier```: the runtime identifier to use
* ```include-dashboard```: include the Aspire Dashboard in the deployment (experimental)

## What is .NET Aspire?
.NET Aspire is a lightweight, cross-platform, and open-source framework for building microservices and distributed applications. It is designed to be easy to use and provides a set of tools and libraries for building, deploying, and managing microservices. It is built on top of the .NET platform and is compatible with .NET Core and .NET 8+.
You can find more information about .NET Aspire on the [Aspire website](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview). If you are new to .NET Aspire, I recommend reading the [Aspire documentation](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/build-your-first-aspire-app) to get started.

## What is Hashicorp Nomad?
HashiCorp Nomad is a flexible, enterprise-grade cluster and application scheduler designed to support the needs of both microservices and traditional applications. It is a single binary that can be used to deploy and manage applications across a cluster of machines.
You can find more information about Nomad on the [HashiCorp website](https://www.hashicorp.com/products/nomad). If you are new to Nomad, I recommend reading the [Nomad documentation](https://developer.hashicorp.com/nomad/docs) to get started.

## Service Discovery
Nomadify uses the [ServiceDiscovery.Nomad](https://github.com/lvermeulen/ServiceDiscovery.Nomad) package to find the addresses of services. It works by reading the ```NOMAD_ADDR_*``` environment variables at runtime.

## How does Nomadify work?
Nomadify reads the Aspire manifest file and generates a Nomad job file for each service. It then builds the service and creates a raw exec archive or a Docker image for it. Finally, it uploads the raw exec project to a file server or the Docker image to a Docker registry and deploys the service to Nomad.

Nomadify will ask you a few questions to get the information it needs to generate the Nomad job file. It will create a nomadify-state.json file, then create a manifest.json file in the folder of your Aspire AppHost project.

The Nomad job file will be created in the same folder as the manifest.json file, or in the output path if specified in the nomadify-state.json file. The job file will be named after the service and will have the extension ".job.hcl". It is generated by using a default values json file which you can specify in the nomadify-state.json file.

Here's an example replacement values file:
```json
{
	"PreferredProtocol": "http",
	"JobDatacenter": "dc1",
	"JobType": "service",
	"JobSemVer": "1.2.3",
	"JobRescheduleDelay": "30s",
	"JobRescheduleDelayFunction": "constant",
	"JobRescheduleUnlimited": true,
	"JobUpdateMaxParallel": "1",
	"JobUpdateMinHealthyTime": "10s",
	"JobUpdateHealthyDeadline": "5m",
	"JobUpdateProgressDeadline": "10m",
	"JobUpdateAutoRevert": true,
	"JobUpdateCanary": "0",
	"JobUpdateStagger": "30s",
	"GroupDesiredInstances": "1",
	"GroupUpdateCanary": "1",
	"GroupUpdateMaxParallel": "5",
    "GroupServiceNameLiteralPrefix": "local/",
    "GroupTaskDriver": "raw_exec",
  	"GroupTaskArtifactDestinationPrefix": "",
	"GroupTaskConfigStartCommand": "/opt/dotnet/dotnet",
	"GroupTaskConfigArgs": ["", ""],
	"GroupTaskServiceProvider": "consul",
	"GroupTaskServiceCheckType": "http",
	"GroupTaskServiceCheckPath": "/health",
	"GroupTaskServiceCheckInterval": "2s",
	"GroupTaskServiceCheckTimeout": "200s"
}
```

## Roadmap
* Dapr support
* Aspire Dashboard support

## Compatibility
Nomadify was written for .NET 9.0 and .NET Aspire 9.1, but do try it out with the 8.0 versions of .NET and Aspire and let me know.

## Thanks
* [Camel](https://thenounproject.com/icon/camel-4192809/) icon by [Slidicon](https://thenounproject.com/creator/slidicon/) from [The Noun Project](https://thenounproject.com)
* So much thanks to [David Sekula](https://github.com/prom3theu5), the author of [Aspir8](https://github.com/prom3theu5/aspirational-manifests), which gave me a lot of inspiration (and quite a bit of code) and convinced me to do "the same" for Hashicorp Nomad
