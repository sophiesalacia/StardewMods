namespace WildHaven;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        Shared.Initialize();
    }
}
