using SharpTF2.Prices;
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

        public int DefIndex { get; private set; }

        public uint ID { get; private set; }

		public uint OriginalID { get; private set; }

		public String Name { get; private set; }

		public ItemType Type { get; private set; }

		public Price BasePrice { get; private set; }

		public List<PriceBonus> PriceBonuses { get; private set; }

		public int Level { get; private set; }

		public Quality Quality { get; private set; }

		public int Quantity { get; private set; }

		public Origin Origin { get; private set; }

		public bool IsCraftable { get; private set; }

		public bool IsTradable { get; private set; }

		public uint Position { get; private set; }

		public Dictionary<int, int> EquipSlots { get; private set; }

		public Dictionary<int, Attribute> Attributes { get; private set; }

        public Price Price
        {
            get
            {
                Price totalPrice = new Price(BasePrice);
                foreach (PriceBonus bonus in PriceBonuses)
                    totalPrice += bonus.BonusPrice;
                return totalPrice;
            }
        }

        public Item()
        {
            Attributes = new Dictionary<int, Attribute>();
            EquipSlots = new Dictionary<int, int>();
            PriceBonuses = new List<PriceBonus>();
        }

        public void AddEquipSlot(int slot, int val)
        {
            EquipSlots[slot] = val;
        }

        public void AddAttribute(int id, Attribute value)
        {
            Attributes[id] = value;
        }

        public bool HasAttribute(int id)
        {
            return Attributes.Keys.Contains(id);
        }

        public void AddPriceBonus(String name, Price price)
        {
            this.PriceBonuses.Add(new PriceBonus() { Name = name, BonusPrice = price });
        }
    }
}
