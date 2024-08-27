using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace CarWarp;

internal class EventHookManager
{
    /// <summary>
    /// Sets up initial event handler.
    /// </summary>
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += LoadAssets;
        Globals.EventHelper.GameLoop.GameLaunched += HookIntoApis;
    }

    private static void LoadAssets(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.WarpLocationsContentPath))
        {
            e.LoadFrom(
                () => new Dictionary<string, WarpLocationModel>(),
                AssetLoadPriority.Medium
            );
        }
    }


    /// <summary>
    /// Tries to get and utilize all APIs.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>")]
    private static void HookIntoApis(object? sender, GameLaunchedEventArgs e)
    {
        if (!Globals.InitializeCPApi() || Globals.ContentPatcherApi is null)
        {
            Log.Warn("Failed to fetch ContentPatcher API.");
            return;
        }

        if (Globals.InitializeGMCMApi())
        {
            GenericModConfigMenuHelper.BuildConfigMenu();
        }
        else
        {
            Log.Info("Failed to fetch GMCM API, skipping config menu setup.");
        }

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "Configuration", () =>
            {
                return new[] {
                    Globals.Config.Configuration.ToLower() switch
                    {
                        "right" =>  "overlay-steering-wheel-right",
                        "left" =>   "overlay-steering-wheel-left",
                        "none" =>   "overlay-no-steering-wheel",
                        "empty" =>  "overlay-no-dashboard",
                        _ =>        "overlay-steering-wheel-right"
                    }
                };
            }
        );

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "SeasonalOverlay", () =>
            {
                return new[] { Globals.Config.SeasonalOverlay.ToString() };
            }
        );
        
        GameLocation.RegisterTileAction("sophie.CarWarp/Activate", HandleWarp);
    }

    private static bool HandleWarp(GameLocation location, string[] args, Farmer player, Point tile)
    {
        Building car = location.getBuildingAt(tile.ToVector2());

        if (car is null or not {buildingType.Value: "skellady.CW_Car"})
        {
            Log.Warn($"Tile action \"sophie.CarWarp/Active\" triggered from tile position {tile}, but no appropriate building found at that location.");
            return false;
        }

        new CarWarp(car).Activate();

        return true;
    }
}
