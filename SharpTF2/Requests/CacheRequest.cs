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

        //why is this even here
        public static int DaysSinceCacheUpdate(String cacheLocation)
        {
            if (cacheLocation == null)
                throw new InvalidOperationException("No cache location given");
            String date = null;
            using (StreamReader file = new StreamReader(cacheLocation))
                date = file.ReadLine();
            TimeSpan span = DateTime.Now - DateTime.Parse(date).Date;
            return span.Days;
        }

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
