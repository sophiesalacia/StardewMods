using System.Collections.Generic;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;

namespace CommunityUpgradeFramework;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += AssetManager.LoadAssets;
    }
    
}
