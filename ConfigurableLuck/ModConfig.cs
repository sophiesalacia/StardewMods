using StardewValley;

namespace ConfigurableLuck;

internal class ModConfig
{
    public bool Enabled = true;
    public float LuckValue = 0.0f;

    internal void ApplyConfigChangesToGame()
    {
        LuckManager.SetLuck(Game1.player, LuckValue);
        Log.Trace($"Updated luck value to {LuckValue}");
    }
}
