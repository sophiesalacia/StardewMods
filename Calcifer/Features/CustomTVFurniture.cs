using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace Calcifer.Features;

[HasEventHooks]
[HarmonyPatch]
class CustomTVFurniture
{
    private record TVScreenShape(float PosX, float PosY, float Scale);
    private static readonly ConditionalWeakTable<TV, TVScreenShape?> TVScreens = [];

    private static readonly Regex TVContextTag = new Regex(@"^calcifer_tv_x(-?\d+(?:\.\d+)?)y(-?\d+(?:\.\d+)?)s(-?\d+(?:\.\d+)?)$");

    [HarmonyPatch(typeof(Furniture), nameof(Furniture.GetFurnitureInstance))]
    [HarmonyPostfix]
    public static void Furniture_GetFurnitureInstance_Postfix(string itemId, ref Furniture __result)
    {
        foreach (string tag in __result.GetContextTags())
        {
            if (TVContextTag.IsMatch(tag))
            {
                __result = new TV(itemId, __result.TileLocation);
                return;
            }
        }
    }

    private static TVScreenShape? GetTVScreenShape(TV tv)
    {
        foreach (string tag in tv.GetContextTags())
        {
            if (TVContextTag.Match(tag) is Match tvTag && tvTag.Success &&
                float.TryParse(tvTag.Groups[1].Value, out float posX) &&
                float.TryParse(tvTag.Groups[2].Value, out float posY) &&
                float.TryParse(tvTag.Groups[3].Value, out float scale))
            {
                return new TVScreenShape(posX, posY, scale); ;
            }
        }
        return null;
    }

    [HarmonyPatch(typeof(TV), nameof(TV.getScreenPosition))]
    [HarmonyPostfix]
    public static void TV_getScreenPosition_Postfix(TV __instance, ref Vector2 __result)
    {
        if (TVScreens.GetValue(__instance, GetTVScreenShape) is TVScreenShape shape)
        {
            __result = new(__instance.boundingBox.X + shape.PosX, __instance.boundingBox.Y + shape.PosY);
        }
    }

    [HarmonyPatch(typeof(TV), nameof(TV.getScreenSizeModifier))]
    [HarmonyPostfix]
    public static void TV_getScreenSizeModifier_Postfix(TV __instance, ref float __result)
    {
        if (TVScreens.GetValue(__instance, GetTVScreenShape) is TVScreenShape shape)
        {
            __result = shape.Scale;
        }
    }

}