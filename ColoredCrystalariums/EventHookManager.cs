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
        Globals.EventHelper.GameLoop.GameLaunched += InitializeGmcmMenu;
    }
    

    /// <summary>
    /// Tries to get and utilize all APIs.
    /// </summary>
    private static void InitializeGmcmMenu(object sender, GameLaunchedEventArgs e)
    {
        if (Globals.InitializeGmcmApi())
        {
            GenericModConfigMenuHelper.BuildConfigMenu();
        }
        else
        {
            Log.Info("Failed to fetch GMCM API, skipping config menu setup.");
        }
    }
}
