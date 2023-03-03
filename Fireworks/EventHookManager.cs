namespace Fireworks;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += OnGameLaunched;
    }

    private static void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        HarmonyPatcher.ApplyPatches();
    }
}
