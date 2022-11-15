using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityUpgradeFramework
{
    internal class CommunityUpgrade
    {
        public string Location;
        public string Name;
        public string Description;
        public Dictionary<int, int> ItemPriceDict;
        public Dictionary<string, int> CurrencyPriceDict;
        public string ThumbnailPath;
    }
}
