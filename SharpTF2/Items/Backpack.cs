using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTF2.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Collections;
using System.IO;
using SharpTF2.Prices;

namespace SharpTF2.Items
{
    /// <summary>
    /// A collection of items along with positional data.
    /// </summary>
    public class Backpack
    {
        #region Get
        //helper method for requesting backpack data straight from the steam servers

		public static Backpack GetFromFile(String id)
		{
			CacheRequest request = new CacheRequest();
			request.CacheLocation = String.Format("backpack{0}.txt", id);
			return Backpack.Get(request);
		}

        public static Backpack Get(String profileID, String apiKey)
        {
            TF2BackpackRequest request = new TF2BackpackRequest();
            request.APIKey = apiKey;
            request.ProfileID = profileID;
            return Backpack.Get(request, true, profileID);
        }

        public static Backpack Get(IRequest request, bool cache=true, String profileID=null)
        {
            //get the json
            String raw = request.GetJSON();
			if(cache)
				Cache.SaveJSON(String.Format("backpack{0}.txt", profileID ?? ""), raw);
            JToken json = JObject.Parse(raw)["result"];

            //parse it
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
            //parse all the items
            List<Item> items = ParseItems(json["items"]);

            //place the items in the backpack
            foreach (Item i in items)
            {
				if (i.IsPlaced)
					backpack.ItemsAsPlaced[i.Position - 1] = i;
				else
					backpack.UnplacedItems.Add(i);
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
            //copy across
            uint id = itemJSON["id"].ToObject<uint>();
            uint original_id = itemJSON["original_id"].ToObject<uint>();
            int defindex = itemJSON["defindex"].ToObject<int>();
            int level = itemJSON["level"].ToObject<int>();
            int quality = itemJSON["quality"].ToObject<int>();

			uint inv = itemJSON["inventory"].ToObject<uint>();
            uint position = (uint)(inv & 65535); //mask the top word, it's deprecated
			if ((inv & 0x40000000) == 0x40000000) //except, of course, the 2nd bit (!!???!)
				position = uint.MaxValue;

            //optional
            int origin = itemJSON["origin"] == null ? -1 : itemJSON["origin"].ToObject<int>();
            bool cannotTrade = itemJSON["flag_cannot_trade"] == null ? false : itemJSON["flag_cannot_trade"].ToObject<bool>();
            bool cannotCraft = itemJSON["flag_cannot_craft"] == null ? false : itemJSON["flag_cannot_craft"].ToObject<bool>();
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

                        //there's 2 kinds of ingredients - specific (i.e. X of item Y) and nonspecific 
                        //(i.e. X of Y where Y has attribute Z)
                        //and now with set #3 of the chem sets, there are just 'X of quality Y' - i.e. 3 strange items
                        if (attrib["itemdef"] == null)
                        {
                            //X of quality Y
                            if (attrib["match_all_attribs"] == null)
                            {
                                ingAttr.ItemDef = IngredientAttribute.AnyItemOfQuality;
                            }
                            else
                            {
                                //X of Y where Y has attr Z
                                ingAttr.MatchAll = attrib["match_all_attribs"].ToObject<bool>();
                                ingAttr.AttributeInfo = new List<Attribute>();
                                foreach (JToken inner in attrib["attributes"])
                                {
                                    Attribute innerAttr = new Attribute();

                                    innerAttr.DefIndex = inner["defindex"].ToObject<int>();
                                    innerAttr.Value = inner["value"].ToObject<String>();
                                    innerAttr.FloatValue = inner["float_value"] != null ? inner["float_value"].ToObject<float>() : -1;
                                    ingAttr.AttributeInfo.Add(innerAttr);
                                }
                            }
                        }
                        else
                        {
                            //specific
                            ingAttr.ItemDef = attrib["itemdef"].ToObject<int>();
                        }

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
                    attribs.Add(attr);
                }
            }

            return Item.CreateItem(id, original_id, defindex, level, quality, position, origin, cannotTrade, cannotCraft, equip, attribs);
        }
        #endregion

        public int NumberOfSlots { get; set; }

        public Item[] ItemsAsPlaced { get; set; }

		public List<Item> UnplacedItems { get; set; }

		public Item[] Items
		{
			get
			{
				var arr = UnplacedItems.Concat(ItemsAsPlaced.Where(i => i != null)).ToArray();
				return arr;
			}
		}

        public Backpack(int slots)
        {
            this.NumberOfSlots = slots;
            this.ItemsAsPlaced = new Item[slots];
			this.UnplacedItems = new List<Item>();
        }

        public void LoadSchema(ItemSchema items)
        {
            foreach (Item i in Items)
            {
                i.Name = items.Items[i.DefIndex].Name;
            }
        }

        public void LoadPrices(ItemSchema items, PriceSchema prices)
        {
            foreach (Item i in Items)
            {
                //so we've got several things to consider for prices.
                //Strange parts
                //Paint
                //Killstreaks
                //Levels
                //Vintage levels
                //Craft numbers
                i.BasePrice = prices.GetPrice(i);

                //strange parts
                foreach(int attr in new int[]{ Attribute.StrangePart1, Attribute.StrangePart2, Attribute.StrangePart3})
                {
                    if (i.HasAttribute(attr))
                    {
                        Price sPart = prices.GetUniquePriceByDefindex(items.StrangePartIDs[(int)i.Attributes[attr].FloatValue]);
                        i.AddPriceBonus(items.StrangePartNames[(int)i.Attributes[attr].FloatValue], sPart*0.5);
                    }
                }

                //paint
                if (i.HasAttribute(Attribute.Paint))
                {
                    Price paint = prices.GetUniquePriceByDefindex(items.PaintIDs[(int)i.Attributes[Attribute.Paint].FloatValue]);
					string paintName = items.PaintNames[(int)i.Attributes[Attribute.Paint].FloatValue];
                    i.AddPriceBonus(paintName, paint * 0.5);
                }

                //killstreaks
                //TODO

                //levels
                //IGNORE FOR NOW, TOOD

                //vintage levels
                //TODO

                //craft numbers
                //TODO
            }
        }
    }
}
