using System;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;

namespace Calcifer.Features.TriggerActions;

[HasEventHooks]
internal static class CustomTriggerActionHooks
{
    private const string AddBuildingTriggerString = "sophie.Calcifer_AddBuilding";
    private const string AddAnimalTriggerString = "sophie.Calcifer_AddAnimal";

    internal static void InitHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += (_, _) => TriggerActionManager.RegisterAction(AddBuildingTriggerString, AddBuildingTriggerAction.AddBuilding);
        Globals.EventHelper.GameLoop.GameLaunched += (_, _) => TriggerActionManager.RegisterAction(AddAnimalTriggerString, AddAnimal);
    }

    // AddAnimal <AnimalType> [Location]
    private static bool AddAnimal(string[] args, TriggerActionContext context, out string error)
    {
        throw new NotImplementedException();

        error = "";

        Game1.PerformActionWhenPlayerFree(() =>
            {

            }
        );

        return true;
    }
}
