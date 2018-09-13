using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mesh_lrc
{
    class Tests
    {
        public static readonly string[] testCases = { "Hello", "Goodbye" };

        public static bool TestExists(string testCase)
        {
            return false;
        }
        public static bool SelectTest(string testCase)
        {
            switch (testCase)
            {
                case "Hello":
                    Console.WriteLine(testCases[0]);
                    break;
                case "Goodbye":
                    Console.WriteLine(testCases[1]);
                    break;

                default:
                    return false;
            }
            return true;
        }   
    }
}
