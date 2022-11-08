using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;

namespace ColoredCrystalariums;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.SaveLoaded += AssetManager.InitializeAssets;
        Globals.EventHelper.Content.AssetRequested += AssetManager.LoadAssets;
        Globals.EventHelper.Content.AssetReady += AssetManager.UpdateAssets;
    }
}
