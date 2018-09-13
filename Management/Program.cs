namespace mesh_lrc
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Security;
    using CommandLine;

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

        static int Main(string[] args)
        {

            ConnectionInformation settings = ParseArgs(args);
            IServiceFabricClient sfClient = Connect(settings);
            if (sfClient == null)
            {
                //@TODO: Log Connection failure in Connect
                return -1;
            }

            if (settings.CLIEnabled)
            {
                CLI.EnterCLI(settings, sfClient);
            }
            else if (settings.TestSelected)
            {
                SelectTest();
            }
            else
            {
                Console.WriteLine("No test selected. Goodbye!");
            }
            return 0;
            //Parser.Default.ParseArguments<Options>(args)
            //       .WithParsed<Options>(opts =>
            //       {
            //           if (opts.AppName)
            //           {
            //           }

            //       });

            //var parsedArguments = new ToolArguments();
            //if (!CommandLineUtility.ParseCommandLineArguments(args, parsedArguments))
            //{
            //    Console.Out.Write(CommandLineUtility.CommandLineArgumentsUsage(typeof(ToolArguments)));
            //    return -1;
            //}

            //clusterUrl = parsedArguments.clusterUrl;
            //certLocation = parsedArguments.certLocation;
            //serverCertThumbprint = parsedArguments.serverCertThumbprint;
            //applicationName = parsedArguments.appName;
            //applicationFileLocation = parsedArguments.applicationFileLocation;
            //applicationFileLocation2 = parsedArguments.applicationFileLocation2;

            //var settings = new ClientSettings(GetSecurityCredentials);
            //IServiceFabricClient sfClient = ServiceFabricClientFactory.Create(new Uri(clusterConnectionUrl), settings);

            ////check if the application exists first
            //var applicationInfo = sfClient.ApplicationResources.GetApplicationResourceAsync(applicationName).GetAwaiter().GetResult();
            //if(applicationInfo == null)
            //{
            //    Console.Out.Write("Application does not exist");
            //    return -1;
            //}

            //upgrade(sfClient);

            //return 0;
        }
        public static ConnectionInformation ParseArgs(string[] args)
        {
            return new ConnectionInformation();
        }
        public static IServiceFabricClient Connect(ConnectionInformation connectionInfo)
        {
            
            return null;
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
