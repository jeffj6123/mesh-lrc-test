using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Security;

using Newtonsoft.Json;


namespace mesh_lrc
{
    public static class ConnectionManagerClientFactory
    {
        public static IConnectionManagerClient Create(ConnectionMangerSettings settings)
        {
            if (settings.AreInvalid())
            {
                //@TODO log invalid settings
                return null;
            }

            IConnectionManagerClient connectionManagerClient;
            switch (settings.connectionType)
            {
                case ConnectionType.ONEBOX:
                case ConnectionType.AZURE:
                    connectionManagerClient = new LRCCLient(settings);
                    break;
                case ConnectionType.LRC:
                    connectionManagerClient = new ACClient(settings);
                    break;
                default:
                    //@TODO add log error
                    connectionManagerClient = null;
                    break;
            }
            return connectionManagerClient;
        }
    }

    public abstract class IConnectionManagerClient
    {
        public ConnectionMangerSettings settings;
        protected IConnectionManagerClient(ConnectionMangerSettings settings)
        {
            this.settings.connectionType = settings.connectionType;
        }
 
        public abstract bool UpdateApp(string pathToTemplate);
    }
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

        public LRCCLient(ConnectionMangerSettings settings) : base(settings)
        {
            var clientSettings = new ClientSettings(this.GetSecurityCredentials);
             this.sfClient =
                ServiceFabricClientFactory.Create(
                    new Uri(this.settings.clusterConnectionUrl), clientSettings);

            var applicationInfo = this.sfClient.ApplicationResources.GetApplicationResourceAsync(this.settings.applicationName).GetAwaiter().GetResult();

            if (applicationInfo == null)
            {
                // @TODO log attempting to create warn("Application does not exist.");
                try
                {
                    this.sfClient.ApplicationResources.CreateApplicationResourceAsync(
                        this.settings.applicationResourceFile,
                        this.settings.applicationName).GetAwaiter();
                }
                catch (Exception e)
                {
                    // @TODO log correctly
                    Console.WriteLine("{0} Exception caught.", e);
                }
                
            }
        }
        public override bool UpdateApp(string pathToTemplate)
        {
            return false;
        }
    }

    internal class ACClient : IConnectionManagerClient
    {
        public ACClient(ConnectionMangerSettings settings) : base(settings)
        {
        }
        public override bool UpdateApp(string pathToTemplate)
        {
            return false;
        }
    }
}




