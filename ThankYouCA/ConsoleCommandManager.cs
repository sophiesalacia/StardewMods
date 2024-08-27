using StardewValley;

namespace ThankYouCA;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add("thankyouca", "", (_, _) => ShowThankYouMenu());
    }

    private static void ShowThankYouMenu()
    {
        Game1.activeClickableMenu = new ThankYouMenu(false);
    }
}
