using StardewModdingAPI;

namespace InventoryRandomizer;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        Globals.InitializeConfig();
        EventHookManager.InitializeEventHooks();
        ConsoleCommandManager.InitializeConsoleCommands();
        ChatManager.CheckIfChatCommandsPresent();
    }
}
