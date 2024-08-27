using System.Linq;
using System.Reflection;
using StardewValley.Locations;

namespace Calcifer.Features.ContentPatcherTokens;

internal class MinesTokens
{
    private static PropertyInfo? IsQuarryArea;

    internal static void RegisterTokens()
    {
        if (Globals.ContentPatcherApi is null)
            return;

        IsQuarryArea = typeof(MineShaft).GetProperty("isQuarryArea", BindingFlags.NonPublic | BindingFlags.Instance);

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "InMines", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    return ms.mineLevel < 121 ? new[] { true.ToString() } : new[] { false.ToString() };
                }

                // save is loaded but we are not in a mineshaft
                if (Context.IsWorldReady)
                {
                    return new[] { false.ToString() };
                }

                // no save loaded (e.g. on the title screen)
                return Enumerable.Empty<string>();
            });


        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "InQuarry", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    bool quarryArea = (bool?)IsQuarryArea?.GetValue(ms) ?? false;
                    bool quarryLevel = ms.mineLevel == 77377;

                    return (quarryArea || quarryLevel) ? new[] { true.ToString() } : new[] { false.ToString() };
                }

                // save is loaded but we are not in a mineshaft
                if (Context.IsWorldReady)
                {
                    return new[] { false.ToString() };
                }

                // no save loaded (e.g. on the title screen)
                return Enumerable.Empty<string>();
            });

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "InSkullCavern", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    return ms.mineLevel >= 121 ? new[] { true.ToString() } : new[] { false.ToString() };
                }

                // save is loaded but we are not in a mineshaft
                if (Context.IsWorldReady)
                {
                    return new[] { false.ToString() };
                }

                // no save loaded (e.g. on the title screen)
                return Enumerable.Empty<string>();
            });

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "CurrentMineLevel", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    return new[] { ms.mineLevel.ToString() };
                }

                // no save loaded (e.g. on the title screen) or we are not in a mineshaft
                return Enumerable.Empty<string>();
            });

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "IsHardModeActive", () =>
            {
                // save is loaded
                if (Context.IsWorldReady)
                {
                    return new[] { (Game1.netWorldState.Value.MinesDifficulty > 0).ToString() };
                }

                // no save loaded (e.g. on the title screen)
                return Enumerable.Empty<string>();
            });
    }
}
