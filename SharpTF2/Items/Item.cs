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
        public int DefIndex { get; private set; }

        public int ID { get; private set; }

        public int OriginalID { get; private set; }

        public String Name { get; private set; }

        public int Level { get; private set; }

        public Quality Quality { get; private set; }

        public int Quantity { get; private set; }
    }
}
