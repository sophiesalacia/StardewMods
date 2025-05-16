using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using static StardewValley.Objects.FishTankFurniture;

namespace Calcifer.Features;

/// <summary>
/// Allow any furniture to become a fish tank through a context tag.
/// Has priority low to come after ConnectedTextures hooks
/// </summary>
[HasEventHooks(HookPriority.Low)]
[HarmonyPatch]
class CustomFishTankFurniture
{
    private record FishTankInfo(int Capacity, int PosX, int PosY, int Width, int Height)
    {
        /// <summary>Mark tank bounds as need combining</summary>
        internal bool IsDirty = true;
        /// <summary>Cached tank bounds</summary>
        internal Rectangle CurrentTankBounds = Rectangle.Empty;
        /// <summary>Base tank bounds given a tile location</summary>
        internal Rectangle GetBaseTankBounds(Vector2 tileLocation)
        {
            return new Rectangle(
                (int)(tileLocation.X * Game1.tileSize + PosX),
                (int)(tileLocation.Y * Game1.tileSize + PosY),
                Width,
                Height
            );
        }
    }
    private static readonly ConditionalWeakTable<FishTankFurniture, FishTankInfo?> FishTankInfos = [];

    /// <summary>Pattern for context tag that holds the needed info for tank.</summary>
    private static readonly Regex FishTankContextTag = new Regex(@"^calcifer_fishtank_c(\d+)x(-?\d+)y(-?\d+)w(\d+)h(\d+)$");

    /// <summary>Allow making non type="fishtank" FishTankFurniture, most useful wall tanks via "painting"</summary>
    /// <param name="itemId"></param>
    /// <param name="__result"></param>
    [HarmonyPatch(typeof(Furniture), nameof(Furniture.GetFurnitureInstance))]
    [HarmonyPostfix]
    public static void Furniture_GetFurnitureInstance_Postfix(string itemId, ref Furniture __result)
    {
        if (__result is FishTankFurniture || __result.furniture_type.Value != Furniture.painting)
            return;
        foreach (string tag in __result.GetContextTags())
        {
            if (FishTankContextTag.IsMatch(tag))
            {
                __result = new FishTankFurniture(itemId, __result.TileLocation);
                return;
            }
        }
    }

    /// Calculate the tank bounds when player enters a location, and handle any tanks that should be connected (horizontal only)
    internal static void InitHooks()
    {
        Globals.EventHelper.GameLoop.DayStarted += OnDayStarted;
        Globals.EventHelper.Player.Warped += OnWarped;
        Globals.EventHelper.World.FurnitureListChanged += OnFurnitureListChanged;
    }

