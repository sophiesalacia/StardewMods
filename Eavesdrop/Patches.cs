using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Eavesdrop;

[HarmonyPatch]
internal class Patches
{
    [HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
    [HarmonyPrefix]
    public static bool NPC_checkAction_Prefix()
    {

    }
}
