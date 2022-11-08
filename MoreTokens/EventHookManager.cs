using System;

namespace MoreTokens;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += OnGameLaunched;
    }

    private static void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        HarmonyPatcher.ApplyPatches();

        if (!Globals.InitializeCpApi())
        {
            Log.Warn("Unable to locate ContentPatcher API. Aborting token registration.");
            return;
        }

        TokenManager.RegisterTokens();
    }
}