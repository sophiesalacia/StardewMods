//using System;
//using System.Collections.Generic;
//using HarmonyLib;
//using Microsoft.Xna.Framework;
//using StardewModdingAPI;
//using StardewModdingAPI.Events;
//using StardewValley;
//using StardewValley.Locations;
//using StardewValley.Objects;
//using SObject = StardewValley.Object;
//// ReSharper disable UnusedType.Local
//// ReSharper disable RedundantAssignment
//// ReSharper disable UnusedMember.Global
//// ReSharper disable InconsistentNaming

//#pragma warning disable IDE1006 // Naming Styles

//namespace Calcifer.Features;

//class CustomChests
//{
//    private const string CustomChestsAssetString = "sophie.Calcifer/CustomChests";
//    private static readonly IAssetName CustomChestsAssetName = Globals.GameContent.ParseAssetName(CustomChestsAssetString);

//    [HarmonyPatch]
//    static class CustomChestPatches
//    {
//        private static Dictionary<string, CustomChestData>? _customChestsAsset;
//        internal static Dictionary<string, CustomChestData> CustomChestsAsset
//        {
//            get => _customChestsAsset ??=
//                Globals.GameContent.Load<Dictionary<string, CustomChestData>>(CustomChestsAssetString);
//            set => _customChestsAsset = value;
//        }

//        [HarmonyPatch(typeof(SObject), nameof(SObject.placementAction))]
//        [HarmonyPrefix]
//        public static bool Object_placementAction_Prefix(bool __result, SObject __instance, ref GameLocation location, ref int x, ref int y)
//        {
//            try
//            {
//                if (!CustomChestsAsset.TryGetValue(__instance.QualifiedItemId, out CustomChestData? chestData) || chestData is not { })
//                    return true;

//                // make sure fridge can be placed if it is a mini-fridge
//                if (!chestData.IsFridge)
//                    return true;

//                switch (location)
//                {
//                    case not FarmHouse or IslandFarmHouse:
//                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
//                        __result = false;
//                        return false;

//                    case FarmHouse {upgradeLevel: < 1}:
//                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:MiniFridge_NoKitchen"));
//                        __result = false;
//                        return false;
//                }

//                return true;

//            }
//            catch (Exception e)
//            {
//                Log.Error($"Failed in {nameof(Object_placementAction_Prefix)}:\n{e}");
//                return true;
//            }
//        }

//        [HarmonyPatch(typeof(SObject), nameof(SObject.placementAction))]
//        [HarmonyPostfix]
//        public static void Object_placementAction_Postfix(bool __result, SObject __instance, ref GameLocation location, ref int x, ref int y)
//        {
//            try
//            {
//                if (!__result || !CustomChestsAsset.TryGetValue(__instance.QualifiedItemId, out CustomChestData? chestData) || chestData is not { })
//                    return;

//                Vector2 tileLoc = new(x / 64, y / 64);
//                location.Objects.Remove(tileLoc);

//                Chest chest = new(__instance.ItemId, tileLoc, chestData.StartingFrame, chestData.OpeningFrames)
//                {
//                    fridge = { Value = chestData.IsFridge }
//                };

//                location.Objects.Add(tileLoc, chest);
//            }
//            catch (Exception e)
//            {
//                Log.Error($"Failed in {nameof(Object_placementAction_Postfix)}:\n{e}");
//            }
//        }
//    }
    
//    internal static class CustomChestHooks
//    {
//        internal static void InitHooks()
//        {
//            // content pipeline
//            Globals.EventHelper.Content.AssetRequested += OnAssetRequested;
//            Globals.EventHelper.Content.AssetReady += OnAssetReady;
//            Globals.EventHelper.Content.AssetsInvalidated += OnAssetsInvalidated;
//        }
    
//        private static void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
//        {
//            if (!e.NamesWithoutLocale.Contains(CustomChestsAssetName))
//                return;
    
//            CustomChestPatches.CustomChestsAsset = Game1.content.Load<Dictionary<string, CustomChestData>>(CustomChestsAssetString);
//        }
    
//        private static void OnAssetReady(object? sender, AssetReadyEventArgs e)
//        {
//            if (!e.NameWithoutLocale.IsEquivalentTo(CustomChestsAssetString))
//                return;
    
//            CustomChestPatches.CustomChestsAsset = Game1.content.Load<Dictionary<string, CustomChestData>>(CustomChestsAssetString);
//        }

//        private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
//        {
//            if (e.NameWithoutLocale.IsEquivalentTo(CustomChestsAssetString))
//                e.LoadFrom(() => new Dictionary<string, CustomChestData>(), AssetLoadPriority.Low);
//        }
//    }

//    public class CustomChestData
//    {
//        public int StartingFrame;
//        public int OpeningFrames;
//        public bool IsFridge;
//    }
//}



//#pragma warning restore IDE1006 // Naming Styles
