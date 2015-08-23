using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SharpTF2.Items;

namespace SharpTF2
{
    public class StatsSchema
    {
        public static string GetJSON()
        {
            return File.ReadAllText("stats.txt");

            Uri uri = new Uri("http://stats.tf/api/IGetItemStats.php");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            String json = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                json = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }

            return json;
        }

        public static StatsSchema Get()
        {
            StatsSchema schema = new StatsSchema();
            String raw = GetJSON();
            JToken json = JToken.Parse(raw);
            foreach (JProperty propertyItem in json)
            {
                dynamic item = propertyItem.Value;
                int defindex = item._id;
                if (defindex == 1015)
                {
                    foreach (JProperty prop in item)
                    {
                        Console.WriteLine(prop.Name + " " + prop.Value);
                        
                    }
                    System.Diagnostics.Debugger.Break();
                }
                if (item.count == 0)
                {
                    Console.WriteLine("Skipping item " + defindex + " as none exist.");
                    continue;
                }
                int count = item.count;
                int tradable = item.tradable;
                int craftable = item.craftable;
                Dictionary<Quality, int> qualities = item.qualities.ToObject<Dictionary<Quality, int>>();
                Dictionary<int, int> particles = item.particles.ToObject<Dictionary<int, int>>();

                //System.Diagnostics.Debugger.Break();
            }

            return schema;
        }
    }
}
