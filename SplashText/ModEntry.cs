using StardewModdingAPI;

namespace SplashText
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Globals.InitializeGlobals(this);
            EventHookManager.InitializeEventHooks();
            ConsoleCommandManager.InitializeConsoleCommands();
            SplashText.Initialize();
        }
    }
}
