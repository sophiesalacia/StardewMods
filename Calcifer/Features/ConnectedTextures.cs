// This code is hereby released into the public domain
// attribution is appreciated but not required.
//
// Based on ConnectedTextures.cs by tlitookilakin https://gist.github.com/tlitookilakin/a1a8d6d8fd9b894578d13f9c56bf9338

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace Calcifer.Features;

/// <summary>
/// Connected Textures
/// Supported:
/// - Most placable objects and big craftables, as long as they obey ParentSheetIndex
/// - Most furniture
/// Not supported
/// - Rotations
/// - Fish Tank (fishtank)
/// - Bed (bed\w+)
/// - Free Cactuses (randomized_plant)
/// - Bigger Craftables
/// - Furniture via Furniture Framework
/// </summary>
[HasEventHooks]
class ConnectedTextures
{
    /// APPLY TEXTURES

    /// This is used to track what objects/furniture have gotten change, since the asset sophie.Calcifer/ConnectedTextures could potentially change between checks
    public const string ConnectedTextureApplied = "sophie.Calcifer/ConnectedTextures.Applied";

    private static readonly MethodInfo? Furniture_isLampStyleLightSourceMethod = typeof(Furniture).GetMethod("isLampStyleLightSource", BindingFlags.NonPublic | BindingFlags.Instance);

    private static bool Furniture_isLampStyleLightSource(Furniture furniture)
    {
        return (bool)(Furniture_isLampStyleLightSourceMethod?.Invoke(furniture, []) ?? false);
    }

    /// Use various events to modify sprite index as needed
    internal static void InitHooks()
    {
        Globals.EventHelper.Content.AssetRequested += OnAssetRequested;
        Globals.EventHelper.Content.AssetsInvalidated += OnAssetInvalidated;

        Globals.EventHelper.GameLoop.Saving += PreSaveReset;
        Globals.EventHelper.GameLoop.Saved += PostSaveSetup;
        Globals.EventHelper.GameLoop.SaveLoaded += PostSaveSetup;

        Globals.EventHelper.Player.Warped += OnWarped;
        Globals.EventHelper.World.ObjectListChanged += OnObjectListChanged;
        // World.FurnitureListChanged gives inaccorate TileLocation of the removed furniture
        // Need to use where.furniture.OnValueAdded/OnValueRemoved instead
    }

    /// <summary>Location change handling</summary>
    private static void OnWarped(object? sender, WarpedEventArgs e)
    {
        LocationTeardown(e.OldLocation);
        LocationSetup(e.NewLocation);
    }

    /// <summary>Reset all connected textures before save</summary>
    private static void PreSaveReset(object? sender, SavingEventArgs e)
    {
        LocationTeardown(Game1.currentLocation);
        // do not allow modified sprite index/source rect go into the save
        Utility.ForEachLocation(
            where =>
            {
                foreach (StardewValley.Object obj in where.Objects.Values)
                {
                    Object_ResetParentSheetIndex(obj);
                }
                foreach (Furniture furniture in where.furniture)
                {
                    Furniture_ResetSourceRect(furniture);
                }
                return true;
            }
        );
    }

    /// <summary>Setup after saving</summary>
    private static void PostSaveSetup(object? sender, EventArgs e)
    {
        LocationSetup(Game1.currentLocation, true);
    }

    /// <summary>Setup connected textures in new location (as needed) and furniture add/remove handling</summary>
    private static void LocationSetup(GameLocation? where, bool forceCheck = false)
    {
        if (where == null)
            return;
        foreach ((Vector2 tile, StardewValley.Object obj) in where.Objects.Pairs)
        {
            Object_UpdateParentSheetIndex(where, tile, obj, forceCheck: forceCheck);
        }
        foreach (Furniture furniture in where.furniture)
        {
            Furniture_UpdateSourceRect(where, furniture, forceCheck: forceCheck);
        }

        where.furniture.OnValueAdded += OnFurnitureAdded;
        where.furniture.OnValueRemoved += OnFurnitureRemoved;
    }

