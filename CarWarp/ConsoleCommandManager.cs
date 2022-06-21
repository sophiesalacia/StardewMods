using StardewModdingAPI;

namespace CarWarp
{
    internal class ConsoleCommandManager
    {
        internal static void InitializeConsoleCommands()
        {
            Globals.CCHelper.Add("sophie.cw.warp", "Triggers the Car Warp dialogue", (_, _) =>
                {
                    if (Globals.SolidFoundationsApi is null || !Context.IsWorldReady)
                    {
                        Log.Error("Unable to trigger warp.");
                        return;
                    }

                    new CarWarp().Activate();
                }
            );
        }
    }
}
