using System;
using System.IO;

namespace Common
{
    class Logger
    {
        private readonly StreamWriter logStream;
        public Logger(string filePath)
        {
            this.logStream = new StreamWriter(filePath);
        }
        public void LogInfo(string logStr)
        {
            Console.WriteLine(logStr);
        }

        public void LogWarn(string logStr)
        {
            Console.WriteLine(logStr);
        }

        public void LogError(string logStr)
        {
            Console.WriteLine(logStr);
        }

        private void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
