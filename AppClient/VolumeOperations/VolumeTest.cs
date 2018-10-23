using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AppClient.VolumeOperations
{
    class VolumeTest: TestCase
    {
        override public int TestOperation()
        {
            string endpoint = "";

            string fileName = utilities.GetRandomString() + ".txt";
            string fileData = utilities.GetRandomString();

            using (var client = new HttpClient())
            {

                Console.WriteLine($"Hitting {endpoint}/{fileName} setting with data {fileData}");

                var httpContent = new StringContent(fileData, Encoding.UTF8, "text/plain");
                var httpResponse = client.PostAsync($"{endpoint}/{fileName}", httpContent);

                string returnedValues;
                try
                {

                    var response = httpResponse.GetAwaiter().GetResult();

                    Console.WriteLine($"Received response from post request");
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Volume test received non successful status code {response.StatusCode} for post");
                        return -1;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception reported {e}");
                    return -1;

                }


                Console.WriteLine($"Hitting {endpoint}/{fileName} second time");

                var result = client.GetAsync($"{endpoint}/{fileName}");
                try
                {

                    var response = result.GetAwaiter().GetResult();
                    returnedValues = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    Console.WriteLine($"Received response from get request");
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Volume test received non successful status code {response.StatusCode} for post");
                        return -1;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception reported {e}");
                    return -1;

                }
                if(returnedValues != fileData)
                {
                    Console.WriteLine($"secondValue is not larger than first value");
                    return -1;
                }
            }

            return 1;
        }
    }
}
