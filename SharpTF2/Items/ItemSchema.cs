using Newtonsoft.Json.Linq;
using SharpTF2.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Items
{
    public enum ItemType
    {
        Weapon,
        Cosmetic,
        Tool
    }

    public class ItemTemplate
    {
        public String Name { get; set; }

        public ItemType Type { get; set; }
    }
    /// <summary>
    /// The schema of all Team Fortress 2 items.
    /// </summary>
    public class ItemSchema
    {
        //schema of items, mapped by index
        public Dictionary<int, ItemTemplate> Items = new Dictionary<int, ItemTemplate>(); //DONE

        //mapping of particle effect ID to unusual effect names
        public Dictionary<int, String> UnusualNames = new Dictionary<int, String>();

        //mapping of strange part ID (kill eater) to their names
        public Dictionary<int, String> StrangePartNames = new Dictionary<int, String>();

        //maps s part ID to their defindices
        public Dictionary<int, int> StrangePartIDs = new Dictionary<int, int>();

        //mapping of paint colours (some form of hex code) to their defindices
        public Dictionary<int, int> PaintIDs = GetPaintIDs(); //DONE

        //mapping of paint defindices to (shorthand) names; e.g. Black not ...Hue.
        public Dictionary<int, string> PaintNames = GetPaintNames(); //DONE

        //mapping of defindices to their default levels (for vintage weapons)
        public Dictionary<int, int> DefaultVintageLevels = new Dictionary<int, int>(); //DONE

        public static ItemSchema Get(IRequest request)
        {
            //get the json
            String raw = request.GetJSON();
            Cache.SaveJSON("schema.txt", raw);
            JToken json = JObject.Parse(raw)["result"];
            ItemSchema schema = new ItemSchema();

            foreach (JToken item in json["items"])
            {
                ItemTemplate template = new ItemTemplate();
                template.Name = item["item_name"].ToObject<String>();
                int defIndex = item["defindex"].ToObject<int>();

                if (template.Name.StartsWith("Strange Part:"))
                {
                    int id = item["attributes"][0]["value"].ToObject<int>();
                    schema.StrangePartIDs.Add(id, defIndex);
                    schema.StrangePartNames.Add(id, template.Name.Substring(template.Name.IndexOf(':')+2));
                }

                String type = item["item_slot"] == null ? "other" : item["item_slot"].ToObject<String>();
                switch (type)
                {
                    //weapons!
                    case "primary":
                    case "secondary":
                    case "melee":
                    case "pda":
                    case "pda2":
                    case "building":
                        template.Type = ItemType.Weapon;
                        //if it's got the same values, then add it to the vintage chart (we can safely ignore basically everything else)
                        //of course, this does give a few things that you can't get in vintage or is entirely pointless, but *shrug*
                        if (item["min_ilevel"].ToObject<int>() == item["max_ilevel"].ToObject<int>())
                            schema.DefaultVintageLevels.Add(defIndex, item["min_ilevel"].ToObject<int>());
                        break;
                    case "head":
                    case "misc":
                        template.Type = ItemType.Cosmetic;
                        break;
                    default:
                        template.Type = ItemType.Tool;
                        break;
                }
                schema.Items.Add(defIndex, template);
            }
            

            return null;
        }

        private static Dictionary<int, String> GetPaintNames()
        {
            return new Dictionary<int,string>(){
            {7511618, "Indub. Green"},
            {4345659, "Greed"},
            {5322826, "Violet"},
            {14204632, "216-190-216"},
            {8208497, "Purple"},
            {13595446, "Orange"},
            {10843461, "Muskelmannbraun"},
            {12955537, "Drab"},
            {6901050, "Brown"},
            {8154199, "Rustic"},
            {15185211, "Gold"},
            {8289918, "Grey"},
            {15132390, "White"},
            {1315860, "Black"},
            {16738740, "Pink"},
            {3100495, "Slate"},
            {8421376, "Olive"},
            {3329330, "Lime"},
            {15787660, "Business Pants"},
            {15308410, "Salmon"},
            {12073019, "Team Spirit"},
            {4732984, "Operator's Overalls"},
            {11049612, "Lab Coat"},
            {3874595, "Balaclavas"},
            {6637376, "Air of Debonair"},
            {8400928, "Value of Teamwork"},
            {12807213, "Cream Spirit"},
            {2960676, "After Eight"},
            {1, "Team Spirit"}, //glitched TS
            {12377523, "Mint"}};
        }

        private static Dictionary<int, int> GetPaintIDs()
        {
            return new Dictionary<int, int>()
            {
                {7511618, 5027},
                {4345659, 5028},
                {5322826, 5029},
                {14204632, 5030},
                {8208497, 5031},
                {13595446, 5032},
                {10843461, 5033},
                {12955537, 5034},
                {6901050, 5035},
                {8154199, 5036},
                {15185211, 5037},
                {8289918, 5038},
                {15132390, 5039},
                {1315860, 5040},
                {16738740, 5051},
                {3100495, 5052},
                {8421376, 5053},
                {3329330, 5054},
                {15787660, 5055},
                {15308410, 5056},
                {12073019, 5046},
                {4732984, 5060},
                {11049612, 5061},
                {3874595, 5062},
                {6637376, 5063},
                {8400928, 5064},
                {12807213, 5065},
                {2960676, 5077},
                {12377523, 5076},
                {1, 5046},//'bugged' team spirit
                {0, 0}
            };
        }
    }
}
