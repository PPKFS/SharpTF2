using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2
{
    public class TF2BackpackRequest : APIKeyRequest
    {
        public override String GetJSON()
        {
            if (APIKey == null)
                throw new InvalidOperationException("No API key provided.");

            if (ProfileID == null)
                throw new InvalidOperationException("No profile ID provided.");


            Uri uri = new Uri(
                "http://api.steampowered.com/IEconItems_440/GetPlayerItems/v0001/"+"?key="+APIKey+"&SteamID="+ProfileID);
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
