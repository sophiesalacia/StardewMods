using HarmonyLib;
using SObject = StardewValley.Object;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace DontGiveMeGarbage;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter

[HarmonyPatch]
public class Patches
{
    [HarmonyPatch(nameof(GameLocation), nameof(GameLocation.TryGetGarbageItem))]
    [HarmonyPostfix]
    public static void GameLocation_TryGetGarbageItem_Postfix(GameLocation __instance, ref Item item)
    {
        if (item is null)
            return;

        item.modData["sophie.DontGiveMeGarbage/ItemFromTrash"] = bool.TrueString;
    }

    [HarmonyPatch(nameof(NPC), nameof(NPC.TryGetDialogue))]
    [HarmonyPostfix]
    public static void NPC_TryGetDialogue_Postfix(NPC __instance, SObject gift, int taste, ref Dialogue __result)
    {
        // back out if ungiftable
        if (__result is null)
            return;


    }
}

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0060 // Remove unused parameter
