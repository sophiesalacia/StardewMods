using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace TestEnvironment;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.ConsoleCommands.Add("sophie.spouse.status", "Lists friendship status for the player with their spouse.", (_, _) =>
        {
            if (!Context.IsWorldReady || Game1.player.spouse is null or "") return;

            Monitor.Log($"Current relationship status: {Game1.player.friendshipData[Game1.player.spouse].Status}");
        });
    }
}
