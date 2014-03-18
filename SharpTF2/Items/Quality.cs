using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Items
{
    /// <summary>
    /// An enum of possible item qualities. UncraftableXXX variants are only used for querying the backpack.tf schema and are not used otherwise.
    /// </summary>
    public enum Quality
    {
        Normal = 0,
        Genuine = 1,
        Vintage = 3,
        Unusual = 5,
        Unique = 6,
        Community = 7,
        Valve = 8,
        SelfMade = 9,
        Strange = 11,
        Haunted = 13,
        Collectors = 14,
        UncraftableVintage = 300, //This is a very niche quality
        Uncraftable = 600, //these 3 bottom qualities only exist for the sake of being nice with backpack.tf
        UncraftableStrange = 1100
    }
}
