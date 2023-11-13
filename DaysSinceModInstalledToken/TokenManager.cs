namespace DaysSinceModInstalledToken;

internal class TokenManager
{
    internal static void RegisterToken()
    {
        Globals.ContentPatcherApi?.RegisterToken(Globals.Manifest!, "DaysSinceModInstalled", new DaysSinceModInstalledToken());
    }

}
