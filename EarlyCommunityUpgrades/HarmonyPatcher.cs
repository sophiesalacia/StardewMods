using System;
using HarmonyLib;

namespace EarlyCommunityUpgrades;

internal class HarmonyPatcher
{
    internal static readonly Harmony Harmony = new(Globals.UUID);

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
