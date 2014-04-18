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

        public static PriceSchema LoadFromFile(String filename="bptf.txt")
        {
            String json;
            using (StreamReader reader = new StreamReader(filename))
                json = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<PriceSchema>(json);
        }

        public static PriceSchema Get(String apiKey)
        {
            BackpackTFRequest request = new BackpackTFRequest();
            request.APIKey = apiKey;
            return PriceSchema.Get(apiKey, request);
        }

        public static PriceSchema Get(String apiKey, IRequest request)
        {
            //get the json
            String raw = request.GetJSON();
            dynamic fullJson = JsonConvert.DeserializeObject(raw);
            dynamic json = fullJson.response;

            PriceSchema schema = new PriceSchema();

            //first, get the raw prices of buds/keys/ref.
            Price.RefPrice = json.raw_usd_value;
            Price.KeyPrice = json["items"]["Mann Co. Supply Crate Key"]["prices"]["6"]["Tradable"]
                ["Craftable"]["0"]["value_raw"];
            Price.BudsPrice = json["items"]["Earbuds"]["prices"]["6"]["Tradable"]["Craftable"]["0"]["value_raw"];

            foreach (dynamic item in json.items)
            {
                bool isAus = item.Name.StartsWith("Australium");
                int defIndex = (int)item.Value.defindex["0"].Value;
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
                            //this is 0 for most things, but some things like unusuals and crates have values
                            foreach (dynamic attributes in craftableItem.Value)
                            {
                                //ignore it if it's 0.
                                int series = Convert.ToInt32(attributes.Name);

                                Price price = new Price();
                                double lowPrice = attributes.Value.value.Value;
                                double highPrice = attributes.Value.value_high == null ? lowPrice : attributes.Value.value_high.Value;

                                String currency = attributes.Value.currency.Value;

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
            else if (series == 0)
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

        public void GetPrice(int i)
        {
            /*if (i.IsCraftable)
            {
                if(i.Quality == Quality.Unusual)
                    return GetPrice(i.DefIndex, i.Quality, (int)i.Attributes[134].FloatValue);
                else
                    return GetPrice(i.DefIndex, i.Quality);
            }
            else
            {
                switch (i.Quality)
                {
                    case Quality.Strange:
                        return GetPrice(i.DefIndex, Quality.UncraftableStrange, 0);
                    case Quality.Vintage: //a very special edge case
                        return GetPrice(i.DefIndex, Quality.UncraftableVintage, 0);
                    default: //normal uncraft
                        return GetPrice(i.DefIndex, Quality.Uncraftable, 0);
                }
            }*/
        }

        /*private void BuildPriceList()
        {
            string returnedJSON = String.Empty;
            String data = "bptf.txt";
            using (StreamReader sr = new StreamReader(data))
            {
                returnedJSON = sr.ReadToEnd();
            }

            JObject bptfRaw = JObject.Parse(returnedJSON);
            JObject itemPriceListRaw = bptfRaw["response"]["prices"].Value<JObject>();
            foreach (KeyValuePair<String, JToken> itemEntry in itemPriceListRaw)
            {
                int defIndex = Convert.ToInt32(itemEntry.Key);
                foreach (JProperty qualityEntry in itemEntry.Value)
                {
                    if (qualityEntry.Name == "alt_defindex") //no idea what the hell this does
                        continue;
                    Quality quality = (Quality)Enum.Parse(typeof(Quality), qualityEntry.Name);

                    //now we view each effect. 
                    //normal stuff appears as 0, some weird stuff like 4 like the sparkle lugers, unusuals have their own ones
                    foreach (JProperty effectItem in qualityEntry.Value)
                    {
                        ItemPricingTemplate priceTemplate = new ItemPricingTemplate();
                        priceTemplate.DefIndex = defIndex;
                        priceTemplate.Quality = quality;
                        priceTemplate.Effect = Convert.ToInt32(effectItem.Name);
                        Price p = new Price();
                        double low = effectItem.Value["current"]["value"].Value<double>();
                        double high = low;
                        if (effectItem.Value["current"]["value_high"] != null)
                            high = effectItem.Value["current"]["value_high"].Value<double>();

                        //now convert into ref
                        String currType = effectItem.Value["current"]["currency"].Value<String>();
                        switch (currType)
                        {
                            case "keys":
                                p.LowRefinedPrice = low * Price.KeyPrice;
                                p.HighRefinedPrice = high * Price.KeyPrice;
                                break;
                            case "metal":
                                p.LowRefinedPrice = low;
                                p.HighRefinedPrice = high;
                                break;
                            case "earbuds":
                                p.LowRefinedPrice = low * Price.KeyPrice * Price.BudsPrice;
                                p.HighRefinedPrice = high * Price.KeyPrice * Price.BudsPrice;
                                break;
                            case "usd": //seems to be both unusuals and refined
                                p.LowRefinedPrice = low / Price.RefPrice;
                                p.HighRefinedPrice = high / Price.RefPrice;
                                break;
                            default:
                                System.Diagnostics.Debugger.Break();
                                break;
                        }
                        PriceList.Add(priceTemplate.ToString(), p);
                    }
                }
                //System.Diagnostics.Debugger.Break();
            }
            //try struct
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            new
            {
                ItemSchema = Schema.ItemSchema,
                UnusualNames = Schema.UnusualNames,
                PaintIDs = Schema.PaintIDs,
                PaintNames = Schema.PaintNames,
                StrangePartNames = Schema.StrangePartNames,
                DefaultVintageLevels = Schema.DefaultVintageLevels
            });

            using (StreamWriter writer = new StreamWriter(TF2PricerMain.PriceLocation))
                writer.Write(json);
        }

        public Price GetPaintPrice(string p)
        {
            Price price;
            PriceList.TryGetValue(String.Join("|", TF2PricerMain.Schema.PaintIDs[Convert.ToInt32(p)], Quality.Unique, 0), out price);
            if (price == null)
                return Price.Unpriced;
            else
                return new Price() { LowRefinedPrice = price.LowRefinedPrice / 2, HighRefinedPrice = price.HighRefinedPrice / 2 };
        }

        public Price GetPartPrice(string p)
        {
            Price price;
            if (p == null)
                return Price.Unpriced;
            PriceList.TryGetValue(String.Join("|", TF2PricerMain.Schema.StrangePartIDs[Convert.ToInt32(p)], Quality.Unique, 0), out price);
            if (price == null)
                return Price.Unpriced;
            else
                return new Price() { LowRefinedPrice = price.LowRefinedPrice / 2, HighRefinedPrice = price.HighRefinedPrice / 2 };
        }*/
    }
}
