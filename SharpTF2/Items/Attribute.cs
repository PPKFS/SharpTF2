using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Items
{
    public class Attribute
    {
        public int DefIndex;

        public String Value;

        public T ValueToObject<T>()
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        public float FloatValue;

        public SteamInfo SteamInfo = null;
    }

    //we've got a bit of redundancy, but it's not enough to care about
    public class IngredientAttribute : Attribute
    {
        public static int AnyItemOfQuality = 0xDEADBEE;
        public bool IsOutput;

        public int Quantity;

        public int ItemDef;

        public Quality Quality;

        public bool MatchAll;

        public List<Attribute> AttributeInfo = null;
    }

    public class SteamInfo
    {
        public String SteamID;

        public String Name;
    }

    public class AttributeInfo
    {

    }
}
