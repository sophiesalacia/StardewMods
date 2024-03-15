using System.Collections.Generic;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;

namespace KitchenCounters;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += LoadAssets;
    }

    private static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {

    }
}
