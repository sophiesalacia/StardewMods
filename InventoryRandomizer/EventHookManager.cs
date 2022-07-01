using StardewModdingAPI.Events;

namespace InventoryRandomizer;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.SaveLoaded += ReloadObjectData;
        Globals.EventHelper.Content.AssetReady += ReloadObjectData;
        Globals.EventHelper.GameLoop.OneSecondUpdateTicked += TimeManager.OnOneSecondUpdateTicked;
        Globals.EventHelper.GameLoop.GameLaunched += InitializeGmcmMenu;
    }

    /// <summary>
    ///     Tries to get and utilize all APIs.
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

    private static void ReloadObjectData(object sender, SaveLoadedEventArgs e)
    {
        Log.Info("Reloading object data for all supported assets.");
        AssetManager.ReloadObjectData();

        // reset timer, chatbox just to be safe
        TimeManager.ResetTimer();
        TimeManager.RegrabChatbox();
    }

    private static void ReloadObjectData(object sender, AssetReadyEventArgs e)
    {
        if (!AssetManager.AssetIsSupported(e.Name.BaseName))
            return;

        Log.Info($"Reloading data for {e.Name.BaseName}.");
        AssetManager.ReloadObjectData(e.Name.BaseName);
    }
}
