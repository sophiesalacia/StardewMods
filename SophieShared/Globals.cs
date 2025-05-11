using StardewModdingAPI.Events;

namespace SophieShared;

[HasEventHooks(HookPriority.High)]
internal static partial class Globals
{
    public static IManifest Manifest { get; set; }
    public static IModHelper Helper { get; set; }
    public static ICommandHelper CCHelper => Helper.ConsoleCommands;
    public static IGameContentHelper GameContent => Helper.GameContent;
    public static IModContentHelper ModContent => Helper.ModContent;
    public static IContentPackHelper ContentPackHelper => Helper.ContentPacks;
    public static IDataHelper DataHelper => Helper.Data;
    public static IInputHelper InputHelper => Helper.Input;
    public static IModEvents EventHelper => Helper.Events;
    public static IMultiplayerHelper MultiplayerHelper => Helper.Multiplayer;
    public static IReflectionHelper ReflectionHelper => Helper.Reflection;
    public static ITranslationHelper TranslationHelper => Helper.Translation;
    public static IModRegistry ModRegistry => Helper.ModRegistry;
    public static string UUID => Manifest.UniqueID;

    public static IContentPatcherApi? ContentPatcherApi;

    internal static void InitializeGlobals(Mod modEntry)
    {
        Manifest = modEntry.ModManifest;
        Helper = modEntry.Helper;
        Log.Monitor = modEntry.Monitor;
    }

    internal static void InitHooks()
    {
        EventHelper.GameLoop.GameLaunched += (_, _) =>
            {
                ContentPatcherApi = Helper.ModRegistry.GetApi<IContentPatcherApi>("Pathoschild.ContentPatcher");
            };
    }
}
