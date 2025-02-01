namespace Calcifer.Features.ContentPatcherTokens;

internal class ShortDayNameToken
{
    internal static void RegisterToken()
    {
        if (Globals.ContentPatcherApi is null)
            return;

        Globals.ContentPatcherApi.RegisterToken(
            Globals.Manifest,
            "ShortDayName",
            () => Context.IsWorldReady ? [Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)] : []
        );
    }
}
