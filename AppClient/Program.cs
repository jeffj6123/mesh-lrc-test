using System;
using System.Collections.Generic;
using System.Linq;

namespace AppClient
{
    class Program
    {
        public static Tests Tests = new Tests();

        //config options
        public static int timeBetweenTest = 0;

        static void Main(string[] args)
        {
            var tests = GetTestCases();
            RunTests(tests);
        }

        public static void RunTests(List<TestCase> testList)
        {
            while (true)
            {
                foreach(var test in testList)
                {
                    string testName = test.GetType().Name;
                    Console.WriteLine("Starting test : " + testName);
                    var exitCode = test.TestOperation();
                    checkExitCode(exitCode, testName);

                    if(timeBetweenTest > 0)
                    {
                        System.Threading.Thread.Sleep(timeBetweenTest);
                    }
                }
            }
        }

        public static void checkExitCode(int exitCode, string testName)
        {
            if(exitCode != 0)
            {
                Console.WriteLine(testName + " exit with " + exitCode.ToString());
            }
        }

        public static List<string> GetTestCaseNames()
        {

             string tests = Environment.GetEnvironmentVariable("testNames");

             if(tests == null)
            {
                tests = "";
            }
            tests = "ScaleOutTest,ScaleInTest";
            List<string> testNames = tests.Split(',').ToList();
            return testNames;
        }

        public static List<TestCase> GetTestCases()
        {
            List<TestCase> testCases = new List<TestCase>();
            var testCaseNames = GetTestCaseNames();
            foreach(var testName in testCaseNames)
            {
                if (Tests.testsList.ContainsKey(testName))
                {
                    testCases.Add(Tests.testsList[testName]);
                }
                else
                {
                    Console.WriteLine(testName + " does not exist in test cases");
                }
            }

            return testCases;
        }
    }
}