    /// <summary>Remove furniture add/remove handling</summary>
    private static void LocationTeardown(GameLocation? where)
    {
        if (where == null)
            return;
        where.furniture.OnValueAdded -= OnFurnitureAdded;
        where.furniture.OnValueRemoved -= OnFurnitureRemoved;
    }

    /// <summary>Furniture tile bounds</summary>
    public static Rectangle FurnitureTileBounds(Furniture furniture)
    {
        return new Rectangle(
            (int)furniture.TileLocation.X,
            (int)furniture.TileLocation.Y,
            furniture.getTilesWide(),
            furniture.getTilesHigh()
        );
    }

    /// <summary>
    // Update neighbour object/furnitures.
    // Duplicate work potentially happen, but usually furniture changes one at a time.
    // </summary>
    private static void UpdateNeighbours(GameLocation where, Rectangle bounds)
    {
        if (bounds.Height is 1)
        {
            Object_UpdateParentSheetIndex(where, new(bounds.Left - 1, bounds.Y), null, true);
            Object_UpdateParentSheetIndex(where, new(bounds.Right, bounds.Y), null, true);
        }

        if (bounds.Width is 1)
        {
            Object_UpdateParentSheetIndex(where, new(bounds.Top - 1, bounds.Y), null, true);
            Object_UpdateParentSheetIndex(where, new(bounds.Bottom, bounds.Y), null, true);
        }

        foreach (var furniture in where.furniture)
        {
            var fbounds = FurnitureTileBounds(furniture);

            // if it's touching to the right or left
            if (fbounds.Top == bounds.Bottom || fbounds.Bottom == bounds.Top)
            {
                // if it's aligned or at a corner
                if (fbounds.X == bounds.X || fbounds.Right == bounds.Left || fbounds.Left == bounds.Right)
                    Furniture_UpdateSourceRect(where, furniture, true);
            }
            // if it's touching to the top or bottom
            else if (fbounds.Top == bounds.Bottom || fbounds.Bottom == bounds.Top)
            {
                // if it's aligned or at a corner
                if (fbounds.Y == bounds.Y || fbounds.Top == bounds.Bottom || fbounds.Bottom == bounds.Top)
                    Furniture_UpdateSourceRect(where, furniture, true);
            }
        }
    }

    // APPLY TEXTURES: OBJECTS

    /// <summary>Update neighbour objects</summary>
    private static void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
            return;

