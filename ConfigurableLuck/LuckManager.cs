using System;
using StardewValley;

namespace ConfigurableLuck;

internal class LuckManager
{
    internal static void SetLuck(Farmer player, float luckValue)
    {
        luckValue = Math.Clamp(luckValue, -0.12f, 0.12f);
        player.team.sharedDailyLuck.Value = luckValue;
    }
}
