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
    public class BackpackTFRequest : BaseRequest
    {
		private string GetJSON(String addr)
		{
			CheckForAPIKey();
			Uri uri = new Uri(addr + APIKey);
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
			String json = String.Empty;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
				Stream dataStream = response.GetResponseStream();
				StreamReader reader = new StreamReader(dataStream);
				json = reader.ReadToEnd();
				reader.Close();
				dataStream.Close();
			}

			return json;
		}

        public override string GetJSON()
        {
			return GetJSON("http://backpack.tf/api/IGetPrices/v4/?format=json&raw=1&compress=1&key=");
        }

		public string GetCurrencyJSON()
		{
			return GetJSON("http://backpack.tf/api/IGetCurrencies/v1/?format=json&compress=1&key=");
		}
    }
}
