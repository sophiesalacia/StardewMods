using StardewModdingAPI;

namespace InventoryRandomizer;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        //todo: add weighted probabilities to Config

        Globals.InitializeGlobals(this);
        Globals.InitializeConfig();
        EventHookManager.InitializeEventHooks();
        ConsoleCommandManager.InitializeConsoleCommands();
    }
}
