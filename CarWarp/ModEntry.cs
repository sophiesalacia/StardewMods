using StardewModdingAPI;

namespace CarWarp
{
	public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			Globals.InitializeGlobals(this);
			Globals.InitializeAssetHandlers();
			Globals.InitializeConfig();
			ConsoleCommandManager.InitializeConsoleCommands();
			EventHookManager.InitializeEventHooks();
		}
	}
}