using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Calcifer.Features.ContentPatcherTokens;

internal class StatToken
{
    public Stats? CachedStats;
    
    public PropertyInfo[] StatProperties = typeof(Stats).GetProperties();

    /*********
    ** Public methods
    *********/

    /****
    ** Metadata
    ****/

    /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
    /// <remarks>Default false.</remarks>
    public bool AllowsInput() => true;

    /// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
    /// <remarks>Default true.</remarks>
    public bool RequiresInput() => true;

    /// <summary>Whether the token may return multiple values for the given input.</summary>
    /// <param name="input">The input arguments, if any.</param>
    /// <remarks>Default true.</remarks>
    public bool CanHaveMultipleValues(string? input = null) => false;

    /****
    ** State
    ****/

    /// <summary>Update the values when the context changes.</summary>
    /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
    public bool UpdateContext()
    {
        Stats newStats = Game1.stats ?? SaveGame.loaded.player.stats;

        if (CachedStats is null)
        {
            CachedStats = newStats;
            return true;
        }

        bool hasChanged = false;

        foreach (PropertyInfo property in StatProperties)
        {
            if (property.GetValue(CachedStats) != property.GetValue(newStats))
            {
                hasChanged = true;
                property.SetValue(CachedStats, property.GetValue(newStats));
            }
        }

        if (CachedStats.Values.Count != newStats.Values.Count || !CachedStats.Values.Keys.Any(key => !newStats.Values.ContainsKey(key) || newStats.Values[key] != CachedStats.Values[key]))
        {
            hasChanged = true;
            CachedStats.Values = new(newStats.Values);
        }

        return hasChanged;

    }

    /// <summary>Get whether the token is available for use.</summary>
    public bool IsReady()
    {
        return Game1.stats is not null || SaveGame.loaded.player.stats is not null;
    }

    /// <summary>Get the current values.</summary>
    /// <param name="input">The input arguments, if any.</param>
    public IEnumerable<string> GetValues(string? input)
    {
        if (string.IsNullOrEmpty(input) || CachedStats is null)
            return Enumerable.Empty<string>();

        PropertyInfo? inputProperty = StatProperties.FirstOrDefault(propertyInfo => propertyInfo.Name == input);
        if (inputProperty is not null)
        {
            object? propVal = inputProperty.GetValue(CachedStats);

            if (propVal is null)
                return Enumerable.Empty<string>();

            return new[] { ((uint)propVal).ToString() };
        }

        if (CachedStats.Values.TryGetValue(input, out uint val))
        {
            return new[] { val.ToString() };
        }

        return Enumerable.Empty<string>();
    }
}
