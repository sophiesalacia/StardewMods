using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI.Utilities;

namespace Calcifer.Features.ContentPatcherTokens;

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter

internal class DaysSinceModInstalledToken
{
    private Dictionary<string, int> CachedModInstallStrings = new();

    /****
    ** Metadata
    ****/

    public bool AllowsInput() => true;
    public bool RequiresInput() => true;
    public bool CanHaveMultipleValues(string? input) => false;

    public bool TryValidateInput(string? input, [NotNullWhen(false)] out string? error)
    {
        error = "";

        if (input == null)
            error = "Token must be provided input.";

        else if (!DaysSinceModInstalledHelper.IsModLoaded(input))
            error = $"Provided UniqueID \"{input}\" does not correspond to any currently loaded mod.";

        return error is "";
    }

    public bool HasBoundedRangeValues(string? input, out int min, out int max)
    {
        min = -1;
        max = int.MaxValue;
        return true;
    }

    /****
    ** State
    ****/

    public bool IsReady() => Constants.SaveFolderName is not null;

    public bool UpdateContext()
    {
        bool hasChanged = DaysSinceModInstalledHelper.ModInstallStrings.Equals(CachedModInstallStrings);
        CachedModInstallStrings = new(DaysSinceModInstalledHelper.ModInstallStrings);

        return hasChanged;
    }

    public IEnumerable<string> GetValues(string input)
    {
        if (CachedModInstallStrings.TryGetValue(input, out int value))
        {
            return new string[] { value.ToString() };
        }

        return new string[] { "-1" };
    }
}

internal static class DaysSinceModInstalledHelper
{
    internal static Dictionary<string, int> ModInstallStrings = new();
    internal static List<string> ModList = new();
    internal static string? CurrentSaveId;

    internal static bool IsModLoaded(string modUniqueId)
    {
        return modUniqueId is not null && Globals.ModRegistry.IsLoaded(modUniqueId);
    }

    internal static void GetModList()
    {
        ModList = Globals.ModRegistry.GetAll().Select(modInfo => modInfo.Manifest.UniqueID).ToList();
    }

    internal static void LoadOrCreateFile()
    {
        CurrentSaveId = Constants.SaveFolderName;
        ModInstallStrings = Globals.DataHelper.ReadJsonFile<Dictionary<string, int>>(PathUtilities.NormalizePath($"data/{CurrentSaveId}/data.json")) ?? new();
        InitializeMissingModInstallStrings();
    }

    internal static void InitializeMissingModInstallStrings()
    {
        foreach (string modId in ModList)
        {
            if (!ModInstallStrings.ContainsKey(modId))
                ModInstallStrings.Add(modId, 0);
        }
    }

    internal static void UpdateModInstallStrings()
    {
        foreach (string key in ModInstallStrings.Keys)
            ModInstallStrings[key]++;

        Globals.DataHelper.WriteJsonFile(PathUtilities.NormalizePath($"data/{CurrentSaveId}/data.json"), ModInstallStrings);
    }
}

[HasEventHooks]
public class DaysSinceModInstalledHooks
{
    public static void InitHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += (_, _) => DaysSinceModInstalledHelper.GetModList();
        Globals.EventHelper.GameLoop.SaveLoaded += (_, _) => DaysSinceModInstalledHelper.LoadOrCreateFile();
        Globals.EventHelper.GameLoop.DayEnding += (_, _) => DaysSinceModInstalledHelper.UpdateModInstallStrings();
    }
}

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0060 // Remove unused parameter
