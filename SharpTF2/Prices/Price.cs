using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Prices
{
    public class PriceBonus
    {
        public String Name { get; set; }
        public Price BonusPrice { get; set; }
    }

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

        public Price(Price price)
        {
            this.LowRefPrice = price.LowRefPrice;
            this.HighRefPrice = price.HighRefPrice;
        }

        public Price()
        {
        }

        public static Price operator +(Price a, Price b)
        {
            return new Price() { HighRefPrice = a.HighRefPrice + b.HighRefPrice, LowRefPrice = a.LowRefPrice + b.LowRefPrice };
        }

        public static Price operator *(Price a, double b)
        {
            return new Price() { HighRefPrice = a.HighRefPrice * b, LowRefPrice = a.LowRefPrice * b};
        }
    }
}
