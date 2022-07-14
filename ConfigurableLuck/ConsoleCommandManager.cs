using System;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace ConfigurableLuck;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add(
            "sophie.cl.printluck",
            "Prints luck value for the current player.",
            (_, _) =>
            {
                if (Context.IsWorldReady)
                {
                    Log.Info($"Current player's luck value: {Game1.player.DailyLuck}");
                }
                else
                {
                    Log.Warn("This command should only be used while a save is loaded.");
                }
            }
        );

        Globals.CCHelper.Add(
            "sophie.cl.setluck",
            "Sets luck value for the current player to the specified amount (min: -0.12, max: 0.12",
            (_, args) =>
            {
                if (!Globals.Config.Enabled)
                {
                    Log.Warn(
                        "ConfigurableLuck is currently disabled via config. Set Enabled to true and then try again.");
                    return;
                }

                if (!args.Any() || !float.TryParse(args[0], out float luck))
                {
                    Log.Info("Usage: sophie.cl.setluck <value>\n\tValue should be between -0.12 and 0.12.");
                    return;
                }

                Globals.Config.LuckValue = Math.Clamp(luck, -0.12f, 0.12f);
                Globals.Config.ApplyConfigChangesToGame();
            }
        );
    }
}
