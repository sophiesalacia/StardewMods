using System;
using StardewValley;

namespace ConfigurableLuck;

internal class LuckManager
{
    internal static readonly double MIN_LUCK_VALUE = -1.0;
    internal static readonly double MAX_LUCK_VALUE = 0.74;

    internal static void SetLuck(Farmer player, double luckValue)
    {
        luckValue = Math.Clamp(luckValue, MIN_LUCK_VALUE, MAX_LUCK_VALUE);

        if (player.hasSpecialCharm)
            player.team.sharedDailyLuck.Value = luckValue - 0.025f;
        else
            player.team.sharedDailyLuck.Value = luckValue;
    }
}
