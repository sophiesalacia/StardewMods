using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewValley.Delegates;
using StardewValley.Minigames;
using StardewValley.Triggers;
using SObject = StardewValley.Object;
// ReSharper disable InconsistentNaming

namespace Calcifer.Features.Triggers;

[HarmonyPatch]
class StatMilestonePatches
{
    private const string MilestonesAssetString = "sophie.Calcifer/StatMilestones";
    private static Dictionary<string, StatMilestones>? _statMilestonesAsset;
    internal static Dictionary<string, StatMilestones> StatMilestonesAsset
    {
        get => _statMilestonesAsset ??= Globals.GameContent.Load<Dictionary<string, StatMilestones>>(MilestonesAssetString);
        set => _statMilestonesAsset = value;
    }

    internal static void UpdateStatsToCheck()
    {
        StatsToCheck.Clear();

        foreach (var kvp in StatMilestonesAsset)
        {
            StatsToCheck.Add(kvp.Value.Stat);
        }
    }

    internal static HashSet<string> StatsToCheck = [];

    internal static string CurrentStat;
    internal static uint Milestone;

    [HarmonyPatch(typeof(Stats), nameof(Stats.Set), typeof(string), typeof(uint))]
    [HarmonyPostfix]
    public static void Stats_Set_Postfix(Stats __instance, string key)
    {
        if (!StatsToCheck.Any())
        {
            UpdateStatsToCheck();
        }

        if (!StatsToCheck.Contains(key))
            return;

        uint value = __instance.Get(key);

        foreach (StatMilestones milestones in StatMilestonesAsset.Where(kvp => kvp.Value.Stat.Equals(key)).Select(kvp => kvp.Value))
        {
            if (!milestones.Milestones.Contains(value))
                continue;

            CurrentStat = key;
            Milestone = value;
            TriggerActionManager.Raise("sophie.Calcifer/StatMilestoneReached", [key, value]);
            CurrentStat = "";
            Milestone = 0u;
        }
    }
}

[HasEventHooks]
class StatMilestoneHooks
{
    private const string MilestonesAssetString = "sophie.Calcifer/StatMilestones";
    private static readonly IAssetName MilestonesAssetName = Globals.GameContent.ParseAssetName(MilestonesAssetString);

    internal static void InitHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += RegisterTriggers;
        Globals.EventHelper.GameLoop.GameLaunched += RegisterQuery;

        Globals.EventHelper.Content.AssetRequested += OnAssetRequested;
        Globals.EventHelper.Content.AssetReady += OnAssetReady;
        Globals.EventHelper.Content.AssetsInvalidated += OnAssetsInvalidated;
    }

    private static void RegisterQuery(object? sender, GameLaunchedEventArgs e)
    {
        GameStateQuery.Register("sophie.Calcifer/CurrentStatMilestone", CurrentStatMilestoneQuery);
    }

    private static bool CurrentStatMilestoneQuery(string[] query, GameStateQueryContext context)
    {
        if (!ArgUtility.TryGet(query, 1, out string stat, out string error, false, "stat key"))
        {
            GameStateQuery.Helpers.ErrorResult(query, $"Failed to parse stat key from provided query: {error}");
        }

        if (!ArgUtility.TryGetOptionalInt(query, 2, out int milestone, out error, -1, "milestone value"))
        {
            GameStateQuery.Helpers.ErrorResult(query, $"Failed to parse milestone value from provided query: {error}");
        }

        return StatMilestonePatches.CurrentStat.Equals(stat) && (milestone < 0 || StatMilestonePatches.Milestone.Equals((uint) milestone));
    }

    private static void RegisterTriggers(object? sender, GameLaunchedEventArgs e)
    {
        TriggerActionManager.RegisterTrigger("sophie.Calcifer/StatMilestoneReached");
    }

    private static void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (!e.NamesWithoutLocale.Contains(MilestonesAssetName))
            return;

        StatMilestonePatches.StatMilestonesAsset = Game1.content.Load<Dictionary<string, StatMilestones>>(MilestonesAssetString);
        StatMilestonePatches.UpdateStatsToCheck();
    }

    private static void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo(MilestonesAssetString))
            return;

        StatMilestonePatches.StatMilestonesAsset = Game1.content.Load<Dictionary<string, StatMilestones>>(MilestonesAssetString);
        StatMilestonePatches.UpdateStatsToCheck();
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(MilestonesAssetString))
            e.LoadFrom(() => new Dictionary<string, StatMilestones>(), AssetLoadPriority.Low);
    }
}

public class StatMilestones
{
    public string Stat;
    public List<uint> Milestones = [];
}
