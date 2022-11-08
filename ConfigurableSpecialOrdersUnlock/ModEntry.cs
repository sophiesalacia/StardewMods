using StardewModdingAPI;

namespace ConfigurableSpecialOrdersUnlock;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        Globals.InitializeConfig();
        EventHookManager.InitializeEventHooks();
        ConsoleCommandManager.InitializeConsoleCommands();

        ApplyPatches();
    }

    private void ApplyPatches()
    {
        Monitor.Log(HarmonyPatches.ApplyHarmonyPatches() ? "Patches successfully applied" : "Failed to apply patches");
    }
}
