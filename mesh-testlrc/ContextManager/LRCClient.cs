using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Security;


namespace mesh_lrc
{
    internal class LRCCLient : IConnectionManagerClient
    {
        private IServiceFabricClient sfClient;
        public X509SecuritySettings GetSecurityCredentials()
        {
            // get the X509Certificate either from Certificate store or from file.
            var clientCert = new System.Security.Cryptography.X509Certificates.X509Certificate2(this.settings.certLocation, "");
            var remoteSecuritySettings = new RemoteX509SecuritySettings(new List<string> { this.settings.serverCertThumbprint });
            return new X509SecuritySettings(clientCert, remoteSecuritySettings);
        }
        public override async Task<IConnectionManagerClient> InitializeAsync()
        {
            Task<ApplicationResourceDescription> applicationInfo = this.sfClient.ApplicationResources
                .GetApplicationResourceAsync(this.settings.applicationName);

            if (applicationInfo == null)
            {
                // @TODO log attempting to create warn("Application does not exist.");
                try
                {
                    ApplicationResourceDescription ret = await this.sfClient.ApplicationResources.CreateApplicationResourceAsync(
                        this.settings.applicationResourceFile,
                        this.settings.applicationName);
                    return this;
                }
                catch (Exception e)
                {
                    // @TODO log correctly
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            return null;
        }

        public LRCCLient(ConnectionMangerSettings settings) : base(settings)
        {
            var clientSettings = new ClientSettings(this.GetSecurityCredentials);
            this.sfClient =
               ServiceFabricClientFactory.Create(
                   new Uri(this.settings.clusterConnectionUrl), clientSettings);
        }
        public override bool UpdateApp(string pathToTemplate)
        {
            return false;
        }
    }
}
