using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Calcifer.Features.TriggerActions;

[HarmonyPatch]
internal static class AddBuildingTriggerActionPatches
{
    private const string AddBuildingTriggerString = "sophie.Calcifer_AddBuilding";

    public static Dictionary<string, List<Vector2>> FurniturePassableTiles = new();

    // tell the game the player has the required resources if menu is from my trigger action
    [HarmonyPatch(typeof(CarpenterMenu), nameof(CarpenterMenu.DoesFarmerHaveEnoughResourcesToBuild))]
    [HarmonyPrefix]
    public static bool CarpenterMenu_DoesFarmerHaveEnoughResourcesToBuild_Prefix(ref CarpenterMenu __instance, ref bool __result)
    {
        if (!__instance.Builder.Equals(AddBuildingTriggerString))
            return true;

        __result = true;
        return false;
    }

    // skip consuming resources if menu is from my trigger action
    [HarmonyPatch(typeof(CarpenterMenu), nameof(CarpenterMenu.ConsumeResources))]
    [HarmonyPrefix]
    public static bool CarpenterMenu_ConsumeResources_Prefix(ref CarpenterMenu __instance)
    {
        return !__instance.Builder.Equals(AddBuildingTriggerString);
    }

    // needed to prevent divide by zero exception if no valid buildings found for provided builder
    [HarmonyPatch(typeof(CarpenterMenu), nameof(CarpenterMenu.SetNewActiveBlueprint), typeof(int))]
    [HarmonyPrefix]
    public static bool CarpenterMenu_SetNewActiveBlueprint_Prefix(ref CarpenterMenu __instance)
    {
        return !__instance.Builder.Equals(AddBuildingTriggerString);
    }
}

internal static class AddBuildingTriggerAction
{
    private const string AddBuildingTriggerString = "sophie.Calcifer_AddBuilding";

    // AddBuilding <Location> <BuildingType> [BuildInstantly = false]
    internal static bool AddBuilding(string[] args, TriggerActionContext context, out string error)
    {
        if (!ArgUtility.TryGet(args, 1, out string locationName, out error, allowBlank: false))
            return false;

        GameLocation buildLocation = locationName.ToLower().Equals("current") ? Game1.currentLocation : Game1.getLocationFromName(locationName);

        if (buildLocation is null)
        {
            error = $"No location found with name matching provided argument \"{locationName}\".";
            return false;
        }

        if (!ArgUtility.TryGet(args, 2, out string buildingType, out error, allowBlank: false))
            return false;

        if (!Game1.buildingData.TryGetValue(buildingType, out BuildingData? buildingData))
        {
            error = $"No building found with type matching provided argument \"{buildingType}\".";
            return false;
        }
        
        if (!ArgUtility.TryGetOptionalBool(args, 3, out bool buildInstantly, out error, defaultValue: false))
            return false;

        Game1.PerformActionWhenPlayerFree(() =>
            {
                if (buildInstantly)
                    buildingData.BuildDays = 0;

                CarpenterMenu.BlueprintEntry blueprint = new(0, buildingType, buildingData, null);

                CarpenterMenu buildMenu = new(AddBuildingTriggerString, buildLocation);
                buildMenu.SetNewActiveBlueprint(blueprint);
                buildMenu.setUpForBuildingPlacement();

                Game1.activeClickableMenu = buildMenu;
            }
        );

        return true;
    }
}
