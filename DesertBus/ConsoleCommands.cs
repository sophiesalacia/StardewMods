namespace DesertBus;

// ReSharper disable once UnusedMember.Global
public class ConsoleCommands
{
    [ConsoleCommand("db", "Tests bus sequence")]
    public static void TestBus(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.Warn("This console command should only be used in a loaded save.");
            return;
        }

        Game1.currentMinigame = new BusJourney();
    }

    [ConsoleCommand("dbkill", "Kills bus sequence")]
    public static void KillBus(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.Warn("This console command should only be used in a loaded save.");
            return;
        }

        Game1.currentMinigame.forceQuit();
    }
}
