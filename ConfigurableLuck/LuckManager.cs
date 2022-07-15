using System;
using StardewValley;

namespace ConfigurableLuck;

internal class LuckManager
{
    internal static readonly double MIN_LUCK_VALUE = -1.0;
    internal static readonly double MAX_LUCK_VALUE = 1.0;

    internal static void SetLuck(Farmer player, double luckValue)
    {
        luckValue = Math.Clamp(luckValue, MIN_LUCK_VALUE, MAX_LUCK_VALUE);
        player.team.sharedDailyLuck.Value = luckValue;
    }
}
