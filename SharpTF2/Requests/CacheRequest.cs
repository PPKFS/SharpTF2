using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Requests
{
    public class CacheRequest : ProfileRequest
    {
        public String CacheLocation { get; set; }

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
                //take out the date first
                file.ReadLine();
                json = file.ReadToEnd();
            }
            return json;
        }
    }
}