        HashSet<Rectangle> needNeighbourUpdate = [];
        foreach ((Vector2 tile, StardewValley.Object obj) in e.Removed)
        {
            Object_ResetParentSheetIndex(obj);
            needNeighbourUpdate.Add(new((int)tile.X, (int)tile.Y, 1, 1));
        }
        foreach ((Vector2 tile, StardewValley.Object obj) in e.Added)
        {
            Object_UpdateParentSheetIndex(e.Location, tile, obj, forceCheck: true);
            needNeighbourUpdate.Add(new((int)tile.X, (int)tile.Y, 1, 1));
        }
        foreach (Rectangle rect in needNeighbourUpdate)
        {
            UpdateNeighbours(e.Location, rect);
        }
    }

    /// <summary>Reset the object to original index if it had been changed by this</summary>
    private static void Object_ResetParentSheetIndex(StardewValley.Object obj)
    {
        if (obj.modData.ContainsKey(ConnectedTextureApplied))
        {
            obj.ResetParentSheetIndex();
            obj.modData.Remove(ConnectedTextureApplied);
        }
    }

    /// <summary>Update the object's index based on offset, mark object as changed</summary>
    private static void Object_UpdateParentSheetIndex(GameLocation where, Vector2 tile, StardewValley.Object? obj = null, bool forceCheck = false)
    {
        if (obj is null && !where.Objects.TryGetValue(tile, out obj))
            return;

        if (!forceCheck && obj.modData.ContainsKey(ConnectedTextureApplied))
            return;

        if (!Data.TryGetValue(obj.QualifiedItemId, out ConnectedTextureData? connectedTextureData))
            return;

        int offset = CalculateOffset(where, new Rectangle((int)tile.X, (int)tile.Y, 1, 1), connectedTextureData);
        Object_ResetParentSheetIndex(obj);
        if (offset == 0)
            return;
        obj.modData[ConnectedTextureApplied] = "T";
        obj.ParentSheetIndex += offset * connectedTextureData.ObjectOffset;
    }

    // APPLY TEXTURES: FURNITURE

    /// <summary>Update placed furniture and neighbours</summary>
    private static void OnFurnitureAdded(Furniture furniture)
    {
        Furniture_UpdateSourceRect(Game1.currentLocation, furniture, true);
        UpdateNeighbours(Game1.currentLocation, FurnitureTileBounds(furniture));
    }

    /// <summary>Reset removed furniture and neighbours</summary>
    private static void OnFurnitureRemoved(Furniture furniture)
    {
        Furniture_ResetSourceRect(furniture);
        UpdateNeighbours(Game1.currentLocation, FurnitureTileBounds(furniture));
    }

    /// <summary>Reset furniture source rect to default</summary>
    private static void Furniture_ResetSourceRect(Furniture furniture)
    {
        if (furniture.modData.ContainsKey(ConnectedTextureApplied))
        {
            furniture.sourceRect.Value = Furniture.GetDefaultSourceRect(furniture.ItemId);
            furniture.defaultSourceRect.Value = furniture.sourceRect.Value;
            furniture.updateRotation();
            furniture.modData.Remove(ConnectedTextureApplied);
        }
    }

    /// <summary>Update furniture source rect</summary>
    private static void Furniture_UpdateSourceRect(GameLocation where, Furniture furniture, bool forceCheck = false)
    {
        if (!forceCheck && furniture.modData.ContainsKey(ConnectedTextureApplied))
            return;
        // TODO: implement support for these?
        if (furniture is RandomizedPlantFurniture || furniture is BedFurniture)
            return;

        if (!Data.TryGetValue(furniture.QualifiedItemId, out ConnectedTextureData? connectedTextureData))
            return;
        int offset = CalculateOffset(where, FurnitureTileBounds(furniture), connectedTextureData);
        Furniture_ResetSourceRect(furniture);
        if (offset == 0)
            return;

        Texture2D texture = ItemRegistry.GetDataOrErrorItem(furniture.QualifiedItemId).GetTexture();
        Rectangle defaultSourceRect = furniture.defaultSourceRect.Value;

        int rectWidth;
        int rectHeight;
        if (connectedTextureData.FurnitureOffset != Point.Zero)
        {
            rectWidth = connectedTextureData.FurnitureOffset.X > 0 ? connectedTextureData.FurnitureOffset.X : defaultSourceRect.Width;
            rectHeight = connectedTextureData.FurnitureOffset.Y > 0 ? connectedTextureData.FurnitureOffset.Y : defaultSourceRect.Height;
        }
        else
        {
            rectWidth = defaultSourceRect.Width;
            rectHeight = defaultSourceRect.Height;
            // HARDCODING: windows & lamps may have sourceIndexOffset.Value=1 which means twice as much width considered for
            // HARDCODING: fish tanks have a second sprite for the glass, so twice as much width
            if (furniture is FishTankFurniture || furniture.furniture_type.Value == Furniture.window || Furniture_isLampStyleLightSource(furniture))
            {
                rectWidth += defaultSourceRect.Width;
            }
        }
        if (rectWidth == 0 || rectHeight == 0)
        {
            Log.Trace($"Error: zero width/height for furniture bounds on {furniture.QualifiedItemId}");
            return;
        }

        int newX = defaultSourceRect.X;
        int newY = defaultSourceRect.Y;
        for (int i = offset; i > 0; i--)
        {
            newX += rectWidth;
            if (newX >= texture.Width)
            {
                newX = 0;
                newY += rectHeight;
            }
        }

        furniture.defaultSourceRect.Value = new(newX, newY, defaultSourceRect.Width, defaultSourceRect.Height);
        furniture.updateRotation();
        furniture.modData[ConnectedTextureApplied] = "T";
    }

    /// DATA & CONNECTIONS
    public const string ConnectedTextureDataString = "sophie.Calcifer/ConnectedTextures";

    public static Dictionary<string, ConnectedTextureData> Data
        => _data ??= Globals.GameContent.Load<Dictionary<string, ConnectedTextureData>>(ConnectedTextureDataString);
    private static Dictionary<string, ConnectedTextureData>? _data;

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(ConnectedTextureDataString))
        {
            e.LoadFrom(() => new Dictionary<string, ConnectedTextureData>(), AssetLoadPriority.Exclusive);
        }
    }

    private static void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Any(assetName => assetName.IsEquivalentTo(ConnectedTextureDataString)))
        {
            _data = null;
        }
    }

    /// <summary>Determines the connection style to use.</summary>
    public enum ConnectionStyle
    {
        /// <summary>No connected textures. 1 sprite</summary>
        None,
        /// <summary>Connects to the top and bottom. 4 sprites</summary>
        Vertical,
        /// <summary>Connects to the left and right. 4 sprites</summary>
        Horizontal,
        /// <summary>Connects to the sides. 16 sprites</summary>
        Simple,
        /// <summary>Connects to the sides and corners. 47 sprites</summary>
        Full
    }

    /// <summary>Generates a tile index offset based on nearby tiles</summary>
    /// <param name="where">The location the object is in</param>
    /// <param name="tile">The tile position</param>
    /// <param name="connections">The types of connections it should use</param>
    /// <param name="style">Connection pattern</param>
    /// <returns>The sprite index offset to use</returns>
    public static int CalculateOffset(GameLocation where, Rectangle bounds, ConnectedTextureData connectedTextureData)
    {
        if (where is null || connectedTextureData.ConnectWith is not IList<string> connections)
            return 0;

        return connectedTextureData.Style switch
        {
            ConnectionStyle.None => 0,
            ConnectionStyle.Vertical => VerticalOffset(where, bounds, connections),
            ConnectionStyle.Horizontal => HorizontalOffset(where, bounds, connections),
            ConnectionStyle.Simple => SimpleOffset(where, bounds, connections),
            ConnectionStyle.Full => FullOffset(where, bounds, connections),
            _ => 0
        };
    }

    private static bool ConnectsToSide(GameLocation where, Vector2 direction, Rectangle bounds, IList<string> connections)
    {
        if (
            // is single tile on checked axis
            (bounds.Width is 1 && (bounds.Height is 1 || direction.Y is 0)) ||
            (bounds.Height is 1 && (bounds.Width is 1 || direction.X is 0))
        )
        {
            if (where.Objects.TryGetValue(new(bounds.X + direction.X * bounds.Width, bounds.Y + direction.Y * bounds.Height), out var obj))
                return Connects(obj.QualifiedItemId, connections);
        }

        return where.furniture.Any(f => Furniture_ConnectedAndAligned(f, bounds, direction, connections));
    }

    private static bool Furniture_ConnectedAndAligned(Furniture f, Rectangle bounds, Vector2 direction, IList<string> connections)
    {
        var fbound = FurnitureTileBounds(f);

        if (direction.X != 0)
        {
            if (fbound.Height != bounds.Height)
                return false;

            if (direction.X is < 0f)
                return fbound.Right == bounds.Left && Connects(f.QualifiedItemId, connections);

            return fbound.Left == bounds.Right && Connects(f.QualifiedItemId, connections);
        }

        if (direction.Y != 0)
        {
            if (fbound.Width != bounds.Width)
                return false;

            if (direction.Y is < 0f)
                return fbound.Bottom == bounds.Top && Connects(f.QualifiedItemId, connections);

            return fbound.Top == bounds.Bottom && Connects(f.QualifiedItemId, connections);
        }

        return false;
    }

    private static bool Connects(string id, IList<string> connections)
    {
        if (connections.Contains(id))
            return true;

        if (!Data.TryGetValue(id, out var data) || data.ConnectWith is null)
            return false;

        foreach (string type in data.ConnectWith)
            if (connections.Contains(type))
                return true;

        return false;
    }

    private static readonly int[] RequiresCornerCheck = [
        // corners
        0b0011, 0b0110, 0b1100, 0b1001,
        // 3-ways
        0b0111, 0b1110, 0b1101, 0b1011,
        // 4-way
        0b1111
    ];

    // 2-3
    // ---
    // 1-0
    private static readonly int[][] CornersToCheck = [
        // corners
        [0],[1],[2],[3],
        // 3-ways
        [0, 1], [1, 2], [2, 3], [3, 0],
        // 4-way
        [0, 1, 2, 3],
    ];

    private static readonly Vector2[] CornerCoords = [new(1, 1), new(-1, 1), new(-1, -1), new(1, -1)];

    private static int FullOffset(GameLocation where, Rectangle bounds, IList<string> connections)
    {
        // get an initial simple offset. if it's not one that supports inside corners, return as-is
        int offset = SimpleOffset(where, bounds, connections);
        int checkCorner = Array.IndexOf(RequiresCornerCheck, offset);
        if (checkCorner is -1)
            return offset;

        // generate a bitmask based on the relevant corners.
        // this is 1-bit for the corners (first 4), 2-bit for the 3-ways (second 4), and 4-bit for the 4-way (last)
        int corners = 0;
        int[] checks = CornersToCheck[checkCorner];
        for (int i = 0; i < checks.Length; i++)
            corners |= ConnectsToSide(where, CornerCoords[checks[i]], bounds, connections) ? (1 << i) : 0;

        // if no corners exist, use the default index
        if (corners is 0)
            return offset;

        int block = 0;
        int unitSize = 0;

        // simple offset size is 4 bits, with a max value of 15. 16 is 0th index after
        // unitsize is defined by bit count (see above)
        // block is defined by the offsets of previous groups, plus the number of units multiplied by the unit size
        switch (checkCorner / 4)
        {
            case 0: // corners
                block = 16;
                unitSize = 1;
                break;
            case 1: // 3-ways
                block = 16 + (4 * 1);
                unitSize = 3;
                break;
            case 2: // 4-way
                block = 16 + (4 * 1) + (4 * 3);
                unitSize = 15;
                break;
        }

        // unit index + block index + variant index
        return (checkCorner % 4) * unitSize + block + (corners - 1);
    }

    /// <summary>Generates a 4-bit bitmask from the 4 cardinal tiles (sides)</summary>
    private static int SimpleOffset(GameLocation where, Rectangle bounds, IList<string> connections)
    {
        int offset = 0;
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(1, 0), 1 << 0);
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(0, 1), 1 << 1);
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(-1, 0), 1 << 2);
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(0, -1), 1 << 3);
        return offset;
    }

    /// <summary>Generates a 2-bit bitmask from horizontal (right/left) offset</summary>
    private static int HorizontalOffset(GameLocation where, Rectangle bounds, IList<string> connections)
    {
        int offset = 0;
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(1, 0), 1);
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(-1, 0), 2);
        return offset;
    }

    /// <summary>Generates a 2-bit bitmask from vertical (down/up) offset</summary>
    private static int VerticalOffset(GameLocation where, Rectangle bounds, IList<string> connections)
    {
        int offset = 0;
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(0, 1), 1);
        offset |= NeighbourOffsetCheck(where, bounds, connections, new(0, -1), 2);
        return offset;
    }

    /// <summary>Get right side neighbours</summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    private static int NeighbourOffsetCheck(GameLocation where, Rectangle bounds, IList<string> connections, Vector2 direction, int directionalValue)
    {
        return ConnectsToSide(where, direction, bounds, connections) ? directionalValue : 0;
    }

    public record class ConnectedTextureData
    {
        /// <summary>Connection style</summary>
        public ConnectionStyle Style { get; set; }
        /// <summary>What objects this object connects with</summary>
        public List<string>? ConnectWith { get; set; }
        /// <summary>Usually the sprite offset for a big craftable is 1 index, can change it here</summary>
        public int ObjectOffset { get; set; } = 1;
        /// <summary>Usually the width offset for furniture is based on it's default source rect, can change it here</summary>
        public Point FurnitureOffset { get; set; } = Point.Zero;
    }
}



