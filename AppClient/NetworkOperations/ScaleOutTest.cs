using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AppClient.NetworkOperations
{
    internal class RequestInfo
    {
        public string url { get; set; }
        public Boolean reachable { get; set; }
        public int statusCode { get; set; }
        public string response { get; set; }
    }
    class ScaleOutTest : TestCase
    {
        override public int TestOperation()
        {

            using (var client = new HttpClient())
            {

                string endpoint = "http://localhost:4200";
                Console.WriteLine($"Hitting {endpoint}/checkfabricids");

                var result = client.GetAsync(endpoint + "/checkfabricids");

                try
                {

                    var x = result.GetAwaiter().GetResult();
                    string jsonString = x.Content.ReadAsStringAsync().GetAwaiter().GetResult();

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
                            return -1;

                        }
                    }

                    Console.WriteLine($"Current number of replicas reported {replicaIds.Count}");

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception reported {e}");
                    return -1;
                }
            }

            return 0;
        }
    }
}
