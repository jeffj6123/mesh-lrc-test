//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace mesh_testlrc
//{
//    class UpgradeTests
//    {
//        public static void upgrade(IServiceFabricClient serviceFabricClient)
//        {
//            var allUniqiue = getAllBackendReplicaIds();
//            Console.WriteLine("Are all replica IDs unique: " + allUniqiue.ToString());

//            Console.WriteLine("Upgrading the application");
//            FlipFlopUpgrade(serviceFabricClient);
//            System.Threading.Thread.Sleep(30000);

//            WaitUntilReady(serviceFabricClient);
//            Console.WriteLine("Upgrade is finished");

//            allUniqiue = getAllBackendReplicaIds();
//            Console.WriteLine("Are all replica IDs unique: " + allUniqiue.ToString());
//        }

//        public static Boolean getAllBackendReplicaIds()
//        {
//            Boolean allUniqueReplicas = true;
//            using (var client = new HttpClient())
//            {
//                Console.WriteLine("hitting " + clusterUrl + "/checkfabricids");
//                var result = client.GetAsync(clusterUrl + "/checkfabricids");
//                try
//                {

//                    var x = result.GetAwaiter().GetResult();
//                    string jsonString = x.Content.ReadAsStringAsync().GetAwaiter().GetResult();
//                    Console.WriteLine(jsonString);

//                    var requestInfos = JsonConvert.DeserializeObject<RequestInfo[]>(jsonString);

//                    List<string> replicaIds = new List<string>();
//                    foreach (var replicaStatus in requestInfos)
//                    {
//                        if (!replicaIds.Contains(replicaStatus.response))
//                        {
//                            replicaIds.Add(replicaStatus.response);
//                        }
//                        else
//                        {
//                            allUniqueReplicas = false;
//                            break;
//                        }
//                    }
//                    Console.WriteLine("Current number of replicas reported : " + replicaIds.Count);
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("{0} Exception caught.", e);
//                }
//            }
//            return allUniqueReplicas;
//        }

//        public static void WaitUntilReady(IServiceFabricClient serviceFabricClient)
//        {
//            var status = serviceFabricClient.ApplicationResources.GetApplicationResourceAsync(applicationName).GetAwaiter().GetResult().Properties.Status;
//            while (status != ApplicationResourceStatus.Ready)
//            {
//                Console.WriteLine(status);
//                System.Threading.Thread.Sleep(2000);
//                status = serviceFabricClient.ApplicationResources.GetApplicationResourceAsync(applicationName).GetAwaiter().GetResult().Properties.Status;
//            }
//        }

//        public static void FlipFlopUpgrade(IServiceFabricClient serviceFabricClient)
//        {
//            string currentVersion = GetCurrentVersionOfTemplate();
//            Console.WriteLine("Current version in resource template is :" + currentVersion);

//            string template = currentVersion == "1" ? applicationFileLocation2 : applicationFileLocation;
//            Console.WriteLine("Upgrading to template : " + template);
//            try
//            {
//                serviceFabricClient.ApplicationResources.CreateApplicationResourceAsync(template, applicationResourceName: applicationName).GetAwaiter().GetResult();
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("{0} Exception caught.", e);
//            }
//        }

//        public static string GetCurrentVersionOfTemplate()
//        {
//            Console.WriteLine("hitting " + clusterUrl + "/env");

//            string currentVersion = "";
//            using (var client = new HttpClient())
//            {
//                try
//                {
//                    var result = client.GetAsync(clusterUrl + "/env").GetAwaiter().GetResult();
//                    currentVersion = result.Content.ReadAsStringAsync().GetAwaiter().GetResult().Trim();

//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("{0} Exception caught.", e);
//                }
//            }
//            return currentVersion;
//        }
//    }
//}