[HarmonyPatch]
class ConnectedTextures_DrawFix
{
    private static int? GetParentSheetIndex(int? previous, IndoorPot pot)
    {
        return previous ?? pot.ParentSheetIndex;
    }

    /// <summary>
    /// Harmony patch to make IndoorPot.draw respect parent sheet index so that connected texture can work.
    /// Could perhaps use similar transpilers on other misbehaving draws
    /// </summary>
    /// <param name="instructions"></param>
    /// <param name="generator"></param>
    /// <returns></returns>
    [HarmonyPatch(nameof(IndoorPot), nameof(IndoorPot.draw))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> IndoorPot_draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        try
        {
            // IL_00d1: ldc.i4.0
            // IL_00d2: cgt.un
            // IL_00d4: ldloca.s 4
            // IL_00d6: initobj valuetype [System.Runtime]System.Nullable`1<int32>
            // IL_00dc: ldloc.s 4
            // IL_00de: callvirt instance valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle StardewValley.ItemTypeDefinitions.ParsedItemData::GetSourceRect(int32, valuetype [System.Runtime]System.Nullable`1<int32>)

            // dataOrErrorItem.GetSourceRect(showNextIndex.Value ? 1 : 0, null)
            CodeMatcher matcher = new(instructions, generator);

            matcher
            .MatchEndForward([
                new(OpCodes.Cgt_Un),
                new(inst => inst.IsLdloc()),
                new(OpCodes.Initobj, typeof(int?)),
                new(inst => inst.IsLdloc()),
                new(OpCodes.Callvirt, AccessTools.DeclaredMethod(typeof(ParsedItemData), nameof(ParsedItemData.GetSourceRect)))
            ])
            .ThrowIfNotMatch("Failed to find '-.GetSourceRect(- ? - : 0, null)'")
            .InsertAndAdvance([
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(ConnectedTextures_DrawFix), nameof(GetParentSheetIndex)))
            ]);

