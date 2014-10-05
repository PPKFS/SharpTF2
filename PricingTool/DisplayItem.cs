using SharpTF2.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingTool
{
	public class DisplayItem
	{
		public Item Item { get; set; }

		private ItemSchema schema;

		public string PageName
		{
			get
			{
				return Item.PageNo == 0 ? "Unplaced" : "Page " + Item.PageNo.ToString();
			}
		}

		public string DisplayQuality
		{
			get
			{
				return Item.Quality == Quality.Unique ? "" : Item.Quality.ToString();
			}
		}

		public string Name
		{
			get
			{
				string name = Item.Name.Replace('-', '‐'); //microsoft please fix your stupid bugs.
				string paint = Item.Attributes[SharpTF2.Items.Attribute.Paint].Value;
				return name;
			}
		}

		public DisplayItem(Item i, ItemSchema schema)
		{
			Item = i;
			this.schema = schema;
		}
	}
}
