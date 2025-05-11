namespace Calcifer.Features.ContentPatcherTokens;

[HasEventHooks]
internal class TokenHandler
{
    public static void InitHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += RegisterTokens;
    }

    private static void RegisterTokens(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        if (Globals.ContentPatcherApi is null)
        {
            Log.Error("Content Patcher API not found, unable to register Content Patcher tokens.");
            return;
        }

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "Stat", new StatToken());
        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "DaysSinceModInstalled", new DaysSinceModInstalledToken());

        MinesTokens.RegisterTokens();
        ShortDayNameToken.RegisterToken();
    }
}
