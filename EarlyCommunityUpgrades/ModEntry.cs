using StardewModdingAPI;

namespace EarlyCommunityUpgrades;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        HarmonyPatcher.ApplyPatches();
        EventHookManager.InitializeEventHooks();
    }
}
