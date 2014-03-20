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

            Backpack backpack = new Backpack(json["num_backpack_slots"].ToObject<int>());
            List<Item> items = ParseItems(json["items"]);
            foreach (Item i in items)
            {
                if (i.Position == 0 || backpack.items[i.Position] != null)
                    System.Diagnostics.Debugger.Break();
                Console.WriteLine("adding item {0} at pos {1}", i.DefIndex, i.Position);
                backpack.items[i.Position] = i;
            }

            return backpack;
        }

        private static List<Item> ParseItems(JToken bpJSON)
        {
            List<Item> items = new List<Item>();
            //now we iterate the items
            foreach (JToken itemJSON in bpJSON.Children())
                items.Add(ParseItem(itemJSON));

            return items;
        }

        private static Item ParseItem(JToken itemJSON)
        {
            uint id = itemJSON["id"].ToObject<uint>();
            uint original_id = itemJSON["original_id"].ToObject<uint>();
            int defindex = itemJSON["defindex"].ToObject<int>();
            int level = itemJSON["level"].ToObject<int>();
            int quality = itemJSON["quality"].ToObject<int>();
            uint position = (uint)(itemJSON["inventory"].ToObject<uint>() & 65535); //mask the top word, it's deprecated

            //optional
            int origin = itemJSON["origin"] == null ? -1 : itemJSON["origin"].ToObject<int>();
            bool cannotTrade = itemJSON["flag_cannot_trade"] == null ? false : itemJSON["flag_cannot_trade"].ToObject<bool>();
            bool cannotCraft = itemJSON["flag_cannot_craft"] == null ? false : itemJSON["flag_cannot_craft"].ToObject<bool>();

            //String customName = itemJSON["custom_name"] == null ? null : itemJSON["custom_name"].ToObject<String>();
            //String customDesc = itemJSON["custom_desc"] == null ? null : itemJSON["custom_desc"].ToObject<String>();
            //TODO: giftwrapped
            int style = itemJSON["style"] == null ? -1 : itemJSON["style"].ToObject<int>();

            //equipped by
            Dictionary<int, int> equip = new Dictionary<int,int>();
            if (itemJSON["equipped"] == null)
                equip = null;
            else
            {
                foreach (JToken equipData in itemJSON["equipped"])
                    equip[equipData.First.ToObject<int>()] = equipData.Last.ToObject<int>();
            }

            //attributes
            List<Attribute> attribs = new List<Attribute>();

            if (itemJSON["attributes"] != null)
            {
                foreach (JToken attrib in itemJSON["attributes"])
                {
                    Attribute attr = new Attribute();
                    attr.DefIndex = attrib["defindex"].ToObject<int>();

                    if (attr.DefIndex >= 2000 && attr.DefIndex <= 2009)
                    {
                        //it's an ingredient attribute
                        attr = new IngredientAttribute();
                        IngredientAttribute ingAttr = (IngredientAttribute)attr;
                        attr.DefIndex = attrib["defindex"].ToObject<int>();
                        ingAttr.IsOutput = attrib["is_output"].ToObject<bool>();
                        ingAttr.Quantity = attrib["quantity"].ToObject<int>();

                        //there's 2 kinds of ingredients - specific (i.e. X of item Y) and nonspecific (i.e. X of Y where Y has attribute Z)
                        if (attrib["itemdef"] == null)
                        {
                            ingAttr.MatchAll = attrib["match_all_attribs"].ToObject<bool>();
                            ingAttr.AttributeInfo = new List<Attribute>();
                            foreach(JToken inner in attrib["attributes"])
                            {
                                Attribute innerAttr = new Attribute();

                                innerAttr.DefIndex = inner["defindex"].ToObject<int>();
                                innerAttr.Value = inner["value"].ToObject<String>();
                                innerAttr.FloatValue = inner["float_value"] != null ? inner["float_value"].ToObject<float>() : -1;
                                ingAttr.AttributeInfo.Add(innerAttr);
                            }
                        }
                        else
                            ingAttr.ItemDef = attrib["itemdef"].ToObject<int>();

                        ingAttr.Quality = (Quality)attrib["quality"].ToObject<int>();
                    }
                    else
                    {
                        attr.Value = attrib["value"].ToObject<String>();
                        attr.FloatValue = attrib["float_value"] != null ? attrib["float_value"].ToObject<float>() : -1;
                    }
                    if (attrib["account_info"] != null)
                    {
                        attr.SteamInfo = new SteamInfo();
                        attr.SteamInfo.Name = attrib["account_info"]["personaname"].ToObject<String>();
                        attr.SteamInfo.SteamID = attrib["account_info"]["steamid"].ToObject<String>();
                    }
                }
            }

            return Item.CreateItem(id, original_id, defindex, level, quality, position, origin, cannotTrade, cannotCraft, equip, attribs);
        }

        private static void ParseAttribute(JToken attribute, Dictionary<int, String> attributeDictionary)
        {
            
        }

        public int NumberOfSlots { get; private set; }

        private Item[] items;

        public Backpack(int slots)
        {
            this.NumberOfSlots = slots;
            this.items = new Item[slots];
        }

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
