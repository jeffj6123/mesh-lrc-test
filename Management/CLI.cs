using Microsoft.ServiceFabric.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mesh_lrc
{
    class CLI
    {
        public static void EnterCLI(ConnectionInformation settings, IServiceFabricClient sfClient)
        {
            Console.WriteLine("Welcome to the Management Client CLI. " + HelpMessage);
            var userExited = false;
            while (!userExited)
            {
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "q":
                    case "quit":
                        userExited = true;
                        break;

                    case "h":
                    case "help":
                        Console.WriteLine(HelpMessage);
                        break;

                    default:
                        if (Tests.TestExists(input))
                        {
                            Tests.SelectTest(input);
                        }
                        else
                        {
                            Console.WriteLine("Unrecognized Command or Test Case. " + HelpMessage +
                            " Supported tests include: " + string.Join(",", Tests.testCases));
                        }
                        break;
                }
            }
        }
        public const string HelpMessage = "Type h or help for help. Enter 'q' or 'quit' to exit.";
    }
}
