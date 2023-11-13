using StardewModdingAPI;

namespace HalloweenJamCSharp;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        HarmonyPatcher.ApplyPatches();

        Globals.EventHelper.GameLoop.GameLaunched += FacilityManager.InitializeVars;
        Globals.EventHelper.GameLoop.SaveLoaded += (_, _) => FacilityManager.DoLightsSetup();
        Globals.EventHelper.Player.Warped += FacilityManager.AbortLightsIfNecessary;
    }
}
