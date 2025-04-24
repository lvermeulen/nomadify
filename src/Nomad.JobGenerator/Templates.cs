namespace Nomad.JobGenerator;

public static class Templates
{
    public const string AspireDashboardTaskTemplate =
        """
        ﻿{{=<% %>=}}task "aspire-dashboard" {
          driver = "docker"
    
          config {
            image = "mcr.microsoft.com/dotnet/aspire-dashboard:9.1-arm64v8"
            args = [
        	    "-p", "${NOMAD_PORT_http_aspire_dashboard}:18888",
        	    "-p", "4317:18889",
        	    "--name", "aspire-dashboard",
        	    "-bind", "${NOMAD_PORT_http_aspire_dashboard}",
        	    #"${nomad.datacenter}",
        	    "--env", "ASPNETCORE_URLS=\"http://*:${NOMAD_PORT_http_aspire_dashboard}\""
            ]
        
          }
    
          service {
            name = "aspire-dashboard"
            port = "http-aspire-dashboard"
            provider = "<%GroupTaskServiceProvider%>"
          
            tags = [<%GroupTaskServiceTags%>]
          
            check {
              type     = "<%GroupTaskServiceCheckType%>"
              path     = "<%GroupTaskServiceCheckPath%>"
              interval = "<%GroupTaskServiceCheckInterval%>"
              timeout  = "<%GroupTaskServiceCheckTimeout%>"
            }
          }
          
          env {
          }
        }

        """;

    public const string DaprdSidecarTemplate =
        """
        ﻿{{=<% %>=}}
        task "daprd-<%DaprComponentResourceName%>" {
            driver = "docker"
          
            config {
          	  image   = "daprio/daprd:1.15.4-linux-arm64"
          	  ports   = [ "http-daprd-<%DaprComponentResourceName%>", "http-daprd-grpc-<%DaprComponentResourceName%>", "http-daprd-http-<%DaprComponentResourceName%>", "http-daprd-metrics-<%DaprComponentResourceName%>" ]
          	  command = "./daprd"
          	  args = [
          	    "-app-id", "<%DaprComponentResourceName%>",
          	    "-app-port", "${NOMAD_PORT_http_daprd_<%DaprComponentResourceName%>}",
          	    "--dapr-grpc-port", "${NOMAD_PORT_http_daprd_grpc_<%DaprComponentResourceName%>}",
          	    "--dapr-http-port", "${NOMAD_PORT_http_daprd_http_<%DaprComponentResourceName%>}",
          	    "--metrics-port", "${NOMAD_PORT_http_daprd_metrics_<%DaprComponentResourceName%>}",
          	    "--app-channel-address", "192.168.1.34",
          	    "--app-protocol", "http",
          	    #"-app-protocol", "grpc",
          	    #"-dapr-internal-grpc-port", "${NOMAD_PORT_http_daprd_rpc}",
          	    "-config", "local/.dapr/config.yaml",
          	    "-resources-path", "local/.dapr/components/",
          	  ]
            }
          
            <! templates !>
          
            resources {
          	memory     = 30
            }
          }
        """;

    public const string DaprdConfigYamlTemplate =
        """
        ﻿{{=<% %>=}}template {
          data = <<EOF
          apiVersion: dapr.io/v1alpha1
          kind: Configuration
          metadata:
            name: daprConfig
          spec:
            tracing:
              samplingRate: "1"
              zipkin:
                endpointAddress: http://${NOMAD_ADDR_http_daprd_<%DaprComponentResourceName%>}:9411/api/v2/spans	
        EOF
        
            destination = "local/.dapr/config.yaml"
          }
        """;

    public const string TaskTemplate =
        """
        ﻿{{=<% %>=}}  task "<%GroupServiceName%>" {
              driver = "<%GroupTaskDriver%>"
        
              artifact {
        	    source = "<%GroupTaskArtifactSourceUrl%>"
        	  }
        
              config {
                command = "/bin/bash"
                args = [
                  "-c",
                  "cd <%GroupServiceNameLiteralPrefix%> && <%GroupTaskConfigStartCommand%> <%GroupServiceNameLiteral%> --urls http://*:${NOMAD_PORT_http_<%GroupServiceName%>};https://*:${NOMAD_PORT_https_<%GroupServiceName%>}"
                ]
              }
        
              service {
                name = "http-<%GroupServiceName%>"
                port = "http-<%GroupServiceName%>"
                provider = "<%GroupTaskServiceProvider%>"
              
                tags = ["http"]
              
                check {
                  type     = "<%GroupTaskServiceCheckType%>"
                  path     = "<%GroupTaskServiceCheckPath%>"
                  interval = "<%GroupTaskServiceCheckInterval%>"
                  timeout  = "<%GroupTaskServiceCheckTimeout%>"
                }
              }
        
              service {
                name = "https-<%GroupServiceName%>"
                port = "https-<%GroupServiceName%>"
                provider = "<%GroupTaskServiceProvider%>"
              
                tags = ["https"]
              
                check {
                  type     = "<%GroupTaskServiceCheckType%>"
                  path     = "<%GroupTaskServiceCheckPath%>"
                  interval = "<%GroupTaskServiceCheckInterval%>"
                  timeout  = "<%GroupTaskServiceCheckTimeout%>"
                }
              }
                      
              env {
                <! env-vars !>
              }
            }

        """;

    public const string JobTemplate =
        """
        ﻿{{=<% %>=}}job "<%JobServiceName%>" {

          datacenters = ["<%JobDatacenter%>"]
          type        = "<%JobType%>"
        
          meta {
            version  = "<%JobSemVer%>"
            run_uuid = "${uuidv4()}"
          }
        
          reschedule {
            delay          = "<%JobRescheduleDelay%>"
            delay_function = "<%JobRescheduleDelayFunction%>"
            unlimited      = <%JobRescheduleUnlimited%>
          }
        
          update {
            max_parallel      = <%JobUpdateMaxParallel%>
            min_healthy_time  = "<%JobUpdateMinHealthyTime%>"
            healthy_deadline  = "<%JobUpdateHealthyDeadline%>"
            progress_deadline = "<%JobUpdateProgressDeadline%>"
            auto_revert       =  <%JobUpdateAutoRevert%>
            canary            =  <%JobUpdateCanary%>
            stagger           = "<%JobUpdateStagger%>"
          }
        
          group "<%JobServiceName%>" {
        
            count = <%GroupDesiredInstances%>
        
            update {
              canary       = <%GroupUpdateCanary%>
              max_parallel = <%GroupUpdateMaxParallel%>
            }
        
            network {
              <! ports !>
            }
          
            <! tasks !>
          }

        """;

    public const string DaprComponentTemplate =
        """
        ﻿{{=<% %>=}}apiVersion: dapr.io/v1alpha1
        kind: Component
        metadata:
          name: <%name%>
        spec:
          type: <%type%>.redis
          version: <%version%>
        <%#emptyMetadata%>
          metadata: []
        <%/emptyMetadata%>
        <%#hasMetadata%>
          metadata:
          <%#metadata.GetEnumerator%>
          - name: <%Key%>
            value: <%#Value%><%Value%><%/Value%><%^Value%>""<%/Value%>
          <%/metadata.GetEnumerator%>
        <%/hasMetadata%>
        """;
}
