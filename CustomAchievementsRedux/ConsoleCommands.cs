namespace CustomAchievementsRedux;

// ReSharper disable once UnusedMember.Global
public class ConsoleCommands
{
    [ConsoleCommand("sophie.car.set", "Sets whether or not the player has the specified achievement, regardless of whether conditions are met. Argument 0 is the achievement ID and argument 1 is whether the player should have it or not (\"true\"/\"false\", default \"true\")")]
    public static void SetAchievement(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.Warn("This console command should only be used in a loaded save.");
            return;
        }

        if (args.Length < 1)
        {
            Log.Warn("You must provide at least one parameter (the achievement ID to set).");
            return;
        }

        bool obtained = true;
        if (args.Length > 1 && bool.TryParse(args[1], out bool obtainedArg))
        {
            obtained = obtainedArg;
        }

        Manager.SetAchievement(Game1.player, args[0], obtained);
    }

    [ConsoleCommand("sophie.car.clear", "Removes all achievements from the pack matching the provided ID.")]
    public static void ClearAchievements(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.Warn("This console command should only be used in a loaded save.");
            return;
        }

        if (args.Length < 1)
        {
            Log.Warn("You must provide at least one parameter (the pack ID to clear).");
            return;
        }

        string packId = args[0];

        if (string.IsNullOrEmpty(packId))
        {
            Log.Warn("You must provide a valid parameter (the pack ID to clear).");
            return;
        }

        if (!Manager.AchievementsData.ContainsKey(packId))
        {
            Log.Warn("Unable to locate achievement pack with matching ID.");
            return;
        }

        string dataKey = "sophie.CustomAchievementsRedux/Achievements/" + packId;

        Game1.player.modData.RemoveWhere(kvp => kvp.Key.StartsWith(dataKey));
    }

}
