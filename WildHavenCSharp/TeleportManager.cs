using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Extensions;
using xTile.Tiles;

namespace WildHaven;

[HasEventHooks]
internal class TeleportManager
{
    internal const string TeleportNetworkAssetPath = "sophie.WHF/TeleportNetworks";
    internal static IAssetName TeleportNetworkAssetName = Globals.GameContent.ParseAssetName(TeleportNetworkAssetPath);

    internal static Dictionary<string, TeleportNetworkDataModel> TeleportNetworkData = [];

    public static void InitHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += RegisterTileAction;

        // asset pipeline
        Globals.EventHelper.Content.AssetRequested += TeleportNetwork_AssetRequested;
        Globals.EventHelper.Content.AssetsInvalidated += TeleportNetwork_AssetsInvalidated;
    }

    private static void TeleportNetwork_AssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Contains(TeleportNetworkAssetName))
            TeleportNetworkData = Globals.GameContent.Load<Dictionary<string, TeleportNetworkDataModel>>(TeleportNetworkAssetPath);
    }

    private static void TeleportNetwork_AssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(TeleportNetworkAssetPath))
            e.LoadFrom(() => new Dictionary<string, TeleportNetworkDataModel>(), AssetLoadPriority.Low);
    }

    private static void RegisterTileAction(object? sender, GameLaunchedEventArgs e)
    {
        GameLocation.RegisterTileAction("sophie.WHF_TeleportNetwork", HandleTeleportNetworkAction);
    }

    private static bool HandleTeleportNetworkAction(GameLocation location, string[] args, Farmer who, Microsoft.Xna.Framework.Point tileLocation)
    {
        TeleportNetworkData = Globals.GameContent.Load<Dictionary<string, TeleportNetworkDataModel>>(TeleportNetworkAssetName);

        if (!ArgUtility.TryGet(args, 1, out string networkId, out string error, allowBlank: false, "Teleport Network ID"))
        {
            Log.Error($"Unable to parse network ID from given tile action string \"{string.Join(' ', args)}\" at tile {tileLocation} in location {location.Name}: {error}");
            return false;
        }

        if (!ArgUtility.TryGet(args, 2, out string locationId, out error, allowBlank: false, "Teleport Network ID"))
        {
            Log.Error($"Unable to parse network location ID from given tile action string \"{string.Join(' ', args)}\" at tile {tileLocation} in location {location.Name}: {error}");
            return false;
        }

        if (!TeleportNetworkData.TryGetValue(networkId, out TeleportNetworkDataModel? network) || network is null)
        {
            Log.Error($"Unable to match network ID from given tile action string \"{string.Join(' ', args)}\" at tile {tileLocation} in location {location.Name} to existing network defined in \"{TeleportNetworkAssetPath}\".");
            return false;
        }

        if (!network.Locations.TryGetValue(locationId, out TeleportLocation? loc) || loc is null)
        {
            Log.Error($"Unable to match network location ID from given tile action string \"{string.Join(' ', args)}\" at tile {tileLocation} in location {location.Name} to entry in network \"{network.Id}\".");
            return false;
        }

        GameStateQueryContext gsqContext = new(location, who, who.ActiveItem, null, null);

        List<Response> destinations = [];

        Dictionary<string, TeleportLocation> sortedLocations = network.Locations.OrderBy(loc => loc.Value.Precedence).ToDictionary(loc => loc.Key, loc => loc.Value);

        foreach (TeleportLocation teleportLocation in sortedLocations.Values)
        {
            if (teleportLocation.Id.Equals(locationId))
                continue;

            if (GameStateQuery.CheckConditions(teleportLocation.Condition, gsqContext) && !string.IsNullOrEmpty(teleportLocation.Destination))
            {
                string[] destinationArgs = ArgUtility.SplitBySpace(teleportLocation.Destination);

                if (!ArgUtility.TryGet(destinationArgs, 0, out string destId, out error, allowBlank: false, name: "Destination Location ID") || Game1.getLocationFromName(destId) is null)
                {
                    Log.Error($"Unable to parse destination for location ID \"{teleportLocation.Id}\": no location found with ID \"{destinationArgs[0]}\". Error: {error}");
                    return false;
                }

                if (!ArgUtility.TryGetInt(destinationArgs, 1, out int destX, out error, name: "Destination Location X Coord") || !ArgUtility.TryGetInt(destinationArgs, 2, out int destY, out error, name: "Destination Location Y Coord"))
                {
                    Log.Error($"Unable to parse destination for location ID \"{teleportLocation.Id}\": failed to parse provided coordinates \"{destinationArgs[1]} {destinationArgs[2]}\". Error: {error}");
                    return false;
                }

                destinations.Add(new Response($"{network.Id}¦{teleportLocation.Id}", teleportLocation.DisplayName));
            }
        }

        destinations.Add(new Response("Cancel", Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.10993")).SetHotKey(Keys.Escape));

        location.createQuestionDialogue(" ", [.. destinations], "sophie.WHF_TeleportNetwork");

        return true;
    }

    internal static void WarpFarmer(string networkId, string locationId)
    {
        string[] destinationArgs = ArgUtility.SplitBySpace(TeleportNetworkData[networkId].Locations[locationId].Destination);

        if (!ArgUtility.TryGet(destinationArgs, 0, out string destId, out string error, allowBlank: false, name: "Destination Location ID") || Game1.getLocationFromName(destId) is null)
        {
            Log.Error($"Unable to parse destination for location ID \"{locationId}\": no location found with ID \"{destinationArgs[0]}\". Error: {error}");
            return;
        }

        if (!ArgUtility.TryGetInt(destinationArgs, 1, out int destX, out error, name: "Destination Location X Coord") || !ArgUtility.TryGetInt(destinationArgs, 2, out int destY, out error, name: "Destination Location Y Coord"))
        {
            Log.Error($"Unable to parse destination for location ID \"{locationId}\": failed to parse provided coordinates \"{destinationArgs[1]} {destinationArgs[2]}\". Error: {error}");
            return;
        }

        MagicWarpFarmer(destId, destX, destY);
    }

    // pretty much ripped straight from game code and tweaked a bit
    internal static void MagicWarpFarmer(string destId, int destX, int destY)
    {
        GameLocation loc = Game1.currentLocation;

        for (int j = 0; j < 12; j++)
        {
            Game1.Multiplayer.broadcastSprites(loc, new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)Game1.player.position.X - 256, (int)Game1.player.position.X + 192), Game1.random.Next((int)Game1.player.position.Y - 256, (int)Game1.player.position.Y + 192)), flicker: false, Game1.random.NextBool()));
        }

        loc.playSound("wand");
        Game1.freezeControls = true;
        Game1.displayFarmer = false;
        Game1.player.CanMove = false;
        Game1.flashAlpha = 1f;

        DelayedAction.fadeAfterDelay(delegate
        {
            Game1.warpFarmer(destId, destX, destY, 2);
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.displayFarmer = true;
            Game1.player.CanMove = true;
            Game1.freezeControls = false;
        }, 1000);

        Rectangle playerBounds = Game1.player.GetBoundingBox();
        Point playerTile = Game1.player.TilePoint;
        
        int j2 = 0;
        for (int x = playerTile.X + 8; x >= playerTile.X - 8; x--, j2 += 25)
        {
            Game1.Multiplayer.broadcastSprites(loc, new TemporaryAnimatedSprite(6, new Vector2(x, playerTile.Y) * 64f, Color.White, 8, flipped: false, 50f)
            {
                layerDepth = 1f,
                delayBeforeAnimationStart = j2,
                motion = new Vector2(-0.25f, 0f)
            });
        }
    }
}
