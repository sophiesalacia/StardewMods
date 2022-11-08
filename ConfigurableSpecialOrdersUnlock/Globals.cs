using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ConfigurableSpecialOrdersUnlock;

internal class Globals
{
    public static IGenericModConfigMenuApi GmcmApi;
    public static ModConfig Config { get; set; }
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
    public static string UUID => Manifest.UniqueID;

    internal static void InitializeConfig()
    {
        Config = Helper.ReadConfig<ModConfig>();
    }

    internal static void InitializeGlobals(ModEntry modEntry)
    {
        Manifest = modEntry.ModManifest;
        Helper = modEntry.Helper;
        Log.Monitor = modEntry.Monitor;
    }

    internal static bool InitializeGmcmApi()
    {
        GmcmApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        return GmcmApi is not null;
    }
}
