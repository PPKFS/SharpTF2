using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpTF2.Items;
using SharpTF2.Requests;

namespace SharpTF2.Prices
{
    public class PriceSchema
    {
        //so this seems to be the easiest way
        public Dictionary<String, Price> PriceList = new Dictionary<String, Price>();

		public const string CacheLocation = "backpacktf.txt";

		public static PriceSchema GetFromFile(String filename=PriceSchema.CacheLocation)
		{
			CacheRequest request = new CacheRequest();
			request.CacheLocation = filename;
			return PriceSchema.Get(request, false);
		}

        public static PriceSchema Get(String apiKey)
        {
            BackpackTFRequest request = new BackpackTFRequest();
            request.APIKey = apiKey;
            return PriceSchema.Get(request);
        }

        public static PriceSchema Get(IRequest request, bool cache=true)
        {
            //get the json
            String raw = request.GetJSON();

            dynamic fullJson = JsonConvert.DeserializeObject(raw);
			if (fullJson.success == 0)
				return null;
			if (cache)
				Cache.SaveJSON("backpacktf.txt", raw);
            dynamic json = fullJson.response;

            PriceSchema schema = new PriceSchema();

            //first, get the raw prices of buds/keys/ref.
            Price.RefPrice = json.raw_usd_value;
			Price.KeyPrice = json.items["Mann Co. Supply Crate Key"]["prices"]["6"]["Tradable"]
				["Craftable"][0]["value_raw"];
            Price.BudsPrice = json["items"]["Earbuds"]["prices"]["6"]["Tradable"]["Craftable"][0]["value_raw"];

            foreach (dynamic item in json.items)
            {
                //we don't want Australium Gold
                bool isAus = item.Name.StartsWith("Australium") && !item.Name.Contains("Gold");
                if (item.Value.defindex.Count == 0)
                {
                    Console.WriteLine("Found an item with no defindex");
                    continue;
                }

				//it's changed - now defindices are defindex: [val]
                int defIndex = (int)item.Value.defindex[0].Value;
                //and now iterate all the qualities
                foreach (dynamic itemQuality in item.Value.prices)
                {
                    Quality quality = (Quality)Enum.Parse(typeof(Quality), itemQuality.Name);
                    foreach (dynamic tradableItem in itemQuality.Value)
                    {
                        //tradables, craftables
                        bool isTradable = (tradableItem.Name == "Tradable");
                        foreach (dynamic craftableItem in tradableItem.Value)
                        {
                            bool isCraftable = (craftableItem.Name == "Craftable");


							//it's now split into some things being Craftability: [blah blah] and some being Craftability: attribute value : blah blah
                            //this is 0 for most things, but some things like unusuals and crates have values
                            foreach (dynamic attributes in craftableItem.Value)
                            {
                                //ignore it if it's 0.
								bool isNested = !(craftableItem.Value is JArray);
                                int series = isNested ? Convert.ToInt32(attributes.Name) : 0;
								dynamic priceObject = isNested ? attributes.Value : attributes;

                                Price price = new Price();
								double lowPrice = priceObject["value"].Value;
								double highPrice = priceObject["value_high"] == null ? lowPrice : priceObject["value_high"].Value;

								String currency = priceObject["currency"].Value;

                                //normalise to refined
                                if (currency == "earbuds")
                                {
                                    lowPrice *= Price.BudsPrice;
                                    highPrice *= Price.BudsPrice;
                                }
                                else if (currency == "keys")
                                {
                                    lowPrice *= Price.KeyPrice;
                                    highPrice *= Price.KeyPrice;
                                }
                                else if (currency == "usd")
                                {
                                    lowPrice *= Price.RefPrice;
                                    highPrice *= Price.RefPrice;
                                }
                                price.LowRefPrice = lowPrice;
                                price.HighRefPrice = highPrice;

                                //separate australiums, unusuals/crates, and the other one
                                schema.PriceList[GetItemKey(quality, defIndex, isTradable, isCraftable, isAus, series)] = price;
                            }
                        }
                    }
                }
            }
            return schema;
        }

        public static String GetItemKey(Item i)
        {
            return GetItemKey(i.Quality, i.DefIndex, i.IsTradable, i.IsCraftable, i.Attributes.Keys.Contains(2027),
                (i.Quality == Quality.Unusual  && i.DefIndex != 267) ? (int)i.Attributes[134].FloatValue : 0);
            //stupid haunted metal scrap
        }

        public static String GetItemKey(Quality quality, int defindex, bool istradable, bool iscraftable, bool isAus = false, int series = 0)
        {
            if (isAus)
                return String.Join("|", quality, defindex, istradable, iscraftable, "Australium");
            else if (series != 0)
                return String.Join("|", quality, defindex, istradable, iscraftable, series);
            else
                return String.Join("|", quality, defindex, istradable, iscraftable);
        }

        public Price GetPrice(Item i)
        {
            Price price;
            PriceList.TryGetValue(GetItemKey(i), out price);
            //if no price, return unpriced
            return price ?? Price.Unpriced;
        }

        //helper method for parts
        public Price GetUniquePriceByDefindex(int defindex)
        {
            Price price;
            PriceList.TryGetValue(GetItemKey(Quality.Unique, defindex, true, true), out price);
            return price ?? Price.Unpriced;
        }
    }
}
