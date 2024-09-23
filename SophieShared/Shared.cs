namespace SophieShared;

internal class Shared
{
    public static void Initialize()
    {
        HarmonyPatcher.ApplyPatches(Globals.UUID);
        EventHookHandler.InitHooks();
        ConsoleCommandHandler.InitConsoleCommands();
    }
}
