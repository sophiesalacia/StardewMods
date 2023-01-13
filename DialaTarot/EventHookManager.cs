using StardewValley;

namespace DialaTarotCSharp;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += OnGameLaunched;
        Globals.EventHelper.Content.AssetRequested += AssetManager.LoadOrEditAssets;
        Globals.EventHelper.GameLoop.DayEnding += (_, _) =>
        {
            Game1.player.modData.Remove("sophie.DialaTarot/ReadingDoneForToday");
        };
    }

    private static void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        HarmonyPatcher.ApplyPatches();
    }
}
