using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace WildHaven;

[HarmonyPatch]
internal class Patches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogueAction))]
    [HarmonyPrefix]
    public static bool answerDialogueAction_Prefix(string questionAndAnswer)
    {
        if (!questionAndAnswer.StartsWith("sophie.WHF_TeleportNetwork"))
            return true;

        if (questionAndAnswer.Equals("sophie.WHF_TeleportNetwork_Cancel"))
            return false;

        string[] networkParams = questionAndAnswer["sophie.WHF_TeleportNetwork_".Length..].Split('¦');

        TeleportManager.WarpFarmer(networkParams[0], networkParams[1]);

        return false;
    }
}
