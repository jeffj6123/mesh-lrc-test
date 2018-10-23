using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppClient.NetworkOperations
{
    class EchoTest : TestCase
    {
        override public int TestOperation()
        {
            string endpoint = "http://localhost:4200";

            using (var client = new HttpClient())
            {

                string random = utilities.GetRandomString();

                Console.WriteLine($"Hitting {endpoint}/all with string {random}");

                var httpContent = new StringContent(random, Encoding.UTF8, "text/plain");
                var httpResponse = client.PostAsync(endpoint, httpContent);
                try
                {

                    var x = httpResponse.GetAwaiter().GetResult();
                    string jsonString = x.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    Console.WriteLine(jsonString);
                    RequestInfo[] requestInfos = JsonConvert.DeserializeObject<RequestInfo[]>(jsonString);
                    List<string> replicaIds = new List<string>();
                    foreach (RequestInfo replicaStatus in requestInfos)
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

            return 1;
        }

    }
}
