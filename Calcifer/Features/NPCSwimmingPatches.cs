
#pragma warning disable IDE1006 // Naming Styles

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

using HarmonyLib;
using StardewValley;

namespace Calcifer.Features;

[HarmonyPatch]
class NPCSwimmingPatches
{
    [HarmonyPatch(typeof(NPC), "finishRouteBehavior")]
    [HarmonyPostfix]
    public static void finishRouteBehavior_Postfix(NPC __instance, string behaviorName)
    {
        if (behaviorName == $"{__instance.Name}_startSwimming")
        {
            __instance.swimming.Value = true;
        }
    }

    [HarmonyPatch(typeof(NPC), "startRouteBehavior")]
    [HarmonyPostfix]
    public static void startRouteBehavior_Postfix(NPC __instance, string behaviorName)
    {
        if (behaviorName == $"{__instance.Name}_stopSwimming")
        {
            __instance.swimming.Value = false;
        }
    }
}

#pragma warning restore IDE1006 // Naming Styles
