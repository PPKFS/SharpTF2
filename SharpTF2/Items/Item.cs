using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Items
{
    /// <summary>
    /// A Team Fortress 2 item. Contains optional attributes which can be both item-related (paint, gifted-ness) or price related (low price, high price)
    /// </summary>
    public class Item
    {
        public static Item CreateItem(uint id, uint original_id, int defindex, int level, int quality, uint position, int origin, 
            bool cannotTrade, bool cannotCraft, Dictionary<int, int> equip, List<Attribute> attribs)
        {
            Item item = new Item();
            item.ID = id;
            item.OriginalID = original_id;
            item.DefIndex = defindex;
            item.Level = level;
            item.Quality = (Quality)quality;
            item.Origin = (Origin)origin;
            item.IsCraftable = !cannotCraft;
            item.IsTradable = !cannotTrade;
            item.Position = position;
            if (attribs != null)
            {
                foreach (Attribute a in attribs)
                    item.AddAttribute(a.DefIndex, a);
            }
            if (equip != null)
            {
                foreach (KeyValuePair<int, int> k in equip)
                    item.AddEquipSlot(k.Key, k.Value);
            }
            return item;
        }

        public int DefIndex { get; set; }

        public uint ID { get; set; }

        public uint OriginalID { get; set; }

        public String Name { get; set; }

        public int Level { get; set; }

        public Quality Quality { get; set; }

        public int Quantity { get; set; }

        public Origin Origin { get; set; }

        public bool IsCraftable { get; set; }

        public bool IsTradable { get; set; }

        public uint Position { get; set; }

        public Dictionary<int, int> EquipSlots { get; set; }

        public Dictionary<int, Attribute> Attributes { get; set; }

        public Item()
        {
            Attributes = new Dictionary<int, Attribute>();
            EquipSlots = new Dictionary<int, int>();
        }

        public void AddEquipSlot(int slot, int val)
        {
            EquipSlots[slot] = val;
        }

        public void AddAttribute(int id, Attribute value)
        {
            Attributes[id] = value;
        }
    }
}
