using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calcifer.EventHooks;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Delegates;
using StardewValley.Triggers;

namespace Calcifer.Features.Triggers;

[HasEventHooks]
internal class TriggerZoneManager
{
    private const string TriggerZonesAssetString = "sophie.Calcifer/TriggerZones";
    private static readonly IAssetName TriggerZonesAssetName = Globals.GameContent.ParseAssetName(TriggerZonesAssetString);

    private static Dictionary<string, StatMilestones>? _triggerZonesAsset;
    internal static Dictionary<string, StatMilestones> TriggerZonesAsset
    {
        get => _triggerZonesAsset ??= Globals.GameContent.Load<Dictionary<string, StatMilestones>>(TriggerZonesAssetString);
        set => _triggerZonesAsset = value;
    }

    public static string CurrentTriggerZoneId = "";

    internal static void InitHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += RegisterTrigger;
        Globals.EventHelper.GameLoop.GameLaunched += RegisterQuery;

        Globals.EventHelper.Content.AssetRequested += OnAssetRequested;
        Globals.EventHelper.Content.AssetReady += OnAssetReady;
        Globals.EventHelper.Content.AssetsInvalidated += OnAssetsInvalidated;


    }

    private static void RegisterTrigger(object? sender, GameLaunchedEventArgs e)
    {
        TriggerActionManager.RegisterTrigger("sophie.Calcifer/TriggerZoneEntered");
    }

    private static void RegisterQuery(object? sender, GameLaunchedEventArgs e)
    {
        GameStateQuery.Register("sophie.Calcifer/CurrentTriggerZone", CurrentTriggerZoneQuery);
    }

    private static bool CurrentTriggerZoneQuery(string[] query, GameStateQueryContext context)
    {
        if (!ArgUtility.TryGet(query, 1, out string zone, out string error, false, "stat key"))
        {
            GameStateQuery.Helpers.ErrorResult(query, $"Failed to parse trigger zone ID from provided query: {error}");
        }

        return CurrentTriggerZoneId.Equals(zone);
    }

    private static void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (!e.NamesWithoutLocale.Contains(TriggerZonesAssetName))
            return;

        TriggerZonesAsset = Game1.content.Load<Dictionary<string, StatMilestones>>(TriggerZonesAssetString);
    }

    private static void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo(TriggerZonesAssetString))
            return;

        TriggerZonesAsset = Game1.content.Load<Dictionary<string, StatMilestones>>(TriggerZonesAssetString);
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(TriggerZonesAssetString))
            e.LoadFrom(() => new Dictionary<string, TriggerZoneDataModel>(), AssetLoadPriority.Low);
    }

}

public class TriggerZoneDataModel
{
    public string Id;
    public string Location;
    public Rectangle Bounds;
}
