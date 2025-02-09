using HarmonyLib;
using StardewModdingAPI.Events;
using StardewValley.Triggers;
using SObject = StardewValley.Object;
// ReSharper disable InconsistentNaming

namespace Calcifer.Features.Triggers;

[HarmonyPatch]
class MachineTriggerPatches
{
    [HarmonyPatch(typeof(SObject), nameof(SObject.PlaceInMachine))]
    [HarmonyPostfix]
    public static void SObject_PlaceInMachine_Postfix(SObject __instance, bool __result, Farmer who, bool probe, Item inputItem)
    {
        if (!__result || probe)
            return;

        // this one at least makes a little more sense
        // target is the machine
        // input is the input
        TriggerActionManager.Raise(
            trigger: "sophie.Calcifer/MachineLoaded",
            triggerArgs: [__instance.QualifiedItemId],
            location: __instance.Location,
            player: who,
            targetItem: __instance,
            inputItem: inputItem
        );
    }

    [HarmonyPatch(typeof(SObject), "CheckForActionOnMachine")]
    [HarmonyPostfix]
    public static void SObject_CheckForActionOnMachine_Postfix(SObject __instance, bool __result, Farmer who, bool justCheckingForActivity)
    {
        if (!__result || justCheckingForActivity)
            return;

        // theoretically
        // location and player are easy
        // target item should be the machine
        // input item SHOULD be what you pull out of the machine
        // which is. not really an input. but i wanted to do it this way for at least some level of consistency between the two triggers
        // so you can check what machine is being interacted with
        TriggerActionManager.Raise(
            trigger: "sophie.Calcifer/MachineHarvested",
            triggerArgs: [__instance.QualifiedItemId],
            location: __instance.Location,
            player: who,
            targetItem: who.mostRecentlyGrabbedItem,
            inputItem: __instance
        );
    }
}

[HasEventHooks]
class MachineTriggerHooks
{
    internal static void InitHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += RegisterTriggers;
    }

    private static void RegisterTriggers(object? sender, GameLaunchedEventArgs e)
    {
        TriggerActionManager.RegisterTrigger("sophie.Calcifer/MachineLoaded");
        TriggerActionManager.RegisterTrigger("sophie.Calcifer/MachineHarvested");
    }
}
