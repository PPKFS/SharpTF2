using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTF2.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpTF2.Items
{
    /// <summary>
    /// A collection of items along with positional data.
    /// </summary>
    public class Backpack : IEnumerable<Item>
    {
        public static String CacheLocation = "cache/backpack.txt";

        public static Backpack Get(String profileID, String apiKey)
        {
            TF2BackpackRequest request = new TF2BackpackRequest();
            request.APIKey = apiKey;
            request.ProfileID = profileID;
            return Backpack.Get(profileID, apiKey, request);
        }

        public static Backpack Get(String profileID, String apiKey, IRequest request)
        {
            Backpack backpack = new Backpack();

            //get the json
            String raw = request.GetJSON();
            JToken json = JObject.Parse(raw)["result"];

            //now we parse
            int result = json["status"].ToObject<int>();

            if (result != 1)
            {
                if (result == 8)
                    throw new ArgumentException("SteamID invalid");
                if (result == 15)
                    throw new InvalidOperationException("Backpack was private");
                if (result == 18)
                    throw new ArgumentException("SteamID does not exist");
            }

            backpack.NumberOfSlots = json["num_backpack_slots"].ToObject<int>();

            backpack = ParseItems(json["items"]);
            return backpack;
        }

        private static Backpack ParseItems(JToken bpJSON)
        {
            //now we iterate the items
            foreach (JToken itemJSON in bpJSON.Children())
            {
                int id = itemJSON["id"].ToObject<int>();
                int original_id = itemJSON["original_id"].ToObject<int>();
                int defindex = itemJSON["defindex"].ToObject<int>();
                int level = itemJSON["level"].ToObject<int>();
                int origin = itemJSON["origin"] == null ? -1 : itemJSON["origin"].ToObject<int>();
                bool cannotTrade = itemJSON["flag_cannot_trade"] == null ? false : itemJSON["flag_cannot_trade"].ToObject<bool>();
                bool cannotCraft = itemJSON["flag_cannot_craft"] == null ? false : itemJSON["flag_cannot_craft"].ToObject<bool>();
                int quality = itemJSON["quality"].ToObject<int>();
                String customName = itemJSON["custom_name"] == null ? null : itemJSON["custom_name"].ToObject<String>();
                String customDesc = itemJSON["custom_desc"] == null ? null : itemJSON["custom_desc"].ToObject<String>();
                //TODO: giftwrapped
                int style = itemJSON["style"] == null ? -1 : itemJSON["style"].ToObject<int>();
            }
            return null;
        }

        public int NumberOfSlots { get; private set; }


        public IEnumerator<Item> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
