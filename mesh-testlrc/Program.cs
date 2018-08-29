namespace mesh_lrc
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.CommandLineParser;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Security;
    using Newtonsoft.Json;

    class RequestInfo
    {
        public string url { get; set; }
        public Boolean reachable { get; set; }
        public int statusCode { get; set; }
        public string response { get; set; }
    }

    class Program
    {
        private static string applicationName = "myapp";

        private static string applicationFileLocation = @"D:\mesh_Port3030_Windows_File.json";
        private static string applicationFileLocation2 = @"D:\mesh_Port3030_Windows_File_2.json";

        private static string serverCertThumbprint = "561D35769884F6A5F91293D8F44B0FAACE7772AE";
        private static string certLocation = @"C:\Users\jejarry\Downloads\jejarry-lrc-jejarry-lrc-20180820.pfx";

        private static string clusterUrl = @"http://jejarry-testlrc.centralus.cloudapp.azure.com:3030";
        private static string clusterConnectionUrl = @"https://jejarry-testlrc.centralus.cloudapp.azure.com:19080";
        static int Main(string[] args)
        {

            var parsedArguments = new ToolArguments();
            if (!CommandLineUtility.ParseCommandLineArguments(args, parsedArguments))
            {
                Console.Out.Write(CommandLineUtility.CommandLineArgumentsUsage(typeof(ToolArguments)));
                return -1;
            }

            clusterUrl = parsedArguments.clusterUrl;
            certLocation = parsedArguments.certLocation;
            serverCertThumbprint = parsedArguments.serverCertThumbprint;
            applicationName = parsedArguments.appName;
            applicationFileLocation = parsedArguments.applicationFileLocation;
            applicationFileLocation2 = parsedArguments.applicationFileLocation2;

            var settings = new ClientSettings(GetSecurityCredentials);
            IServiceFabricClient sfClient = ServiceFabricClientFactory.Create(new Uri(clusterConnectionUrl), settings);

            //check if the application exists first
            var applicationInfo = sfClient.ApplicationResources.GetApplicationResourceAsync(applicationName).GetAwaiter().GetResult();
            if(applicationInfo == null)
            {
                Console.Out.Write("Application does not exist");
                return -1;
            }

            upgrade(sfClient);

            return 0;
        }

        public static void upgrade(IServiceFabricClient serviceFabricClient)
        {
            var allUniqiue = getAllBackendReplicaIds();
            Console.WriteLine("Are all replica IDs unique: " + allUniqiue.ToString());

            Console.WriteLine("Upgrading the application");
            FlipFlopUpgrade(serviceFabricClient);
            System.Threading.Thread.Sleep(30000);

            WaitUntilReady(serviceFabricClient);
            Console.WriteLine("Upgrade is finished");

            allUniqiue = getAllBackendReplicaIds();
            Console.WriteLine("Are all replica IDs unique: " + allUniqiue.ToString());
        }

        public static Boolean getAllBackendReplicaIds()
        {
            Boolean allUniqueReplicas = true; 
            using (var client = new HttpClient())
            {
                Console.WriteLine("hitting " + clusterUrl + "/checkfabricids");
                var result = client.GetAsync(clusterUrl + "/checkfabricids");
                try
                {
                    
                    var x = result.GetAwaiter().GetResult();
                    string jsonString = x.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine(jsonString);

                    var requestInfos = JsonConvert.DeserializeObject<RequestInfo[]>(jsonString);

                    List<string> replicaIds = new List<string>();
                    foreach (var replicaStatus in requestInfos)
                    {
                        if (!replicaIds.Contains(replicaStatus.response))
                        {
                            replicaIds.Add(replicaStatus.response);
                        }
                        else
                        {
                            allUniqueReplicas = false;
                            break;
                        }
                    }
                    Console.WriteLine("Current number of replicas reported : " + replicaIds.Count);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            return allUniqueReplicas;
        }

        public static void WaitUntilReady(IServiceFabricClient serviceFabricClient)
        {
            var status = serviceFabricClient.ApplicationResources.GetApplicationResourceAsync(applicationName).GetAwaiter().GetResult().Status;
            while (status != ApplicationResourceStatus.Ready)
            {
                Console.WriteLine(status);
                System.Threading.Thread.Sleep(2000);
                status = serviceFabricClient.ApplicationResources.GetApplicationResourceAsync(applicationName).GetAwaiter().GetResult().Status;
            }
        }

        public static void FlipFlopUpgrade(IServiceFabricClient serviceFabricClient)
        {
            string currentVersion = GetCurrentVersionOfTemplate();
            Console.WriteLine("Current version in resource template is :" + currentVersion);

            string template = currentVersion == "1" ? applicationFileLocation2 : applicationFileLocation;
            Console.WriteLine("Upgrading to template : " + template);
            try
            {
                serviceFabricClient.ApplicationResources.CreateApplicationResourceAsync(template, applicationResourceName: applicationName).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        public static string GetCurrentVersionOfTemplate()
        {
            Console.WriteLine("hitting " + clusterUrl + "/env");

            string currentVersion = "";
            using (var client = new HttpClient())
            {
                try
                {
                    var result = client.GetAsync(clusterUrl + "/env").GetAwaiter().GetResult();
                    currentVersion = result.Content.ReadAsStringAsync().GetAwaiter().GetResult().Trim();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            return currentVersion;
        }


        public static X509SecuritySettings GetSecurityCredentials()
        {
            // get the X509Certificate either from Certificate store or from file.
            var clientCert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certLocation, "");
            var remoteSecuritySettings = new RemoteX509SecuritySettings(new List<string> { serverCertThumbprint });
            return new X509SecuritySettings(clientCert, remoteSecuritySettings);
        }
    }
}
