using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Requests
{
    /// <summary>
    /// A request to the http://backpack.tf API.
    /// </summary>
    class BackpackTFRequest : BaseRequest
    {
        public override string GetJSON()
        {
            CheckForAPIKey();

            Uri uri = new Uri("http://backpack.tf/api/IGetPrices/v3/?format=json&key="+APIKey);
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
    }
}
