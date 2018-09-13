namespace Common
{
    using System;

    public class StatusWriter
    {
        public void WriteStatus(string status)
        {
            var timestamp = DateTime.Now.ToString();
            Console.WriteLine($"{timestamp}  -  {status}");
        }
    }

}