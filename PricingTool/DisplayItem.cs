using SharpTF2.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PricingTool
{
	public class DisplayItem
	{
		public Item Item { get; set; }

		public static ItemSchema Schema;

		public static Brush ColorFromContents(String contents)
		{
			string[] teamColours = {"Team Spirit",
            "Operator's Overalls",
            "Lab Coat",
            "Balaclavas",
            "Air of Debonair",
            "Value of Teamwork",
            "Cream Spirit"};

			Dictionary<string, Color> teamEndColours = new Dictionary<string,Color>(){
					{"Team Spirit", (Color)ColorConverter.ConvertFromString("#5885A2")},
					{"Operator's Overalls", (Color)ColorConverter.ConvertFromString("#384248")},
					{"Lab Coat", (Color)ColorConverter.ConvertFromString("#839FA3")},
					{"Balaclavas", (Color)ColorConverter.ConvertFromString("#18233D")},
					{"Air of Debonair", (Color)ColorConverter.ConvertFromString("#28394D")},
					{"Value of Teamwork", (Color)ColorConverter.ConvertFromString("#256D8D")},
					{"Cream Spirit", (Color)ColorConverter.ConvertFromString("#B88035")}
			};

			int id = Schema.PaintNames.First(i => i.Value == contents).Key;
			if(teamColours.Contains(contents))
			{
				Color startColor = (Color)ColorConverter.ConvertFromString("#" + id.ToString("X"));
				Color endColor = teamEndColours[contents];
				return new LinearGradientBrush(startColor, endColor, 0f);
			}

			string hexString = id.ToString("X");
			Color color = (Color)ColorConverter.ConvertFromString("#"+hexString);
			return new SolidColorBrush(color);
		}

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
				if (NeedsSpecialSuffix)
					name += SpecialNameSuffix;
				return name;
			}
			set
			{

			}
		}

		public bool NeedsSpecialSuffix
		{
			get
			{
				return Item.HasAttribute(SharpTF2.Items.Attribute.Paint);
			}
		}

		public string SpecialNameSuffix
		{
			get
			{
				return Item.HasAttribute(SharpTF2.Items.Attribute.Paint) ? 
					" (|COLOUR"+Schema.PaintNames[(int)Item.Attributes[SharpTF2.Items.Attribute.Paint].FloatValue]+"COLOUR|)"
					: "";
			}
		}

		public DisplayItem(Item i)
		{
			Item = i;
		}
	}
}
