using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Requests
{
    public static class Cache
    {
        public static void SaveJSON(String file, String json)
        {
            File.WriteAllText(file, json);
        }
    }

    public class CacheRequest : ProfileRequest
    {
        public String CacheLocation { get; set; }

        public override string GetJSON()
        {
            if (CacheLocation == null)
                throw new InvalidOperationException("No cache location given");
            String json = null;

            using (StreamReader file = new StreamReader(CacheLocation))
            {
                json = file.ReadToEnd();
            }
            return json;
        }
    }
}
