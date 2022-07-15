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
            $"Sets luck value for the current player to the specified amount (min: {LuckManager.MIN_LUCK_VALUE}, max: {LuckManager.MAX_LUCK_VALUE}",
            (_, args) =>
            {
                if (!Globals.Config.Enabled)
                {
                    Log.Warn(
                        "ConfigurableLuck is currently disabled via config. Set Enabled to true and then try again.");
                    return;
                }

                if (!args.Any() || !double.TryParse(args[0], out double luck))
                {
                    Log.Info($"Usage: sophie.cl.setluck <value>\n\tValue should a be a decimal number between {LuckManager.MIN_LUCK_VALUE} and {LuckManager.MAX_LUCK_VALUE}.");
                    return;
                }

                Globals.Config.LuckValue = Math.Clamp(luck, LuckManager.MIN_LUCK_VALUE, LuckManager.MAX_LUCK_VALUE);
                Globals.Config.ApplyConfigChangesToGame();
            }
        );
    }
}
