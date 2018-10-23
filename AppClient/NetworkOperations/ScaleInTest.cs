using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AppClient.NetworkOperations
{
    class ScaleInTest : TestCase
    {
        override public int TestOperation()
        {

            string endpoint = "http://myFrontendService:3031";

            using (var client = new HttpClient())
            {

                Console.WriteLine($"Hitting {endpoint}/checkfabricids");

                try
                {
                    var result = client.GetAsync(endpoint + "/checkfabricids");

                    var x = result.GetAwaiter().GetResult();
                    string jsonString = x.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    RequestInfo[] requestInfos = JsonConvert.DeserializeObject<RequestInfo[]>(jsonString);
                    List<string> replicaIds = new List<string>();
                    foreach (var replicaStatus in requestInfos)
                    {
                        if (!replicaIds.Contains(replicaStatus.response))
                        {
                            replicaIds.Add(replicaStatus.response);
                        }
                        else
                        {
                            Console.WriteLine($"duplicate replica ID found : {replicaStatus.response}");
                            return -1;
                        }
                    }

                    Console.WriteLine($"Current number of replicas reported {replicaIds.Count}");

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception reported {e.Message}");
                    return -1;

                }
            }

            return 0;
        }
    }
}
