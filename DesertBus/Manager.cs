using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace DesertBus;

[HasEventHooks]
public class Manager
{
    // hijack setRichPresence for this giggles

    internal Texture2D desertTexture;
    internal Texture2D desertStuff;

    public static void InitHooks()
    {
        Globals.EventHelper.Content.AssetRequested += Content_AssetRequested;
    }

    private static void Content_AssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("sophie.DesertBus/Desert"))
        {
            e.LoadFromModFile<Texture2D>(PathUtilities.NormalizePath("Assets/desert.png"), AssetLoadPriority.Low);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo("sophie.DesertBus/Stuff"))
        {
            e.LoadFromModFile<Texture2D>(PathUtilities.NormalizePath("Assets/stuff.png"), AssetLoadPriority.Low);
        }
    }
}
