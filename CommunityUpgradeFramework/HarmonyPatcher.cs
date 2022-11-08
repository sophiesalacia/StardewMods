using System;
using HarmonyLib;

namespace CommunityUpgradeFramework;

internal class HarmonyPatcher
{
    internal static readonly Harmony Harmony = new Harmony(Globals.UUID);

    internal static void ApplyPatches()
    {
        Log.Trace("Patching methods.");

        try
        {
            Harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Log.Error($"Exception encountered while patching: {ex}");
        }

    }
}