            return matcher.Instructions();
        }
        catch (Exception err)
        {
            Log.Error($"Error in ConnectedTextures_DrawFix::IndoorPot_draw_Transpiler:\n{err}");
            return instructions;
        }
    }

    private static Rectangle GetDefaultSourceRect(Rectangle rectangle, FishTankFurniture furniture)
    {
        return furniture.defaultSourceRect.Value;
    }

    /// <summary>Fix the fish tank glass draw layer</summary>
    /// <param name="instructions"></param>
    /// <param name="generator"></param>
    /// <returns></returns>
    [HarmonyPatch(nameof(FishTankFurniture), nameof(FishTankFurniture.draw))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> FishTankFurniture_draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        try
        {
            // IL_0092: ldc.i4.0
            // IL_0093: ldloca.s 4
            // IL_0095: initobj valuetype [System.Runtime]System.Nullable`1<int32>
            // IL_009b: ldloc.s 4
            // IL_009d: callvirt instance valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Rectangle StardewValley.ItemTypeDefinitions.ParsedItemData::GetSourceRect(int32, valuetype [System.Runtime]System.Nullable`1<int32>)

            // dataOrErrorItem.GetSourceRect(showNextIndex.Value ? 1 : 0, null)
            CodeMatcher matcher = new(instructions, generator);

            matcher
            .MatchEndForward([
                new(OpCodes.Ldc_I4_0),
                new(inst => inst.IsLdloc()),
                new(OpCodes.Initobj, typeof(int?)),
                new(inst => inst.IsLdloc()),
                new(OpCodes.Callvirt, AccessTools.DeclaredMethod(typeof(ParsedItemData), nameof(ParsedItemData.GetSourceRect)))
            ])
            .ThrowIfNotMatch("Failed to find '-.GetSourceRect(0, null)'")
            .Advance(1)
            .InsertAndAdvance(
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(ConnectedTextures_DrawFix), nameof(GetDefaultSourceRect)))
            );

            return matcher.Instructions();
        }
        catch (Exception err)
        {
            Log.Error($"Error in ConnectedTextures_DrawFix::IndoorPot_draw_Transpiler:\n{err}");
            return instructions;
        }
    }

}
