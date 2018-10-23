using System;
using System.Collections.Generic;
using System.Text;
using AppClient.NetworkOperations;
using AppClient.VolumeOperations;

namespace AppClient
{

    class Tests
    {
        public static readonly Dictionary<string, TestCase> testsList = new Dictionary<string, TestCase>()
        {
            { "ScaleOutTest", new ScaleOutTest() },
            { "ScaleInTest", new ScaleInTest() },
            { "BackendEchoTest", new EchoTest() },
            { "VolumeTest", new VolumeTest() }

        };
    }
}