    private static void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        UpdateFishTankNeighbourBounds(Game1.currentLocation);
    }

    private static void OnWarped(object? sender, WarpedEventArgs e)
    {
        UpdateFishTankNeighbourBounds(e.NewLocation);
    }

    private static void OnFurnitureListChanged(object? sender, FurnitureListChangedEventArgs e)
    {
        if (e.IsCurrentLocation && (e.Removed.Any(furniture => furniture is FishTankFurniture) || e.Added.Any(furniture => furniture is FishTankFurniture)))
            UpdateFishTankNeighbourBounds(Game1.currentLocation);
    }

    /// <summary>Update fish tank bounds of current location</summary>
    private static void UpdateFishTankNeighbourBounds(GameLocation where)
    {
        if (where == null)
            return;

        Dictionary<FishTankFurniture, FishTankInfo> maybeConnectedTanks = [];
        foreach (Furniture furniture in where.furniture)
        {
            if (furniture is FishTankFurniture tank && FishTankInfos.GetValue(tank, GetFishTankInfo) is FishTankInfo tankInfo)
            {
                tankInfo.CurrentTankBounds = tankInfo.GetBaseTankBounds(tank.TileLocation);
                if (tank.modData.ContainsKey(ConnectedTextures.ConnectedTextureApplied))
                {
                    tankInfo.IsDirty = true;
                    maybeConnectedTanks[tank] = tankInfo;
                }
            }
        }

        foreach ((FishTankFurniture tank, FishTankInfo tankInfo) in maybeConnectedTanks.OrderBy(kv => kv.Key.TileLocation.X))
        {
            if (!tankInfo.IsDirty)
                continue;
            List<(FishTankFurniture, FishTankInfo)> needUpdate = [];
            RightOnlyDFS_FishTank(maybeConnectedTanks, tank, tankInfo, ref needUpdate);
            Rectangle combinedBounds = tankInfo.CurrentTankBounds;
            foreach ((_, FishTankInfo subInfo) in needUpdate)
            {
                combinedBounds = Rectangle.Union(combinedBounds, subInfo.CurrentTankBounds);
            }
            foreach ((FishTankFurniture subTank, FishTankInfo subInfo) in needUpdate)
            {
                subInfo.CurrentTankBounds = combinedBounds;
                foreach (TankFish fish in subTank.tankFish)
                {
                    fish.position.X = Random.Shared.Next(combinedBounds.Width);
                }
                subInfo.IsDirty = false;
            }
        }
    }

    /// <summary>Traverse down right side of tank for connections, works because maybeConnectedTanks is searched in TileLocation.X order</summary>
    private static void RightOnlyDFS_FishTank(Dictionary<FishTankFurniture, FishTankInfo> maybeConnectedTanks, FishTankFurniture tank, FishTankInfo tankInfo, ref List<(FishTankFurniture, FishTankInfo)> needUpdate)
    {
        needUpdate.Add(new(tank, tankInfo));

        if (!ConnectedTextures.Data.TryGetValue(tank.QualifiedItemId, out var data) || data.ConnectWith is not IList<string> connections)
            return;

        if (!ConnectedTextures.ConnectsToSide(
            tank.Location, new(1, 0), ConnectedTextures.FurnitureTileBounds(tank),
            connections, out var found, maybeConnectedTanks.Keys
        ))
            return;

        if (found is not FishTankFurniture tank2)
            return;

        RightOnlyDFS_FishTank(maybeConnectedTanks, tank2, maybeConnectedTanks[tank2], ref needUpdate);
    }

    /// <summary>Convert context tag to FishTankInfo</summary>
    private static FishTankInfo? GetFishTankInfo(FishTankFurniture fishtank)
    {
        foreach (string tag in fishtank.GetContextTags())
        {
            if (FishTankContextTag.Match(tag) is Match ftTag && ftTag.Success &&
                int.TryParse(ftTag.Groups[1].Value, out int capacity) &&
                int.TryParse(ftTag.Groups[2].Value, out int posX) &&
                int.TryParse(ftTag.Groups[3].Value, out int posY) &&
                int.TryParse(ftTag.Groups[4].Value, out int width) &&
                int.TryParse(ftTag.Groups[5].Value, out int height))
            {
                return new FishTankInfo(capacity, posX, posY, width, height);
            }
        }
        return null;
    }

    /// <summary>Patch number of fish allowed</summary>
    [HarmonyPatch(typeof(FishTankFurniture), nameof(FishTankFurniture.GetCapacityForCategory))]
    [HarmonyPostfix]
    public static void FishTankFurniture_GetCapacityForCategory_Postfix(FishTankFurniture __instance, FishTankCategories category, ref int __result)
    {
        if (FishTankInfos.GetValue(__instance, GetFishTankInfo) is FishTankInfo tankInfo && tankInfo.Capacity != -2)
        {
            if (category == FishTankCategories.Decoration)
                __result = -1;
            __result = tankInfo.Capacity;
        }
    }

    /// <summary>Patch tank bounds</summary>
    [HarmonyPatch(typeof(FishTankFurniture), nameof(FishTankFurniture.GetTankBounds))]
    [HarmonyPostfix]
    public static void FishTankFurniture_GetTankBounds_Postfix(FishTankFurniture __instance, ref Rectangle __result)
    {
        if (FishTankInfos.GetValue(__instance, GetFishTankInfo) is FishTankInfo tankInfo)
        {
            if (tankInfo.CurrentTankBounds.Width != 0 && tankInfo.CurrentTankBounds.Height != 0)
            {
                __result = tankInfo.CurrentTankBounds;
            }
        }
    }

    /// <summary>Make fishes in 1 tile (64px) wide tanks not move around in the X direction</summary>
    [HarmonyPatch(typeof(TankFish), nameof(TankFish.Update))]
    [HarmonyPostfix]
    public static void TankFish_Update_Postfix(TankFish __instance, FishTankFurniture ____tank)
    {
        if (____tank.GetTankBounds().Width <= Game1.tileSize)
        {
            if (__instance.fishType == TankFish.FishType.Float)
                __instance.velocity.X = 0;
            __instance.facingLeft = false;
        }
    }
}
