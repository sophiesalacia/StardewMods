using StardewModdingAPI;

namespace ThankYouCA;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        HarmonyPatcher.ApplyPatches();
        EventHookManager.InitializeEventHooks();
        ConsoleCommandManager.InitializeConsoleCommands();
    }
}
