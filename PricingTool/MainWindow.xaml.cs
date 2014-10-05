using SharpTF2.Items;
using SharpTF2.Prices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PricingTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Load(object sender, RoutedEventArgs e)
		{
			String profID = "76561198034183306";
			ItemSchema items = ItemSchema.GetFromFile();
			PriceSchema prices = PriceSchema.GetFromFile();
			Backpack bp = Backpack.GetFromFile(profID);
			bp.LoadSchema(items);
			bp.LoadPrices(items, prices);

			listView.ItemsSource = bp.Items.Select(i => new DisplayItem(i, items));
			GridView gridView = (listView.View as GridView);

			// This variable will hold the longest string from the source list.
			string longestItem = bp.Items.OrderByDescending(s => s.Name.Length).First().Name;
			string longestQuality = ((Quality[])Enum.GetValues(typeof(Quality))).OrderByDescending(s => s.ToString().Length).First().ToString();
			ResizeGridViewColumn(longestQuality, gridView.Columns[0]);
			//ResizeGridViewColumn(longestItem, gridView.Columns[1]);

			CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
			PropertyGroupDescription groupDescription = new PropertyGroupDescription("PageName");
			view.GroupDescriptions.Add(groupDescription);

		}

		private void ResizeGridViewColumn(string widest, GridViewColumn column)
		{
			// Initialize a new typeface based on the currently used font.
			var typeFace = new Typeface(listView.FontFamily, listView.FontStyle,
										listView.FontWeight, listView.FontStretch);

			// Initialize a new FormattedText instance based on our longest string.
			var text = new System.Windows.Media.FormattedText(widest,
							   System.Globalization.CultureInfo.CurrentCulture,
							   System.Windows.FlowDirection.LeftToRight, typeFace,
							   listView.FontSize, listView.Foreground);

			// Assign the width of the FormattedText to the column width.
			column.Width = text.Width;
		}
	}
}
