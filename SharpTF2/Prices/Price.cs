using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Prices
{
    public class Price
    {
        public static Price Unpriced = new Price();

        public static double RefPrice;
        public static double KeyPrice;
        public static double BudsPrice;

        public double LowRefPrice { get; set; }

        public double HighRefPrice { get; set; }

        public override string ToString()
        {
            return LowRefPrice + " - " + HighRefPrice + " ref";
        }
    }
}
