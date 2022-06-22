namespace SplashText
{
    internal class ConsoleCommandManager
    {
        internal static void InitializeConsoleCommands()
        {
            Globals.CCHelper.Add("sophie.st.cycletext", "Displays a new splash text while on the menu screen.",
                (_, _) => { SplashText.GetRandomSplashText(); }
            );
        }
    }
}
