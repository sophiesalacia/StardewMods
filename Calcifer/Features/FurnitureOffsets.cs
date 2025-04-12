using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Calcifer.EventHooks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using SObject = StardewValley.Object;

#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable InconsistentNaming - Harmony magic
// ReSharper disable RedundantAssignment - Harmony magic
// ReSharper disable UnusedMember.Global - Harmony magic

namespace Calcifer.Features;

[HarmonyPatch]
internal class FurnitureOffsetPatches
{
    private const string CustomFurnitureOffsetAssetName = "sophie.Calcifer/FurnitureOffsets";

    private static Dictionary<string, Vector2>? _customFurnitureOffsetAsset;
    internal static Dictionary<string, Vector2> CustomFurnitureOffsetAsset
    {
        get => _customFurnitureOffsetAsset ??=
            Globals.GameContent.Load<Dictionary<string, Vector2>>(CustomFurnitureOffsetAssetName);
        set => _customFurnitureOffsetAsset = value;
    }

    internal static ConditionalWeakTable<Furniture, Vector2Holder> cachedOffsetTable = new();

    // technically has side effects bc of ref variable __state, so we make sure it runs early so it doesn't get skipped
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(Furniture), nameof(Furniture.draw))]
    [HarmonyPrefix]
    internal static void draw_Prefix(Furniture __instance, ref SObject? __state)
    {
        __state = null;

        if (!cachedOffsetTable.TryGetValue(__instance, out Vector2Holder? offsetHolder))
        {
            if (!CustomFurnitureOffsetAsset.TryGetValue(__instance.QualifiedItemId, out Vector2 offset))
            {
                cachedOffsetTable.Add(__instance, new Vector2Holder(Vector2.Zero));
                return;
            }

            cachedOffsetTable.Add(__instance, new Vector2Holder(offset));
        }
        // in CWT, no offset - none of this logic is needed
        else if (offsetHolder.Value == Vector2.Zero)
            return;

        // stash heldObject for later, remove from furniture - __instance skips the draw code for the held object
        __state = __instance.heldObject.Value;
        __instance.heldObject.Value = null;
    }

    [HarmonyPatch(typeof(Furniture), nameof(Furniture.draw))]
    [HarmonyPostfix]
    internal static void draw_PostFix(Furniture __instance, SpriteBatch spriteBatch, float alpha, ref SObject? __state)
    {
        Vector2 offset = cachedOffsetTable.GetOrCreateValue(__instance).Value;

        // if offset is non-zero and __state is non-null, mimic __instance heldObject draw logic but with added offset
        if (offset == Vector2.Zero || __state is null)
            return;

        __instance.heldObject.Value = __state;

        if (__instance.heldObject.Value is Furniture furniture)
        {
            // draw held furniture with custom offset
            furniture.drawAtNonTileSpot(
                spriteBatch: spriteBatch,
                location: Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32 + offset.X, __instance.boundingBox.Center.Y - furniture.sourceRect.Height * 4 - (__instance.drawHeldObjectLow.Value ? -16 : 16) + offset.Y)),
                layerDepth: (__instance.boundingBox.Bottom - 7) / 10000f,
                alpha: alpha);
        }
        else
        {
            ParsedItemData heldItemData = ItemRegistry.GetDataOrErrorItem(__instance.heldObject.Value.QualifiedItemId);

            // draw shadow with custom offset
            spriteBatch.Draw(
                texture: Game1.shadowTexture,
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32 + offset.X, __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85) + offset.Y)) + new Vector2(32f, 53f),
                sourceRectangle: Game1.shadowTexture.Bounds,
                color: Color.White * alpha,
                rotation: 0f,
                origin: new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: __instance.boundingBox.Bottom / 10000f);
            // draw item with custom offset
            spriteBatch.Draw(
                texture: heldItemData.GetTexture(),
                position: Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32 + offset.X, __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85) + offset.Y)),
                sourceRectangle: heldItemData.GetSourceRect(),
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: (__instance.boundingBox.Bottom + 1) / 10000f);
        }
    }

    // prefix - set item to null, store item in __state
    // postfix - restore item, draw with modified coords
    // get modified coords from CWT if it is there, moddata if it isn't (and store in cwt)

    // basically a Vector2 but it has to be a reference type to be usable by the CWT so ¯\_(ツ)_/¯
    internal class Vector2Holder
    {
        internal Vector2 Value;

        internal Vector2Holder(Vector2 value) => Value = value;
        internal Vector2Holder() : this(Vector2.Zero) { }
    }

}

[HasEventHooks]
internal static class FurnitureOffsetHooks
{
    private const string OffsetAssetString = "sophie.Calcifer/FurnitureOffsets";
    private static readonly IAssetName OffsetAssetName = Globals.GameContent.ParseAssetName(OffsetAssetString);

    internal static void InitHooks()
    {
        // content pipeline
        Globals.EventHelper.Content.AssetRequested += OnAssetRequested;
        Globals.EventHelper.Content.AssetReady += OnAssetReady;
        Globals.EventHelper.Content.AssetsInvalidated += OnAssetsInvalidated;

        // clear cwt when furniture asset is reloaded or when location changes
        Globals.EventHelper.Player.Warped += (_, _) => ClearCustomOffsetTable();
    }

    private static void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (!e.NamesWithoutLocale.Contains(OffsetAssetName))
            return;

        FurnitureOffsetPatches.CustomFurnitureOffsetAsset = Game1.content.Load<Dictionary<string, Vector2>>(OffsetAssetString);
        ClearCustomOffsetTable();
    }

    private static void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo(OffsetAssetString))
            return;

        FurnitureOffsetPatches.CustomFurnitureOffsetAsset = Game1.content.Load<Dictionary<string, Vector2>>(OffsetAssetString);
        ClearCustomOffsetTable();
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(OffsetAssetString))
            e.LoadFrom(() => new Dictionary<string, Vector2>(), AssetLoadPriority.Low);
    }

    private static void ClearCustomOffsetTable()
    {
        FurnitureOffsetPatches.cachedOffsetTable.Clear();
    }
}

#pragma warning restore IDE1006
