using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace ConfigurableBundleCosts;

internal class EventHookManager
{
    /// <summary>
    /// Sets up initial event handler.
    /// </summary>
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += AssetHandler.LoadOrEditAssets;
        Globals.EventHelper.GameLoop.SaveLoaded += (_, _) => BundleManager.CheckBundleData();
        Globals.EventHelper.GameLoop.SaveCreated += (_, _) => BundleManager.CheckBundleData();
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
