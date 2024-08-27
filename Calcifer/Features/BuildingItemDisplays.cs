//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using HarmonyLib;
//using Microsoft.Xna.Framework;
//using Netcode;
//using StardewModdingAPI;
//using StardewModdingAPI.Events;
//using StardewValley;
//using StardewValley.Buildings;
//using StardewValley.GameData.Buildings;
//using StardewValley.GameData.Objects;
//using StardewValley.Mods;

//#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
//#pragma warning disable IDE1006 // Naming Styles
//#pragma warning disable CS8600

//// ReSharper disable InconsistentNaming
//// ReSharper disable UnusedMember.Global
//// ReSharper disable RedundantAssignment

//namespace Calcifer.Features;

//[HarmonyPatch]
//class BuildingItemDisplayPatches
//{
//    internal static readonly List<TemporaryAnimatedSprite> ItemDisplayList = new();
//    internal const string CategoryCustomField = "sophie.Calcifer/BuildingItemDisplay";

//    //[HarmonyPatch(typeof(Object), nameof(Object.getCategoryName))]
//    //[HarmonyPrefix]
//    //public static bool getCategoryName_Prefix(ref Object __instance, ref string __result, ref bool __state)
//    //{
//    //return false;
//    //}
//}

//class BuildingItemDisplayHooks
//{
//    private const string TileActionName = "sophie.Calcifer/BuildingItemDisplay";
//    private static readonly List<TemporaryAnimatedSprite> ItemDisplayList = new();

//    internal static void InitHooks()
//    {
//        Globals.EventHelper.Player.Warped += RebuildCache;
//    }

//    private static void RebuildCache(object? sender, WarpedEventArgs e)
//    {
//        foreach (var itemTas in ItemDisplayList)
//        {
//            e.OldLocation.TemporarySprites.Remove(itemTas);
//        }

//        foreach (Building building in e.NewLocation.buildings)
//        {
//            //bool found = building.modData.TryGetValue(TileActionName, out string value);

//            // figure out how the fuck to store and retrieve the data

//            var myDict = building.modData.FirstOrDefault(dictionary => dictionary.ContainsKey("sophie.Calcifer/BuildingItemDisplayData"));

//            if (myDict is null)
//                continue;
            

//            BuildingData buildingData = building.GetData();

//            // default action - check custom fields for matching index
//            // moddata - index -> position

//            if (buildingData.DefaultAction.StartsWith(TileActionName) && myDict.TryGetValue("sophie.Calcifer/BuildingItemDisplayData/Default", out string itemId))
//            {
//                if (!string.IsNullOrEmpty(itemId) && ItemRegistry.Exists(itemId))
//                {
//                    TemporaryAnimatedSprite itemSprite = new(null, Rectangle.Empty, Vector2.Zero, flipped: false, 0f, Color.White)
//                    {
//                        layerDepth = 0.135f
//                    };
//                    itemSprite.CopyAppearanceFromItemId(itemId);

//                    ItemDisplayList.Add(itemSprite);
//                }
//            }

//        }
//    }
//}

//#pragma warning restore IDE1006 // Naming Styles
//#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
