using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceFabricMesh;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Security;
using Microsoft.ServiceFabricMesh.End2EndTestFramework;

namespace mesh_lrc
{

    internal class ACClient : IConnectionManagerClient
    {
        private ServiceFabricMeshManagementClient sfMeshClient;
        private ServiceClientCredentials credentials;
        public override Task<IConnectionManagerClient> InitializeAsync()
        {
            return null;
        }
        public ACClient(ConnectionMangerSettings settings) : base(settings)
        {
            Task.Run(() => this.helper()).Wait();


            if (settings.connectionType == ConnectionType.ONEBOX)
            {
                this.sfMeshClient = new ServiceFabricMeshManagementClient(new Uri("http://localhost:8888"), credentials)
                {
                    SubscriptionId = settings.SubscriptionId
                };

                var httpClient = new HttpClient();

                HttpResponseMessage response = await httpClient.PutAsync(
                $"http://localhost:8888/subscriptions/{settings.SubscriptionId}",
                new StringContent(@"{""targetState"": ""Registered"", ""properties"": { ""tenantId"": ""tenantId1"" } }")
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                });
                response.EnsureSuccessStatusCode();

                string quota = @"{""resourceCount"" : ""200""}";
                string gatewaysQuota = @"{""resourceCount"" : ""200"", ""publicIpCountPerIngressNetwork"" :""10"", ""publicPortCountPerIp"" : ""10""}";
                string secretsQuota = @"{""resourceCount"" : ""200"", ""valuesPerSecret"" : ""200""}";

                string[] types = { "applications", "gateways", "networks", "secrets", "volumes", "sfvolumes" };
                foreach (string type in types)
                {
                    if (type == "gateways")
                    {
                        response = await httpClient.PutAsync(
                            $"http://localhost:8888/admin/subscriptionManager/subscriptions/{settings.SubscriptionId}/{type}/quota",
                            new StringContent(gatewaysQuota)
                            {
                                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                            });
                    }
                    else if (type == "secrets")
                    {
                        response = await httpClient.PutAsync(
                            $"http://localhost:8888/admin/subscriptionManager/subscriptions/{settings.SubscriptionId}/{type}/quota",
                            new StringContent(secretsQuota)
                            {
                                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                            });
                    }
                    else
                    {
                        response = await httpClient.PutAsync(
                            $"http://localhost:8888/admin/subscriptionManager/subscriptions/{settings.SubscriptionId}/{type}/quota",
                            new StringContent(quota)
                            {
                                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                            });
                    }

                    response.EnsureSuccessStatusCode();
                }
            }
            else
            {
                this.sfMeshClient = new ServiceFabricMeshManagementClient(credentials)
                {
                    SubscriptionId = settings.SubscriptionId
                };
            }

        }
        public override bool UpdateApp(string pathToTemplate)
        {
            return false;
        }
    }
}
