using System;
using HarmonyLib;

namespace SophieShared;

internal class HarmonyPatcher
{
    internal static Harmony? Harmony;

    internal static void ApplyPatches(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Log.Error($"Failed to acquire UUID, could not apply Harmony patches.");
        }

        Harmony = new Harmony(id);

        Log.Info("Applying Harmony patches.");

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
