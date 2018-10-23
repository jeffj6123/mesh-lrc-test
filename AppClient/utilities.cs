using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppClient
{
    class utilities
    {
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

    }
}
