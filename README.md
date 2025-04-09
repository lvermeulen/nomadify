![Icon](https://i.imgur.com/3zMGk6c.png)
# Nomadify
[![Build status](https://github.com/lvermeulen/Nomadify/actions/workflows/build.yml/badge.svg)
[![license](https://img.shields.io/github/license/lvermeulen/Nomadify.svg?maxAge=2592000)](https://github.com/lvermeulen/Nomadify/blob/master/LICENSE) 
[![NuGet](https://img.shields.io/nuget/v/Nomadify.svg?maxAge=86400)](https://www.nuget.org/packages/Nomadify/) 
![downloads](https://img.shields.io/nuget/dt/Nomadify)
![](https://img.shields.io/badge/net-9.0-yellowgreen.svg)

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
* project-path: the path to your AppHost project
* aspire-manifest: the path to an existing manifest.json file
* no-build: skips the build
* compression-kind: the compression kind to use. Supported values are: zip, tar
* container-builder: the container builder to use. Supported values are: docker, podman
* runtime-identifier: the runtime identifier to use
* include-dashboard: include the Aspire Dashboard in the deployment (experimental)

## Service Discovery
Nomadify uses the [ServiceDiscovery.Nomad](https://github.com/lvermeulen/ServiceDiscovery.Nomad) package to find the addresses of services. It works by reading the ```NOMAD_ADDR_*``` environment variables at runtime.

## Roadmap
* Support Dapr
* Support Aspire Dashboard

## Thanks
* [Camel](https://thenounproject.com/icon/camel-4192809/) icon by [Slidicon](https://thenounproject.com/creator/slidicon/) from [The Noun Project](https://thenounproject.com)
* So much thanks to [David Sekula](https://github.com/prom3theu5), the author of [Aspir8](https://github.com/prom3theu5/aspirational-manifests), which gave me a lot of inspiration (and quite a bit of code) and convinced me to do "the same" for Hashicorp Nomad
