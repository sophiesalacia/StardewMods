using StardewModdingAPI;

namespace ColoredCrystalariums;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        EventHookManager.InitializeEventHooks();
        ConsoleCommandManager.InitializeConsoleCommands();
        HarmonyPatcher.ApplyPatches();
    }
}