namespace Microsoft.ServiceFabricMesh.End2EndTestFramework
{
    using KeyVault.Wrapper;
    using KeyVault.Wrapper.Data;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
    using Microsoft.Azure.Management.ServiceFabricMesh;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure.Authentication;
    using Microsoft.ServiceFabricMesh.End2EndTestFramework.Common;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Context : IDisposable
    {

        private static string vaultURI = "https://TestInfraKeyVault.vault.azure.net";

        private Context(ServiceFabricMeshManagementClient client, IAzure azure, TestSettings settings, KeyVaultClient keyVaultWrapper, LogWriter logWriter, StatusWriter statusWriter, X509Certificate2 stagingCert, X509Certificate2 testinfraCert)
        {
            this.settings = settings;
            this.KeyVaultClient = keyVaultWrapper;
            this.StatusWriter = statusWriter;
            this.LogWriter = logWriter;
            this.Client = client;
            this.AzureClient = azure;
            this.StagingCertificate = stagingCert;
            this.TestInfraCertificate = testinfraCert;
        }

        public static async Task<Context> CreateAsync(string subscription, string location, string logName, string pool, bool doNotDeleteFailed, bool loginWithCert, bool useOnebox, bool enableChaos, bool measurePerf)
        {
            // This is used to avoid the error "The current SynchronizationContext may not be used as a TaskScheduler." on UserTokenProvider.LoginWithPromptAsync
            // https://stackoverflow.com/questions/8245926/the-current-synchronizationcontext-may-not-be-used-as-a-taskscheduler
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            // Taken from https://msazure.visualstudio.com/One/_git/winfab-RP?_a=contents&path=%2Fsrc%2FSFAppDeployer%2FSFAppDeployer.settings.json&version=GBmaster
            string keyvaultClientId = "33e3af95-cf38-49be-9db9-909bb5b3cb2e";
            string keyvaultAppCertThumbprint = "27B3DFCE8054446DDE1E238591933EB4FE7B0C8C";

            string powershellClientID = "1950a258-227b-4e31-a9cf-717495945fc2";
            var powershellRedirectURI = new Uri("urn:ietf:wg:oauth:2.0:oob");

            string stagingCertName = "Certificate-seabreeze-staging";
            string testinfraCertName = "Certificate-KeyVaultClientApp";

            string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

            string E2E_AAD_certName = "Certificate-SeaBreezeE2E";
            string E2E_AAD_clientId = "8122c480-9b56-4413-9338-8397ffffa22b";

            var settings = new TestSettings(subscription, location, "sbzend2end", logName, pool, doNotDeleteFailed, loginWithCert, useOnebox, enableChaos, measurePerf);
            var logWriter = new LogWriter(settings.TestLogFileName);
            LoggingHandler logging = new LoggingHandler(logWriter);
            var statusWriter = new StatusWriter();

            ServiceClientCredentials credentials;
            AzureCredentials azureCreds;
            X509Certificate2 clientCertificate;
            X509Certificate2 stagingCertificate;
            X509Certificate2 testinfraCertificate;
            KeyVaultClient keyVaultClient;
            IAzure azure;

            if (settings.LoginWithCert)
            {
                testinfraCertificate = CertificateStore.GetCertificateByThumbprint(keyvaultAppCertThumbprint);

                keyVaultClient = new KeyVaultClient(((authority, resource, scope) => {
                    AuthenticationContext authContext = new AuthenticationContext(authority);
                    AuthenticationResult authResult = authContext.AcquireToken(resource, new ClientAssertionCertificate(keyvaultClientId, testinfraCertificate));
                    return Task.FromResult(authResult.AccessToken);
                }));

                clientCertificate = GetCertificateFromSecret(keyVaultClient, vaultURI, E2E_AAD_certName);

                credentials = await ApplicationTokenProvider.LoginSilentWithCertificateAsync(tenantId, new ClientAssertionCertificate(E2E_AAD_clientId, clientCertificate));

                azureCreds = new AzureCredentialsFactory().FromServicePrincipal(E2E_AAD_clientId, clientCertificate, tenantId, AzureEnvironment.AzureGlobalCloud);
                azure = Azure.Authenticate(azureCreds).WithSubscription(settings.SubscriptionId);
            }
            else
            {
                keyVaultClient = new KeyVaultClient(((authority, resource, scope) => {
                    AuthenticationContext authContext = new AuthenticationContext(authority);
                    AuthenticationResult authResult = authContext.AcquireToken(resource, powershellClientID, powershellRedirectURI);
                    return Task.FromResult(authResult.AccessToken);
                }));

                testinfraCertificate = GetCertificateFromSecret(keyVaultClient, vaultURI, testinfraCertName);

                credentials = await UserTokenProvider.LoginWithPromptAsync(new ActiveDirectoryClientSettings(powershellClientID, powershellRedirectURI));
                azureCreds = new AzureCredentials(credentials, credentials, tenantId, AzureEnvironment.AzureGlobalCloud);
                azure = Azure.Authenticate(azureCreds).WithSubscription(settings.SubscriptionId);
            }

            stagingCertificate = GetCertificateFromSecret(keyVaultClient, vaultURI, stagingCertName);

            ServiceFabricMeshManagementClient client;
            if (settings.UseOnebox)
            {
                client = new ServiceFabricMeshManagementClient(new Uri("http://localhost:8888"), credentials, logging)
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
                client = new ServiceFabricMeshManagementClient(credentials, logging)
                {
                    SubscriptionId = settings.SubscriptionId
                };
            }

            return new Context(client, azure, settings, keyVaultClient, logWriter, statusWriter, stagingCertificate, testinfraCertificate);
        }

        public string GetSerializedResource(
            Assembly assembly,
            string templateFileName,
            string timestamp,
            Dictionary<string, string> parameterToReplaceWith = null,
            Dictionary<string, string> parameterToKeyVaultSecretMap = null)
        {
            string templateFileContents = GetSerializedResource(assembly, templateFileName, timestamp, parameterToReplaceWith);

            if (parameterToKeyVaultSecretMap != null)
            {
                foreach (var key in parameterToKeyVaultSecretMap.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(parameterToKeyVaultSecretMap[key]))
                    {
                        templateFileContents = templateFileContents.Replace("[parameters('" + key + "')]",
                            GetKeyVaultSecret(parameterToKeyVaultSecretMap[key]));
                    }
                }
            }

            return templateFileContents;
        }

        public string GetSerializedResource(Assembly assembly, string templateFileName, string timestamp, Dictionary<string, string> parameterToReplaceWith = null)
        {
            var templateFileContents = GetTemplateFileContents(assembly, templateFileName);
            templateFileContents = templateFileContents.Replace(Constants.subIdParam, this.settings.SubscriptionId);
            templateFileContents = templateFileContents.Replace(Constants.locationParam, this.settings.Location);
            templateFileContents = templateFileContents.Replace(Constants.resourceGroupParam, this.settings.ResourceGroup + timestamp);

            if (parameterToReplaceWith != null)
            {
                foreach (var key in parameterToReplaceWith.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(parameterToReplaceWith[key]))
                    {
                        templateFileContents = templateFileContents.Replace(key, parameterToReplaceWith[key]);
                    }
                }
            }

