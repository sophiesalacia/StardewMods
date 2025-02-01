using StardewValley.Network.NetEvents;

#pragma warning disable CA1822 // Mark members as static
// ReSharper disable UnusedMember.Global
namespace CustomAchievementsRedux;

internal class HasAchievementToken
{
    // maps from 
    internal Dictionary<long, Dictionary<string, string>> CachedAchievementData = [];

    /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
    /// <remarks>Default false.</remarks>
    public bool AllowsInput()
    {
        return true;
    }

    /// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
    /// <remarks>Default false.</remarks>
    public bool RequiresInput()
    {
        return true;
    }

    /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <param name="allowedValues">The possible values for the input.</param>
    /// <remarks>Default unrestricted.</remarks>
    public bool HasBoundedValues(string? input, out IEnumerable<string> allowedValues)
    {
        allowedValues = [bool.TrueString, bool.FalseString];
        return true;
    }

    public bool CanHaveMultipleValues(string input = null)
    {
        return false;
    }

    public bool UpdateContext()
    {
        Dictionary<long, Dictionary<string, string>> newAchievementData = [];
        bool hasChanged = false;

        foreach (Farmer farmer in Game1.getAllFarmers())
        {
            Dictionary<string, string> farmerAchievements = [];

            foreach (string key in farmer.modData.Keys.Where(key =>
                         key.StartsWith("sophie.CustomAchievementsRedux/Achievements")))
            {
                farmerAchievements[key] = farmer.modData[key];
            }

            newAchievementData.Add(farmer.UniqueMultiplayerID, farmerAchievements);
        }

        if (newAchievementData.Any(kvp => !CachedAchievementData.ContainsKey(kvp.Key)) ||
            CachedAchievementData.Any(kvp => !newAchievementData.ContainsKey(kvp.Key)))
        {
            hasChanged = true;
        }
        else
        {
            foreach (long id in newAchievementData.Keys)
            {
                Dictionary<string, string> newDict = newAchievementData[id];
                Dictionary<string, string> cachedDict = CachedAchievementData[id];

                // if dicts are identical, move to next id
                if (newDict.Keys.Count == cachedDict.Keys.Count && newDict.Keys.All(k => cachedDict.ContainsKey(k) && string.Equals(cachedDict[k], newDict[k])))
                    continue;
                

                hasChanged = true;
                break;
            }
        }

        CachedAchievementData = new Dictionary<long, Dictionary<string, string>>(newAchievementData);
        return hasChanged;
    }

    /// <summary>Get whether the token is available for use.</summary>
    public bool IsReady()
    {
        return Context.IsWorldReady || SaveGame.loaded is not null && Manager.AchievementsData is not null;
    }

    /// <summary>Get the current values.</summary>
    /// <param name="input">The input arguments, if applicable.</param>
    public IEnumerable<string> GetValues(string input)
    {
        string[] inputSplit = input.Split('|');
        string player = "Current";

        if (inputSplit.Length > 1)
        {
            string[] playerArg = inputSplit[1].Split('=');

            if (playerArg.Length > 1)
            {
                player = playerArg[1];
            }
        }

        string achievement = "sophie.CustomAchievementsRedux/Achievements/" + inputSplit[0].Trim();

        bool output = GameStateQuery.Helpers.WithPlayer(Game1.player, player, farmer => CachedAchievementData[farmer.UniqueMultiplayerID].TryGetValue(achievement, out string value) && bool.TryParse(value, out bool result) && result);

        return [output.ToString()];
    }
}

#pragma warning restore CA1822 // Mark members as static
