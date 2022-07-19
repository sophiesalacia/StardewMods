using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ConfigurableBundleCosts;

internal class BundleManager
{
    [EventPriority(EventPriority.Low)]
    internal static void CheckBundleData()
    {
        try
        {
            // dump and reload the asset to force AssetManager.BundleData to update
            Globals.GameContent.InvalidateCache("Data/Bundles");
            Globals.GameContent.Load<Dictionary<string, string>>("Data/Bundles");
            Game1.netWorldState?.Value?.SetBundleData(AssetManager.BundleData);
        }
        catch (Exception ex)
        {
            Log.Error($"Exception encountered while updating bundle data: {ex}");
        }
    }
}
