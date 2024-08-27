namespace Calcifer;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        HarmonyPatcher.ApplyPatches(Globals.UUID);
        EventHookHandler.InitHooks();
    }
}
