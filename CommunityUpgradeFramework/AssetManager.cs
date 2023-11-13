using System;
using System.Collections.Generic;
using StardewModdingAPI.Events;

namespace CommunityUpgradeFramework;

internal static class AssetManager
{
    internal static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.CommunityUpgradesPath))
        {
            e.LoadFrom(
                () => new Dictionary<string, CommunityUpgrade>(),
                AssetLoadPriority.Medium
            );
        }
    }
}
