using StardewModdingAPI;
using StardewValley;

namespace ConfigurableLuck;

internal class ModConfig
{
    public bool Enabled = true;
    public double LuckValue = 0.0f;

    internal void ApplyConfigChangesToGame()
    {
        if (!Context.IsWorldReady)
            return;

        LuckManager.SetLuck(Game1.player, LuckValue);
        Log.Trace($"Updated luck value to {LuckValue}");
    }
}
