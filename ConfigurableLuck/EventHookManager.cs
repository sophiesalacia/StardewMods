using StardewModdingAPI.Events;
using StardewValley;

namespace ConfigurableLuck;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.Helper.Events.GameLoop.DayStarted += SetLuck;
        Globals.EventHelper.GameLoop.GameLaunched += InitializeGmcmMenu;
    }

    private static void SetLuck(object sender, DayStartedEventArgs e)
    {
        if (!Globals.Config.Enabled)
            return;

        LuckManager.SetLuck(Game1.player, Globals.Config.LuckValue);
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
}
