using HarmonyLib;
using StardewValley;

namespace Fireworks;

[HarmonyPatch]
class HarmonyPatches
{
	[HarmonyPatch(typeof(Event), nameof(Event.command_cutscene))]
	[HarmonyPrefix]
	public static bool command_cutscene_Prefix(Event __instance, string[] split)
	{
		// if custom event script is active, skip prefix and run original code
		if (__instance.currentCustomEventScript != null)
		{
			return true;
		}
		if (split[1] == "ModJam")
		{
			__instance.currentCustomEventScript = new EventScript_ModJam();
		}

		return true;
	}
}
