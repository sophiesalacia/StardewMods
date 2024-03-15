using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Triggers;
using xTile.Dimensions;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

#pragma warning disable IDE1006 // Naming Styles

namespace Calcifer.Features;

[HarmonyPatch]
class FurnitureActionPatches
{
    private const string ActionsAssetString = "sophie.Calcifer/CustomFurnitureActions";

    private static Dictionary<string, string>? _customFurnitureActionsAsset;
    internal static Dictionary<string, string> CustomFurnitureActionsAsset
    {
        get => _customFurnitureActionsAsset ??=
            Globals.GameContent.Load<Dictionary<string, string>>(ActionsAssetString);
        set => _customFurnitureActionsAsset = value;
    }

    [HarmonyPatch(typeof(Furniture), nameof(Furniture.checkForAction))]
    [HarmonyPostfix]
    public static void checkForAction_Postfix(Furniture __instance, Farmer who)
    {
        try
        {
            // raise trigger
            TriggerActionManager.Raise(
                trigger: "sophie.CustomFurnitureActions_FurnitureTriggered",
                triggerArgs: new object[] { __instance, who },
                location: __instance.Location,
                player: who,
                inputItem: who.CurrentItem,
                targetItem: __instance
            );

            // do associated tile action stuff
            if (CustomFurnitureActionsAsset.TryGetValue(__instance.QualifiedItemId, out string? action))
            {
                Game1.currentLocation.performAction(action, who, new Location((int)who.Tile.X, (int)who.Tile.Y));
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {nameof(checkForAction_Postfix)}:\n{e}");
        }
    }
}

internal static class FurnitureActionHooks
{
    private const string ActionsAssetString = "sophie.Calcifer/CustomFurnitureActions";
    private static readonly IAssetName ActionsAssetName = Globals.GameContent.ParseAssetName(ActionsAssetString);

    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += (_, _) => TriggerActionManager.RegisterTrigger("sophie.CustomFurnitureActions_FurnitureTriggered");

        // content pipeline
        Globals.EventHelper.Content.AssetRequested += OnAssetRequested;
        Globals.EventHelper.Content.AssetReady += OnAssetReady;
        Globals.EventHelper.Content.AssetsInvalidated += OnAssetsInvalidated;
    }

    private static void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (!e.NamesWithoutLocale.Contains(ActionsAssetName))
            return;

        FurnitureActionPatches.CustomFurnitureActionsAsset = Game1.content.Load<Dictionary<string, string>>(ActionsAssetString);
    }

    private static void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo(ActionsAssetString))
            return;

        FurnitureActionPatches.CustomFurnitureActionsAsset = Game1.content.Load<Dictionary<string, string>>(ActionsAssetString);
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(ActionsAssetString))
            e.LoadFrom(() => new Dictionary<string, string>(), AssetLoadPriority.Low);
    }
}

#pragma warning restore IDE1006 // Naming Styles