            // add the ARM tag that is used to target pools
            var js = JsonConvert.DeserializeObject<JObject>(templateFileContents);
            if (Pool == "functions")
            {
                if (js["Tags"] == null)
                {
                    js["Tags"] = JToken.FromObject(new Dictionary<string, string>());
                }

                Dictionary<string, string> temp = js["Tags"].ToObject<Dictionary<string, string>>();
                temp.Add("__SFInternal_Owner__", "AzureFunctions");
                js["Tags"] = JToken.FromObject(temp);
            }
            else if (Pool != "default")
            {
                if (js["Tags"] == null)
                {
                    js["Tags"] = JToken.FromObject(new Dictionary<string, string>());
                }

                Dictionary<string, string> temp = js["Tags"].ToObject<Dictionary<string, string>>();
                temp.Add("__SFInternal_Owner__", Pool);
                js["Tags"] = JToken.FromObject(temp);
            }

            templateFileContents = JsonConvert.SerializeObject(js, Formatting.Indented);

            return templateFileContents;
        }

        public string GetKeyVaultSecret(string secretName)
        {
            return KeyVaultClient.GetSecretAsync(vaultURI, secretName).ConfigureAwait(false).GetAwaiter().GetResult().Value;
        }

        public X509Certificate2 GetCertificateFromSecret(string secretName)
        {
            return GetCertificateFromSecret(KeyVaultClient, vaultURI, secretName);
        }

        private static X509Certificate2 GetCertificateFromSecret(KeyVaultClient keyVaultClient, string vaultUri, string secretName)
        {
            string secretPassword;
            return GetCertificateFromSecret(keyVaultClient, vaultUri, secretName, out secretPassword);
        }

        private static X509Certificate2 GetCertificateFromSecret(KeyVaultClient keyVaultClient, string vaultUri, string secretName, out string secretPassword)
        {
            string secretValue = keyVaultClient.GetSecretAsync(vaultUri, secretName).ConfigureAwait(false).GetAwaiter().GetResult().Value;

            // dadmello: I have no idea why the entire string should be base 64 encoded but for some reason the CRP format is like this
            // So we follow the same pattern to avoid creating any confusion
            secretValue = Encoding.UTF8.GetString(Convert.FromBase64String(secretValue));

            SecretCertificate secretCert = JsonConvert.DeserializeObject<SecretCertificate>(secretValue);
            if (secretCert.DataType == SecretCertificate.DefaultDataType)
            {
                byte[] certBytes = Convert.FromBase64String(secretCert.Data);
                secretPassword = secretCert.Password;

                X509Certificate2 cert = new X509Certificate2(certBytes, secretPassword, X509KeyStorageFlags.Exportable);

                return cert;
            }
            else
            {
                throw new NotSupportedException(string.Format("Unsupported data type {0} encountered", secretCert.DataType));
            }
        }

        private readonly TestSettings settings;

        private KeyVaultClient KeyVaultClient { get; }

        public LogWriter LogWriter { get; }

        public StatusWriter StatusWriter { get; }

        public IAzure AzureClient { get; }

        public X509Certificate2 StagingCertificate { get; }

        public X509Certificate2 TestInfraCertificate { get; }

        public ServiceFabricMeshManagementClient Client { get; }

        public string Location
        {
            get
            {
                return this.settings.Location;
            }
        }

        public string ResourceGroupName
        {
            get
            {
                return this.settings.ResourceGroup;
            }
        }

        public string SubscriptionId
        {
            get
            {
                return this.settings.SubscriptionId;
            }
        }

        public string Pool
        {
            get
            {
                return this.settings.Pool;
            }
        }

        public bool DoNotDeleteFailedResources
        {
            get
            {
                return this.settings.DoNotDeleteFailedResources;
            }
        }

        public bool LoginWithCert
        {
            get
            {
                return this.settings.LoginWithCert;
            }
        }

        public bool UseOnebox
        {
            get
            {
                return this.settings.UseOnebox;
            }
        }

        public bool EnableChaos
        {
            get
            {
                return this.settings.EnableChaos;
            }
        }

        public bool MeasurePerf
        {
            get
            {
                return this.settings.MeasurePerf;
            }
        }

        public void Dispose()
        {
            if (this.Client != null)
            {
                this.Client.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public static string GetTemplateFileContents(Assembly assembly, string fileName)
        {
            var resourceName = $"{assembly.GetName().Name}.TestDefinitions.ARMTemplates.{fileName}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}