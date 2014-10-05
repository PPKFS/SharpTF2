using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace PricingTool
{
	public static class FormattedTextBehavior
	{
		public static string GetFormattedText(DependencyObject obj)
		{
			return (string)obj.GetValue(FormattedTextProperty);
		}

		public static void SetFormattedText(DependencyObject obj, string value)
		{
			obj.SetValue(FormattedTextProperty, value);
		}

		public static readonly DependencyProperty FormattedTextProperty =
			DependencyProperty.RegisterAttached("FormattedText",
												typeof(string),
												typeof(FormattedTextBehavior),
												new UIPropertyMetadata("", FormattedTextChanged));

		private static void FormattedTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			TextBlock textBlock = sender as TextBlock;
			string value = e.NewValue as string;
			string[] tokens = value.Split('|');
			foreach (string token in tokens)
			{
				if (token.StartsWith("COLOUR") && token.EndsWith("COLOUR"))
				{
					string tokenContents = token.Replace("COLOUR", "");
					textBlock.Inlines.Add(new Run(tokenContents) { Foreground = DisplayItem.ColorFromContents(tokenContents) });
				}
				else
				{
					textBlock.Inlines.Add(new Run(token));
				}
			}
		}
	}
}
